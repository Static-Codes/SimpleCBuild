# 🚧 Under Construction 🚧
## Cross platform one-click build utility for native c and C++. 

## How it will work:
  - `SimpleCBuild --repo=<path/to/repo>` or `SimpleCBuild --private --repo=<path/to/repo> --token=<OAuthToken>`
  - Attempts to authenticate the session (if `--private` is passed)
  - Prompts the user to select a branch to build from.
  - Returns a list of ALL files in that specific branch
  - Parses these files for specific c/c++ build processes and toolchains
  - Prompts the user to select an architecture to build for.
  - Creates a container using docker to prevent a dependency hell
  - Builds for the specified architecture.
  - Spits out a binary.
