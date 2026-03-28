# 🚧 Under Construction 🚧
## User-friendly Cross-platform build utility for native c and C++. 

## How it will work:
  - `SimpleCBuild --repo=<path/to/repo>` or `SimpleCBuild --private --repo=<path/to/repo> --token=<OAuthToken>`
  - Attempts to authenticate the session (if `--private` is passed)
  - Prompts the user to select a branch to build from.
  - Returns a list of ALL files in that specific branch (from the last commit)
  - Parses these files for specific c/c++ build processes and toolchains
  - Queries the languages used in the repository to further validate the build system usage, and apply priority to cmake.
  - Prompts the user to select an architecture to build for.
  - Prompts the user to select an output type (either an executable, shared library, or static library).
  - Creates a container using docker to prevent a dependency hell.
  - Builds for the specified architecture.
  - Spits out a binary in the selected output type.
