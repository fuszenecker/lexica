using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace LsReader;

class LsDictionary
{
    private const string DictionaryPath = "Dictionaries";
    const string ESC = "¤";
    const string printable = "[a-zA-Z0-9:.,ē]";

    private IList<XElement> _dictionaries = new List<XElement>();
    private int _tabWidth = 4;

    public LsDictionary(int tabWidth = 4)
    {
        _tabWidth = tabWidth;
        LoadDictionaries();
    }

    public void LookupHeadword(string? headwordOptionValue)
    {
        if (headwordOptionValue is null)
        {
            return;
        }

        var results = new List<XElement>();
        var regex = new Regex($"^{headwordOptionValue}$", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
        
        foreach (var dictionary in _dictionaries)
        {
            foreach (var entry in dictionary.Descendants("entryFree"))
            {
                if (entry.Attribute("key")?.Value is not null && regex.IsMatch(entry.Attribute("key")!.Value) 
                    || entry.Element("ort")?.Value is not null && regex.IsMatch(entry.Element("ort")!.Value))
                {
                    results.Add(entry);
                }
            }
        }

        PrintResults(results, headwordOptionValue);
    }

    private void PrintResults(List<XElement> results, string highlights)
    {
        Console.ForegroundColor = ConsoleColor.Gray;

        foreach (var element in results) 
        {
            var content = element.ToString();

            Console.WriteLine();
            Console.WriteLine("-- DEBUG -------------------------------");
            Console.WriteLine(content);
            Console.WriteLine("-- DEBUG -------------------------------");
            Console.WriteLine();

            content = Regex.Replace(content, "<orth[^<>]+>([^<>]+)</orth>", match => $"{ESC}G{match.Groups[1].Value}{ESC}g", RegexOptions.Compiled);
            content = Regex.Replace(content, "<itype>([^<>]+)</itype>", match => $"{ESC}G{match.Groups[1].Value}{ESC}g", RegexOptions.Compiled);
            content = Regex.Replace(content, "<gen>([^<>]+)</gen>", match => $"{ESC}G{match.Groups[1].Value}{ESC}g", RegexOptions.Compiled);
            content = Regex.Replace(content, "<case>([^<>]+)</case>", match => $"{ESC}Y{match.Groups[1].Value}{ESC}g", RegexOptions.Compiled);
            content = Regex.Replace(content, "<etym>([^<>]+)</etym>", match => $"\n{ESC}Y{match.Groups[1].Value}{ESC}g", RegexOptions.Compiled);

            content = Regex.Replace(content, "<hi[^<>]*>([^<>]+)</hi>", match => $"{ESC}M{match.Groups[1].Value}{ESC}g", RegexOptions.Compiled);
            content = Regex.Replace(content, "<usg[^<>]*>([^<>]+)</usg>", match => $"{ESC}B{match.Groups[1].Value}{ESC}g", RegexOptions.Compiled);

            content = Regex.Replace(content, "<author>([^<>]+)</author>", match => match.Groups[1].Value, RegexOptions.Compiled);
            content = Regex.Replace(content, "<tr>([^<>]+)</tr>", match => match.Groups[1].Value, RegexOptions.Compiled);
            content = Regex.Replace(content, "<trans[^<>]*>([^<>]+)</trans>", match => $"{match.Groups[1].Value} ", RegexOptions.Compiled);
            content = Regex.Replace(content, "<bibl[^<>]*>([^<>]+)</bibl>", match => $"{ESC}D{match.Groups[1].Value}{ESC}g", RegexOptions.Compiled);
            content = Regex.Replace(content, "<quote[^<>]*>([^<>]+)</quote>", match => $"{ESC}R{match.Groups[1].Value}{ESC}g ", RegexOptions.Compiled);
            content = Regex.Replace(content, "<cit[^<>]*>([^<>]+)</cit>", match => match.Groups[1].Value, RegexOptions.Compiled);
            content = Regex.Replace(content, "<pos[^<>]*>([^<>]+)</pos>", match => match.Groups[1].Value, RegexOptions.Compiled);
            content = Regex.Replace(content, "<cb[^<>]*/>", match => match.Groups[1].Value, RegexOptions.Compiled);
            content = Regex.Replace(content, "<foreign[^<>]*>([^<>]+)</foreign>", match => $"{ESC}y{match.Groups[1].Value}{ESC}g", RegexOptions.Compiled);

            content = Regex.Replace(content, "<sense[^<>]*>([^<>]+)</sense>", match => $"\n{match.Groups[1].Value}", RegexOptions.Compiled);
            content = Regex.Replace(content, "<entryFree[^<>]*>([^<>]+)</entryFree>", match => $"{match.Groups[1].Value}\n", RegexOptions.Compiled);

            content = Regex.Replace(content, $"({highlights})", match => $"{ESC}Y{match.Value}{ESC}g", RegexOptions.Compiled);

            PrintInColors(content.Trim());
        }
    }

    private void PrintInColors(string value)
    {
        var parts = value.Split(ESC);

        foreach (var part in parts)
        {
            if (part.StartsWith("W"))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.Write(part[1..]);
            }
            else if (part.StartsWith("G"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(part[1..]);
            }
            else if (part.StartsWith("g"))
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(part[1..]);
            }
            else if (part.StartsWith("B"))
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(part[1..]);
            }
            else if (part.StartsWith("Y"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(part[1..]);
            }
            else if (part.StartsWith("D"))
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(part[1..]);
            }
            else if (part.StartsWith("R"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(part[1..]);
            }
            else if (part.StartsWith("M"))
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(part[1..]);
            }
            else if (part.StartsWith("y"))
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(part[1..]);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(part);
            }
        }
    }

    private void LoadDictionaries()
    {
        foreach (var file in Directory.EnumerateFiles(DictionaryPath, "*.xml"))
        {
            var dictionary = XElement.Load(file);
            _dictionaries.Add(dictionary);
        }
    }
}