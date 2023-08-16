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
        var regex = new Regex($"^{term}[0-9]*$", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
        
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

    public void LookupMeaning(string? term)
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
                .ReplaceFormatting("orth", "G")

                // Declension or conjugation.
                .ReplaceFormatting("itype", "G")

                // Declension or conjugation.
                .ReplaceFormatting("gen", "G")

                // Foreign word.
                .ReplaceFormatting("foreign", "y")

                // Author
                .ReplaceFormatting("author", "g")

                // Meaning:
                .ReplaceFormatting("hi", "B")

                // Translation of citation.
                .ReplaceFormatting("tr", "B")
                .ReplaceFormatting("trans", "", after: " ")

                // Bibl.
                .ReplaceFormatting("bibl", "g")

                // Quote.
                .ReplaceFormatting("quote", "Y", after: " ")
                
                // Citations.
                .ReplaceFormatting("cit", "")

                // Case.
                .ReplaceFormatting("case", "M")

                // Number.
                .ReplaceFormatting("number", "M")

                // Usage.
                .ReplaceFormatting("usg", "C")

                // Cb.
                .RemoveCompletely("cb")

                // Pb.
                .RemoveCompletely("pb")

                // Pos.
                .ReplaceFormatting("pos", "")

                // Etymology.
                .ReplaceFormatting("etym", "", before: " {", after: "}")

                // Sense.
                .ReplaceFormatting("sense", "", before: Environment.NewLine + Environment.NewLine)

                // Entry
                .ReplaceFormatting("entryFree", "");

                ;

            Regex regex = new Regex(highlights, RegexOptions.IgnoreCase);
            content = regex.Replace(content, match => $"{ESC}_{match.Value}{ESC}{RETURN}");

            Console.WriteLine("\n-------------------------------------------------------------------------------\n");
            Printer.PrintInColors($"{ESC}D{content.Trim()}\n");
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