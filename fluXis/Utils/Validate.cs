using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace fluXis.Utils;

public static class Validate
{
    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string REGEX_HEX_COLOR = "^#(?:[0-9a-fA-F]{3}){1,2}$";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string USERNAME = "^[a-zA-Z0-9_]{3,16}$";

    public static bool Matches(this string input, string regex) => Regex.IsMatch(input, regex);
}
