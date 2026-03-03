// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Maintainability", "AV1564:Avoid signatures that take a `bool` parameter", Justification = "Useful for providing InlineData for tests")]
[assembly: SuppressMessage(
    "Style",
    "IDE0058:Expression value is never used",
    Justification = "Disables redundant assignment forcing for fluent assertion expressions")]
