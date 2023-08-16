using System.Text.RegularExpressions;

namespace LsReader;

static class RegexExtension
{
    const string ESC = "¤";
    const char RETURN = '÷';

    public static string ReplaceFormatting(this string content, string node, String color, string before = "", string after = "")
    {
        return Regex.Replace(content, $"<{node}[^<>]*>([^<>]*)</{node}>", 
            match => 
                before + 
                $"{ESC}{color}{match.Groups[1].Value}{ESC}{RETURN}" +
                after,
            RegexOptions.Compiled);
    }
}
