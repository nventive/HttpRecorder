using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:The file header is missing or not located at the top of the file", Justification = "Not needed in this app")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:Field names must not begin with an underscore", Justification = "Stylistic choice")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:Prefix local calls with this", Justification = "Stylistic choice")]
[assembly: SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1005:Single line comments must begin with single space", Justification = "Prevents quick edits during development")]
[assembly: SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1515:Single-line comment must be preceded by blank line", Justification = "Prevents quick edits during development")]

[assembly: SuppressMessage("Reliability", "CA2007:Do not directly await a Task", Justification = "Not needed systematically.")]
[assembly: SuppressMessage("Usage", "CA2234:Pass system uri objects instead of strings", Justification = "String is fine - Uri class is sometime cumbersome.")]
