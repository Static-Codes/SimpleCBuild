# Core Ideology

1. There will never be 100% true test coverage. In the words of Vince Lombardi, **“Gentlemen, we will chase perfection, and we will chase it relentlessly, knowing all the while we can never attain it. But along the way, we shall catch excellence.”**

2. While this project is already an incredible undertaking, significant boundaries needed to be drawn. These boundaries primarily concerned the environment(s) the project's support would cover. The most feasible solution was to limit this support to Desktop/Server environments. Further specificity is required; these machines must be running a single x64 and ARM64 processor. While this was not much of a concern for ARM64, many dual-processor x64 configurations exist. As such, the project will support dual socket systems, however, these systems must be be in a single-populated socket configuration to avoid unexpected behavior (unless otherwise specified).

3. Most build systems will be parsed and converted to CMake. This is done to ensure uniformity for any potential future contributors. While this has it's own shortcomings (namely the introduction of microservices), translation currently provides the best medium between user-accessibility and developer maintainability. Please note this currently excludes Meson.

# General Nuisances

1. Any implementation(s) of `IDockerInstruction` should be use a `record` type to avoid unintended mutations and/or unexpected behavior within previous `IDockerInstruction` implementations, i.e. [`FromInstruction`](https://github.com/Static-Codes/EasyDockerFile/tree/main/src/DockerFileSharp/Instructions/FromInstruction.cs)
    - Note: Each implementation of `IDockerInstruction` uses the following syntax:
        ```c#
        using DockerFileSharp.Common;

        namespace DockerFileSharp.Instructions;

        public record NameOfInstruction(args) : IDockerInstruction
        {
            // Each Implementation of IDockerInstruction requires a Build() method. 
            public string Build()
            {
                // The implementation's build action(s) go here.

                // Returning the result from the hypothetical events above.
                return "result";
            }
        }

        ```
