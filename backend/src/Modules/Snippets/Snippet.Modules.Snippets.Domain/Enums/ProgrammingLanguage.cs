namespace Snippet.Modules.Snippets.Domain.Enums;

/// <summary>
/// Represents supported programming languages and formats for syntax highlighting.
/// </summary>
public enum ProgrammingLanguage
{
    /// <summary>C# programming language</summary>
    CSharp = 1,

    /// <summary>JavaScript programming language</summary>
    JavaScript = 2,

    /// <summary>TypeScript programming language</summary>
    TypeScript = 3,

    /// <summary>Python programming language</summary>
    Python = 4,

    /// <summary>Java programming language</summary>
    Java = 5,

    /// <summary>Go programming language</summary>
    Go = 6,

    /// <summary>Rust programming language</summary>
    Rust = 7,

    /// <summary>C++ programming language</summary>
    Cpp = 8,

    /// <summary>PHP programming language</summary>
    Php = 9,

    /// <summary>Ruby programming language</summary>
    Ruby = 10,

    /// <summary>SQL query language</summary>
    Sql = 11,

    /// <summary>Kusto Query Language (KQL)</summary>
    Kql = 12,

    /// <summary>GraphQL query language</summary>
    GraphQL = 13,

    /// <summary>HTML markup language</summary>
    Html = 14,

    /// <summary>CSS stylesheet language</summary>
    Css = 15,

    /// <summary>JSON data format</summary>
    Json = 16,

    /// <summary>YAML data format</summary>
    Yaml = 17,

    /// <summary>XML markup language</summary>
    Xml = 18,

    /// <summary>Markdown markup language</summary>
    Markdown = 19,

    /// <summary>Bash shell scripting</summary>
    Bash = 20,

    /// <summary>PowerShell scripting</summary>
    PowerShell = 21,

    /// <summary>Plain text without syntax highlighting</summary>
    PlainText = 22
}
