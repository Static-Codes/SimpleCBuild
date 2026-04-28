## Code Model Information

- This document contains a breakdown of the serialized classes in `CodeModelTypes` alongside each sub-classes members.


### CodeModelRoot
- **Role**: This is the entry point of the generated `codemodel-v2*.json` file.

    #### Configurations
    - **Role**: A list of `CodeModelConfiguration` objects. A properly written project will contain atleast one build configuration.

    #### Kind
    - **Role**: This will be a constant value of `"codemodel"`. It is used by the API to signal the schema in use.

    #### Paths
    - **Role**: Contains two string members `Build` and `Source`, and their associated top-level build and source directory paths.

    #### Version
    - **Role**: Contains two members `Major` and `Minor`, and their associated values from the file api object.

### CodeModelConfiguration
- **Role**: Describes a specific build configuration (e.g., Debug or Release).

    #### Directories
    - **Role**: A list of `CodeModelDirectory` objects representing every directory in the project containing a `CMakeLists.txt` file.

    #### Name
    - **Role**: The name of the configuration, such as `"Debug"`, `"Release"`, or `"RelWithDebInfo"`.

    #### Projects
    - **Role**: A list of `CodeModelProject` objects representing the logical projects defined within the build tree.

    #### Targets
    - **Role**: A list of `CodeModelTarget` objects representing all buildable artifacts like executables and libraries.

### CodeModelDirectory
- **Role**: Represents a source directory within the CMake build tree.

    #### Build
    - **Role**: A string path to the output directory relative to the root build directory.

    #### ChildIndexes
    - **Role**: A list of integers acting as indexes into the top-level `Directories` list for any subdirectories.

    #### JsonFile
    - **Role**: A relative path to a separate JSON file providing supplementary details for this specific directory.

    #### MinimumCMakeVersion
    - **Role**: A `CodeModelMinimumCMakeVersion` object specifying the version required by the directory.

    #### ProjectIndex
    - **Role**: An integer index into the `Projects` list identifying which project owns this directory.

    #### Source
    - **Role**: A string path to the directory relative to the root source directory.

    #### ParentIndex
    - **Role**: An integer index into the `Directories` list identifying the parent directory of this entry.

    #### TargetIndexes
    - **Role**: A list of integer indexes into the `Targets` list for targets defined within this directory.

### CodeModelProject
- **Role**: Represents a project scope defined by the `project()` command in CMake.

    #### DirectoryIndexes
    - **Role**: A list of integer indexes into the `Directories` list that belong to this project.

    #### Name
    - **Role**: The logical name assigned to the project via the `project()` command.

    #### TargetIndexes
    - **Role**: A list of integer indexes into the `Targets` list that are owned by this project.

### CodeModelTarget
- **Role**: Represents a buildable target, such as an executable, library, or custom command.

    #### DirectoryIndex
    - **Role**: An integer index into the `Directories` list where this target is defined.

    #### Id
    - **Role**: A unique, persistent identifier for the target.

    #### JsonFile
    - **Role**: A relative path to a separate JSON file containing deep metadata (sources, flags, dependencies) for the target.

    #### Name
    - **Role**: The name of the target artifact.

    #### ProjectIndex
    - **Role**: An integer index into the `Projects` list identifying the project that owns this target.

### CodeModelPaths
- **Role**: Provides absolute filesystem paths for the environment.

    #### Build
    - **Role**: The absolute path to the root build directory.

    #### Source
    - **Role**: The absolute path to the root source directory.

### CodeModelMinimumCMakeVersion
- **Role**: Contains the version constraints for the CMake environment.

    #### String
    - **Role**: The raw version string (e.g., `"3.22"`) parsed from `cmake_minimum_required`.

### CodeModelVersion
- **Role**: Tracks the schema version of the File API.

    #### Major
    - **Role**: The major version integer of the codemodel schema.

    #### Minor
    - **Role**: The minor version integer of the codemodel schema.
