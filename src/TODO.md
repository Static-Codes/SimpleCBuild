|  Icon  |   Key Name        |
|  :---: |     :---:         |
|   ❌   | Not Started       |
|   ⚠️   | In-Progress       |
|   👎   | Needs Improvement |
|   ✅   | Fully Implemented |

## **General Logic Additions**

#### 1. Build system parsing logic
| Name      | Completed? |
| :---:     | :---:      |
| Autotools | ❌         |
| Bazel     | ❌         |
| CMake     | ❌         |
| Make      | ❌         |
| Meson     | 👎         |
| MSBuild   | ❌         |


#### 2. Dependency resolution logic from build systems
| Name      | Approach           | Completed? |
| :---:     | :---:              | :---:      |
| Autotools | Converted to CMake | ❌         |
| Bazel     | Native             | ❌         |
| CMake     | Native             | ❌         |
| Make      | Native             | ❌         |
| Meson     | Native             | 👎         |
| MSBuild   | Native             | ❌         |

## Add a `<build_commands>` section to each `<system>` in `BuildSystems.xml`

