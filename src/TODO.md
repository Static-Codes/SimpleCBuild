|  Icon  |   Key Name        |
|  :---: |     :---:         |
|   ❌   | Not Started       |
|   ⚠️   | In-Progress       |
|   👎   | Needs Improvement |
|   ✅   | Fully Implemented |


## Parsing Logic Progress (Commands + Dependencies)
### This entails the command and dependency parsing for each of the supported build systems.
| Name      | Completed? | Notes               |
| :---:     | :---:      | :---:               |
| Autotools | ❌         | Handled by translating to CMake.
| Bazel     | ❌         | Handled by translating to CMake.
| CMake     | ❌         |
| Make      | ❌         | Handled by translating to CMake.
| Meson     | 👎         | 
| MSBuild   | ❌         | Handled by translating to CMake.


## System Processing Logic

### This entails the processing from the supported build systems. Only CMake and Meson are to be handled natively, the remaining will be translated to CMake.

| Name         | Method                         | Completed? |
| :---:        | :---:                          | :---:      |
| Autotools    | Translation (`auto2cmake.py`)  | 👎         |
| Bazel        | Translation (`bazel2cmake.py`) | 👎         |
| CMake        | Native                         | ❌         |
| Make         | Translation (`make2cmake.py`)  | ❌         |
| Meson        | Native                         | 👎         |
| MSBuild      | TBD                            | ❌         |


### Add a `<build_commands>` section to each `<system>` in `BuildSystems.xml`

