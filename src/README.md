# Components

##### DockerFileSharp
- `DockerFileSharp` is a simple C# wrapper for .dockerfile generation. 
- This library currently it contains all .dockerfile instructions, but does not include all flags for each of these instructions.

##### EasyDockerFile
- [`EasyDockerFile`](./EasyDockerFile/) contains the libraries used for the operations executed by [`SimpleCBuild`](./SimpleCBuild). 
- These include but are not limited to: 
    - Repository parsing
    - Build process parsing
    - Binary loader logic

##### SimpleCBuild
- [`SimpleCBuild`](./SimpleCBuild/) contains the application's workflow.


# Sandboxes

##### DockerFileSharp.Sandbox
- [`DockerFileSharp.Sandbox`](./DockerFileSharp.Sandbox/) used for tests in [`DockerFileSharp`](./DockerFileSharp/)

##### EasyDockerFile.Sandbox
-  [`EasyDockerFile.Sandbox`](./EasyDockerFile.Sandbox/) used for tests in [`EasyDockerFile`](./EasyDockerFile/)