using System.Text.RegularExpressions;

namespace LsReader;

internal static class RegexExtension
{
    const string ESC = "¤";
    const char RETURN = '÷';

    public static string ReplaceFormatting(this string content, string node, String color, string before = "", string after = "")
    {
        return Regex.Replace(content, $"<{node}[^<>]*>([^<>]*)</{node}>", 
            match => 
                before + 
                (string.IsNullOrWhiteSpace(color) ? "" : $"{ESC}{color}") +
                $"{match.Groups[1].Value.Trim()}" +
                (string.IsNullOrWhiteSpace(color) ? "" : $"{ESC}{RETURN}") +
                after,
            RegexOptions.Compiled);
    }

    public static string RemoveCompletely(this string content, string node)
    {
        var result = Regex.Replace(content, $"<{node}[^<>]*>([^<>]*)</{node}>", match => "", RegexOptions.Compiled);
        result = Regex.Replace(result, $"<{node}[^<>]*/>", match => "", RegexOptions.Compiled);

        return result;
    }
}
