# Core Ideology

1. There will never be 100% true test coverage; in the words of Vince Lombardi, “Gentlemen, we will chase perfection, and we will chase it relentlessly, knowing all the while we can never attain it. But along the way, we shall catch excellence.” 


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