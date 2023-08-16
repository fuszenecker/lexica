using System.CommandLine;
using LsReader;

var headwordOption = new Option<string?>(
    new string[] { "--headword", "-h" },
    "The headword to search for (regex)");

var meaningOption = new Option<string?>(
    new string[] { "--meaning", "-m" },
    "The meaning to search for (regex)");

var fullSearchOption = new Option<string?>(
    new string[] { "--full-search", "-f" },
    "The content to search for (regex)");

var rootCommand = new RootCommand
{
    Description = "Searches the Lewis and Short dictionary for a headword and/or content"
};

rootCommand.Add(headwordOption);
rootCommand.Add(meaningOption);
rootCommand.Add(fullSearchOption);

rootCommand.SetHandler((headwordTerm, contentTerm, fullSearchTerm) =>
{
    var dictionary = new LsDictionary();

    if (!string.IsNullOrEmpty(headwordTerm))
    {
        dictionary.LookupHeadword(headwordTerm);
    }

    if (!string.IsNullOrEmpty(contentTerm))
    {
        dictionary.LookupContent(contentTerm);
    }

    if (!string.IsNullOrEmpty(fullSearchTerm))
    {
        dictionary.LookupFullSearch(fullSearchTerm);
    }
}, headwordOption, meaningOption, fullSearchOption);

rootCommand.Invoke(args);
