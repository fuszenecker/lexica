using System.Drawing;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace LsReader;

class LsDictionary
{
    private const string DictionaryPath = "Dictionaries";
    const char ESC = '¤';
    const char RETURN = '÷';
    static readonly Stack<char> _colorStack = new();

    private readonly IList<XElement> _dictionaries = new List<XElement>();

    public LsDictionary()
    {
        LoadDictionaries();
    }

    public void LookupHeadword(string? term)
    {
        if (term is null)
        {
            return;
        }

        _colorStack.Clear();

        var results = new List<XElement>();
        var regex = new Regex($"^{term}$", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
        
        foreach (var dictionary in _dictionaries)
        {
            foreach (var entry in dictionary.Descendants("entryFree"))
            {
                if (entry.Attribute("key")?.Value is not null && regex.IsMatch(entry.Attribute("key")!.Value) 
                    || entry.Element("orth")?.Value is not null && regex.IsMatch(entry.Element("orth")!.Value))
                {
                    results.Add(entry);
                }
            }
        }

        PrintResults(results, term);
    }

    public void LookupContent(string? term)
    {
        if (term is null)
        {
            return;
        }

        _colorStack.Clear();

        var results = new List<XElement>();
        var regex = new Regex($"{term}", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        
        foreach (var dictionary in _dictionaries)
        {
            foreach (var entry in dictionary.Descendants("entryFree"))
            {
                if (entry?.Descendants("hi").Any(e => regex.IsMatch(e.Value)) == true)
                {
                    results.Add(entry);
                }
            }
        }

        PrintResults(results, term);
    }

    public void LookupFullSearch(string? term)
    {
        if (term is null)
        {
            return;
        }

        _colorStack.Clear();

        var results = new List<XElement>();
        var regex = new Regex($"{term}", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        
        foreach (var dictionary in _dictionaries)
        {
            foreach (var entry in dictionary.Descendants("entryFree"))
            {
                if (entry?.Value is not null && regex.IsMatch(entry!.Value))
                {
                    results.Add(entry);
                }
            }
        }

        PrintResults(results, term);
    }

    private void PrintResults(List<XElement> results, string highlights)
    {
        Console.ForegroundColor = ConsoleColor.Gray;

        foreach (var element in results) 
        {
            var content = element.ToString();

            // Headword.
            content = content
                // Headword.
                .ReplaceFormatting("orth", "=")

                // Declension or conjugation.
                .ReplaceFormatting("itype", "=")

                // Author
                .ReplaceFormatting("author", "g")

                ;

            Console.WriteLine("-------------------------------------------------------------------------------");
            PrintInColors($"{ESC}D{content.Trim()}");
            Console.WriteLine();
        }
    }

    private void PrintInColors(string value)
    {
        var parts = value.Split(ESC);

        foreach (var part in parts)
        {
            if (part.Length == 0)
            {
                continue;
            }

            var color = part[0];

            if (color == RETURN)
            {
                _colorStack.Pop();
                color = _colorStack.Peek();
            }
            else
            {
                _colorStack.Push(color);
            }

            switch (color)
            {
                case 'W':
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case 'G':
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case 'g':
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case 'B':
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case 'Y':
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case 'D':
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case 'R':
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case 'M':
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case 'y':
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case 'C':
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case 'r':
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case '_':
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write(part[1..]);
                    break;

                case '=':
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.Write(part[1..]);
                    break;

                default:
                    Console.Write(part);
                    break;
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