# Components

##### DockerFileSharp
- [`DockerFileSharp`](./DockerFileSharp/) is a simple C# wrapper for .dockerfile generation. 
- This library currently it contains all .dockerfile instructions, but does not include all flags for a few of these instructions, namely [`CopyInstruction`](./DockerFileSharp/Instructions/CopyInstruction.cs).

##### EasyDockerFile
- [`EasyDockerFile`](./EasyDockerFile/) utilizes `DockerFileSharp` and contains the code used for the operations executed by [`SimpleCBuild`](./SimpleCBuild). 
- This includes but are not limited to: 
    - Repository parsing
    - Build process parsing
    - Resource loader logic

##### SimpleCBuild
- [`SimpleCBuild`](./SimpleCBuild/) contains the application's workflow.


# Sandboxes

##### DockerFileSharp.Sandbox
- [`DockerFileSharp.Sandbox`](./DockerFileSharp.Sandbox/) used for testing [`DockerFileSharp`](./DockerFileSharp/)

##### EasyDockerFile.Sandbox
-  [`EasyDockerFile.Sandbox`](./EasyDockerFile.Sandbox/) used for testing [`EasyDockerFile`](./EasyDockerFile/)
