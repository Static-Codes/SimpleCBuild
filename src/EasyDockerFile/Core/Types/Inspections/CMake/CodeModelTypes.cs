using System.Text.Json.Serialization;

namespace EasyDockerFile.Core.Types.Inspections.CMake;

public class CodeModelTypes 
{
    public class CodeModelConfiguration
    {
        [JsonPropertyName("directories")]
        public List<CodeModelDirectory> Directories { get; set; } = [];

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("projects")]
        public List<CodeModelProject> Projects { get; set; } = [];

        [JsonPropertyName("targets")]
        public List<CodeModelTarget> Targets { get; set; } = [];
    }

    public class CodeModelDirectory
    {
        [JsonPropertyName("build")]
        public string? Build { get; set; }

        [JsonPropertyName("childIndexes")]
        public List<int?> ChildIndexes { get; set; } = [];

        [JsonPropertyName("jsonFile")]
        public string? JsonFile { get; set; }

        [JsonPropertyName("minimumCMakeVersion")]
        public MinimumCMakeVersion? MinimumCMakeVersion { get; set; }

        [JsonPropertyName("projectIndex")]
        public int? ProjectIndex { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("parentIndex")]
        public int? ParentIndex { get; set; }

        [JsonPropertyName("targetIndexes")]
        public List<int?> TargetIndexes { get; set; } = [];
    }

    public class MinimumCMakeVersion
    {
        [JsonPropertyName("string")]
        public string? String { get; set; }
    }

    public class CodeModelPaths
    {
        [JsonPropertyName("build")]
        public string? Build { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }
    }

    public class CodeModelProject
    {
        [JsonPropertyName("directoryIndexes")]
        public List<int?> DirectoryIndexes { get; } = [];

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("targetIndexes")]
        public List<int?> TargetIndexes { get; } = [];
    }

    public class CodeModelRoot
    {
        [JsonPropertyName("configurations")]
        public List<CodeModelConfiguration> Configurations { get; set; } = [];

        [JsonPropertyName("kind")]
        public string? Kind { get; set; }

        [JsonPropertyName("paths")]
        public CodeModelPaths? Paths { get; set; }

        [JsonPropertyName("version")]
        public CodeModelVersion? Version { get; set; }
    }

    public class CodeModelTarget
    {
        [JsonPropertyName("directoryIndex")]
        public int? DirectoryIndex { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("jsonFile")]
        public string? JsonFile { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("projectIndex")]
        public int? ProjectIndex { get; set; }
    }

    public class CodeModelVersion
    {
        [JsonPropertyName("major")]
        public int? Major { get; set; }

        [JsonPropertyName("minor")]
        public int? Minor { get; set; }
    }

    public class CodeModelReplyTarget 
    {
        public class Artifact
        {
            [JsonPropertyName("path")]
            public string Path { get; set; } = string.Empty;
        }

        public class BacktraceGraph
        {
            [JsonPropertyName("commands")]
            public List<string> Commands { get; set; } = [];

            [JsonPropertyName("files")]
            public List<string> Files { get; set; } = [];

            [JsonPropertyName("nodes")]
            public List<Node> Nodes { get; set; } = [];
        }

        public class CommandFragment
        {
            [JsonPropertyName("fragment")]
            public string Fragment { get; set; } = string.Empty;

            [JsonPropertyName("role")]
            public string Role { get; set; } = string.Empty;

            [JsonPropertyName("backtrace")]
            public int? Backtrace { get; set; }
        }

        public class CompileGroup
        {
            [JsonPropertyName("defines")]
            public List<Define> Defines { get; set; } = [];

            [JsonPropertyName("includes")]
            public List<Include> Includes { get; set; } = [];

            [JsonPropertyName("language")]
            public string Language { get; set; } = string.Empty;

            [JsonPropertyName("sourceIndexes")]
            public List<int> SourceIndexes { get; set; } = [];
        }

        public class Define
        {
            [JsonPropertyName("define")]
            public string Definition { get; set; } = string.Empty;
        }

        public class Include
        {

            [JsonPropertyName("path")]
            public string Path { get; set; } = string.Empty;

            [JsonPropertyName("isSystem")]
            public bool IsSystem { get; set; }
            
            [JsonPropertyName("backtrace")]
            public int? Backtrace;
        }

        public class Link
        {
            [JsonPropertyName("commandFragments")]
            public List<CommandFragment> CommandFragments { get; set; } = [];

            [JsonPropertyName("language")]
            public string Language { get; set; } = string.Empty;
        }

        public class Node
        {
            [JsonPropertyName("file")]
            public int? File { get; set; }

            [JsonPropertyName("command")]
            public int? Command { get; set; }

            [JsonPropertyName("line")]
            public int? Line { get; set; }

            [JsonPropertyName("parent")]
            public int? Parent { get; set; }
        }

        public class Paths
        {
            [JsonPropertyName("build")]
            public string Build { get; set; } = string.Empty;

            [JsonPropertyName("source")]
            public string Source { get; set; } = string.Empty;
        }

        public class Root
        {
            [JsonIgnore]
            public string? ProjectName { get; set; }

            [JsonIgnore]
            public string? SourceDirectory { get; set; }

            [JsonPropertyName("artifacts")]
            public List<Artifact> Artifacts { get; set; } = [];

            [JsonPropertyName("backtrace")]
            public int? Backtrace { get; set; }

            [JsonPropertyName("backtraceGraph")]
            public BacktraceGraph? BacktraceGraph { get; set; }

            [JsonPropertyName("compileGroups")]
            public List<CompileGroup> CompileGroups { get; set; } = [];

            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;

            [JsonPropertyName("link")]
            public Link? Link { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("nameOnDisk")]
            public string NameOnDisk { get; set; } = string.Empty;

            [JsonPropertyName("paths")]
            public Paths? Paths { get; set; }

            [JsonPropertyName("sourceGroups")]
            public List<SourceGroup> SourceGroups { get; set; } = [];

            [JsonPropertyName("sources")]
            public List<Source> Sources { get; set; } = [];

            [JsonPropertyName("type")]
            public string Type { get; set; } = string.Empty;
        }

        public class Source
        {
            [JsonPropertyName("backtrace")]
            public int? Backtrace { get; set; }

            [JsonPropertyName("compileGroupIndex")]
            public int? CompileGroupIndex { get; set; }

            [JsonPropertyName("path")]
            public string Path { get; set; } = string.Empty;

            [JsonPropertyName("sourceGroupIndex")]
            public int? SourceGroupIndex { get; set; }
        }

        public class SourceGroup
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("sourceIndexes")]
            public List<int?> SourceIndexes { get; set; } = [];
        }
    }

    public class CodeModelReplyTargetGroup 
    {
        public string? ProjectName { get; set; }
        public string? TargetNames { get; set; }
        public string? SourcePaths { get; set; }
        public string? SourceLanguage { get; set; }
        public List<CodeModelReplyTarget.Define> Defines { get; set; } = [];
        public List<CodeModelReplyTarget.Include> Includes { get; set; } = [];
    }

}