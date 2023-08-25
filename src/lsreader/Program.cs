using System.CommandLine;
using System.Reflection;
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

rootCommand.SetHandler((headwordTerm, meaningTerm, fullSearchTerm) =>
{
    var dictionary = new LsDictionary();

    if (!string.IsNullOrEmpty(headwordTerm))
    {
        dictionary.LookupHeadword(headwordTerm);
    }

    if (!string.IsNullOrEmpty(meaningTerm))
    {
        dictionary.LookupMeaning(meaningTerm);
    }

    if (!string.IsNullOrEmpty(fullSearchTerm))
    {
        dictionary.LookupFullSearch(fullSearchTerm);
    }
}, headwordOption, meaningOption, fullSearchOption);

try
{
    rootCommand.Invoke(args);
}
catch (TargetInvocationException ex)
{
    Console.WriteLine(ex.InnerException?.Message);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
