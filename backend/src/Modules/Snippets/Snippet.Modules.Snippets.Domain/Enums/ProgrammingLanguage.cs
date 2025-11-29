using System.ComponentModel.DataAnnotations;

namespace Snippet.Modules.Snippets.Domain.Enums;

/// <summary>
/// Represents supported programming languages and formats for syntax highlighting.
/// </summary>
public enum ProgrammingLanguage
{
    [Display(Description = "C# programming language")]
    CSharp = 1,

    [Display(Description = "JavaScript programming language")]
    JavaScript = 2,

    [Display(Description = "TypeScript programming language")]
    TypeScript = 3,

    [Display(Description = "Python programming language")]
    Python = 4,

    [Display(Description = "Java programming language")]
    Java = 5,

    [Display(Description = "Go programming language")]
    Go = 6,

    [Display(Description = "Rust programming language")]
    Rust = 7,

    [Display(Description = "C++ programming language")]
    Cpp = 8,

    [Display(Description = "PHP programming language")]
    Php = 9,

    [Display(Description = "Ruby programming language")]
    Ruby = 10,

    [Display(Description = "SQL query language")]
    Sql = 11,

    [Display(Description = "Kusto Query Language (KQL)")]
    Kql = 12,

    [Display(Description = "GraphQL query language")]
    GraphQL = 13,

    [Display(Description = "HTML markup language")]
    Html = 14,

    [Display(Description = "CSS stylesheet language")]
    Css = 15,

    [Display(Description = "JSON data format")]
    Json = 16,

    [Display(Description = "YAML data format")]
    Yaml = 17,

    [Display(Description = "XML markup language")]
    Xml = 18,

    [Display(Description = "Markdown markup language")]
    Markdown = 19,

    [Display(Description = "Bash shell scripting")]
    Bash = 20,

    [Display(Description = "PowerShell scripting")]
    PowerShell = 21,

    [Display(Description = "Plain text without syntax highlighting")]
    PlainText = 22
}
