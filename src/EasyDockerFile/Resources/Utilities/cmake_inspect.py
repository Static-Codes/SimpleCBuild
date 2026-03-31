# Ensure cmake and make are installed.

# Exit Codes:
# 1 -> Generic Failure
# 2 -> Invalid arguments provided.
# 3 -> queryFile was not created.
# 4 -> CMake execution failed.


import sys
sys.dont_write_bytecode = True

import subprocess
import shutil
from pathlib import Path
import os

if (len(sys.argv) != 2):
    print("Usage: cmake_inspect.py path/to/root/directory")
    exit(2)


repoDirectory = Path(sys.argv[1]).resolve()
buildDirectory = repoDirectory / "build"
apiDirectory = buildDirectory / ".cmake/api/v1"
queryRequestFilePath = apiDirectory / "query/codemodel-v2"
queryReplyDirectory = apiDirectory / "reply"



try:
    print(f"Making a CMake API query using local file at: {queryRequestFilePath}")


    if queryReplyDirectory.exists():
        print("Build artifacts remaining from the last execution, these will be deleted now.")
        shutil.rmtree(queryReplyDirectory)
        print("Removed successfully.")

    queryRequestFilePath.parent.mkdir(parents=True, exist_ok=True)
    
    # Creating the actual temporary file
    queryRequestFilePath.touch()

except Exception as e:
    print("Unable to create a temporary query file.")
    print(f"Error: {e}")
    sys.exit(3)

print("Executing 'cmake -S . -B build'")
try:
    result = subprocess.run(
        ["cmake", "-S", ".", "-B", "build"],
        cwd=repoDirectory,
        capture_output=False,
        text=True
    )
    
    if result.returncode != 0:
        print(f"CMake failed with exit code {result.returncode}")
        sys.exit(4)

except FileNotFoundError:
    print("Error: CMake was not found, please ensure it is installed.")
    sys.exit(1)

except Exception as e:
    print(f"An unexpected error occurred: {e}")
    sys.exit(4)

if queryReplyDirectory.exists():
    globPaths = queryReplyDirectory.glob("*.json")
    print("| ".join(globPaths))
    # for globPath in globPaths:
    #     print(f"{globPath.name}")


else:
    print("CMake execution completed, however, the API did not generate a reply.")