# 🚧 Under Construction 🚧
# User-friendly Cross-platform build utility for native c and C++. 

## Usage:
  ### For public repos
  `SimpleCBuild --repo=<path/to/repo>`

  ### For private repos
  `SimpleCBuild --private --repo=<path/to/repo> --token=<OAuthToken>`

## Workflow:
  ### Repo/Language Parsing
  1. Attempts to authenticate the session (if `--private` is passed)
  2. Prompts the user to select a branch to build from.
  3. Retrieves a list of files from the last commit in that specific branch.
  4. Parses these files for specific C/C++ build processes and toolchains
  5. Queries the languages used in the repository to further validate the build system usage, and apply priority to cmake.
  
  ### User Options
  1. Prompts the user to select an architecture to build for.
      ### Supported Types
        - X64
        - ARM64
  2. Prompts the user to select an output type.
      ### Supported Types
        - Executable
        - Shared Library
        - Static Library

  3. Prompts the user to select a compilation type.
     ### Supported Types
        
       - #### Quick and Dirty (Native)
         - Avoids translation, and attempts to compile a binary using generic build commands.
        
       - #### Experimental Translation

         ##### Autotools, Bazel, Make, MSBuild
         - Will attempt to translate the provided project to CMake then attempt a more nuanced build.

         ##### CMake
         - CMake is the "Gold Standard" for C/C++ projects, it will be the most adventageous target for translation due to its extensive API support; see the [CMake Index](https://cmake.org/cmake/help/latest/genindex.html) for more information.

         ##### Meson
         - Handled natively through parsing due to the lack of resources surrounding translation from Meson to CMake. There is functionality in Meson to convert a pre-existing CMake project to Meson, but not vice versa.

  4. Creates a container using docker to prevent a dependency hell.
  5. Performs a set of build operations within the container, and produces a binary of the selected output type.
