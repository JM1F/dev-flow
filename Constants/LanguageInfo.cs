﻿using System.Collections.Generic;
using dev_flow.Models;

namespace dev_flow.Constants;

/// <summary>
/// Contains all information about the supported languages.
/// </summary>
public static class LanguageInfo
{
    /// <summary>
    /// A list of all supported languages.
    /// </summary>
    public static readonly List<LanguageItem?> Languages = new List<LanguageItem?>
    {
        new LanguageItem { Name = "abap", DisplayName = "ABAP", Extension = ".abap" },
        new LanguageItem { Name = "apex", DisplayName = "Apex", Extension = ".cls" },
        new LanguageItem { Name = "azcli", DisplayName = "Azure CLI", Extension = ".azcli" },
        new LanguageItem { Name = "bat", DisplayName = "Batch", Extension = ".bat" },
        new LanguageItem { Name = "bicep", DisplayName = "Bicep", Extension = ".bicep" },
        new LanguageItem { Name = "cameligo", DisplayName = "Cameligo", Extension = ".mligo" },
        new LanguageItem { Name = "clojure", DisplayName = "Clojure", Extension = ".clj" },
        new LanguageItem { Name = "coffeescript", DisplayName = "CoffeeScript", Extension = ".coffee" },
        new LanguageItem { Name = "csharp", DisplayName = "C#", Extension = ".cs" },
        new LanguageItem { Name = "cpp", DisplayName = "C++", Extension = ".cpp" },
        new LanguageItem { Name = "dart", DisplayName = "Dart", Extension = ".dart" },
        new LanguageItem { Name = "dockerfile", DisplayName = "Dockerfile", Extension = ".dockerfile" },
        new LanguageItem { Name = "fsharp", DisplayName = "F#", Extension = ".fs" },
        new LanguageItem { Name = "flow9", DisplayName = "Flow9", Extension = ".flow" },
        new LanguageItem { Name = "foxpro", DisplayName = "FoxPro", Extension = ".prg" },
        new LanguageItem { Name = "go", DisplayName = "Go", Extension = ".go" },
        new LanguageItem { Name = "graphql", DisplayName = "GraphQL", Extension = ".graphql" },
        new LanguageItem { Name = "handlebars", DisplayName = "Handlebars", Extension = ".handlebars" },
        new LanguageItem { Name = "hcl", DisplayName = "HCL", Extension = ".hcl" },
        new LanguageItem { Name = "html", DisplayName = "HTML", Extension = ".html" },
        new LanguageItem { Name = "ini", DisplayName = "INI", Extension = ".ini" },
        new LanguageItem { Name = "java", DisplayName = "Java", Extension = ".java" },
        new LanguageItem { Name = "javascript", DisplayName = "JavaScript", Extension = ".js" },
        new LanguageItem { Name = "json", DisplayName = "JSON", Extension = ".json" },
        new LanguageItem { Name = "kotlin", DisplayName = "Kotlin", Extension = ".kt" },
        new LanguageItem { Name = "less", DisplayName = "Less", Extension = ".less" },
        new LanguageItem { Name = "lexon", DisplayName = "Lexon", Extension = ".lex" },
        new LanguageItem { Name = "lua", DisplayName = "Lua", Extension = ".lua" },
        new LanguageItem { Name = "markdown", DisplayName = "Markdown", Extension = ".md" },
        new LanguageItem { Name = "mips", DisplayName = "MIPS", Extension = ".s" },
        new LanguageItem { Name = "msdax", DisplayName = "DAX", Extension = ".dax" },
        new LanguageItem { Name = "mysql", DisplayName = "MySQL", Extension = ".sql" },
        new LanguageItem { Name = "objective-c", DisplayName = "Objective-C", Extension = ".m" },
        new LanguageItem { Name = "pascal", DisplayName = "Pascal", Extension = ".pas" },
        new LanguageItem { Name = "pascaligo", DisplayName = "Pascaligo", Extension = ".ligo" },
        new LanguageItem { Name = "perl", DisplayName = "Perl", Extension = ".pl" },
        new LanguageItem { Name = "pgsql", DisplayName = "PostgreSQL", Extension = ".sql" },
        new LanguageItem { Name = "php", DisplayName = "PHP", Extension = ".php" },
        new LanguageItem { Name = "postiats", DisplayName = "ATS", Extension = ".dats" },
        new LanguageItem { Name = "powerquery", DisplayName = "Power Query", Extension = ".pq" },
        new LanguageItem { Name = "powershell", DisplayName = "PowerShell", Extension = ".ps1" },
        new LanguageItem { Name = "proto", DisplayName = "Protocol Buffers", Extension = ".proto" },
        new LanguageItem { Name = "pug", DisplayName = "Pug", Extension = ".pug" },
        new LanguageItem { Name = "python", DisplayName = "Python", Extension = ".py" },
        new LanguageItem { Name = "r", DisplayName = "R", Extension = ".r" },
        new LanguageItem { Name = "razor", DisplayName = "Razor", Extension = ".cshtml" },
        new LanguageItem { Name = "redis", DisplayName = "Redis", Extension = ".redis" },
        new LanguageItem { Name = "redshift", DisplayName = "Redshift", Extension = ".sql" },
        new LanguageItem { Name = "ruby", DisplayName = "Ruby", Extension = ".rb" },
        new LanguageItem { Name = "rust", DisplayName = "Rust", Extension = ".rs" },
        new LanguageItem { Name = "sb", DisplayName = "Small Basic", Extension = ".sb" },
        new LanguageItem { Name = "scala", DisplayName = "Scala", Extension = ".scala" },
        new LanguageItem { Name = "scheme", DisplayName = "Scheme", Extension = ".scm" },
        new LanguageItem { Name = "scss", DisplayName = "SCSS", Extension = ".scss" },
        new LanguageItem { Name = "shell", DisplayName = "Shell Script", Extension = ".sh" },
        new LanguageItem { Name = "solidity", DisplayName = "Solidity", Extension = ".sol" },
        new LanguageItem { Name = "sophia", DisplayName = "Sophia", Extension = ".aes" },
        new LanguageItem { Name = "sql", DisplayName = "SQL", Extension = ".sql" },
        new LanguageItem { Name = "st", DisplayName = "Structured Text", Extension = ".st" },
        new LanguageItem { Name = "swift", DisplayName = "Swift", Extension = ".swift" },
        new LanguageItem { Name = "systemverilog", DisplayName = "SystemVerilog", Extension = ".sv" },
        new LanguageItem { Name = "tcl", DisplayName = "Tcl", Extension = ".tcl" },
        new LanguageItem { Name = "twig", DisplayName = "Twig", Extension = ".twig" },
        new LanguageItem { Name = "typescript", DisplayName = "TypeScript", Extension = ".ts" },
        new LanguageItem { Name = "vb", DisplayName = "Visual Basic", Extension = ".vb" },
        new LanguageItem { Name = "verilog", DisplayName = "Verilog", Extension = ".v" },
        new LanguageItem { Name = "xml", DisplayName = "XML", Extension = ".xml" },
        new LanguageItem { Name = "yaml", DisplayName = "YAML", Extension = ".yaml" }
    };
}