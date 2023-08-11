using System.CommandLine;
using LsReader;

var headwordOption = new Option<string?>(
    new string[] { "--headword", "-h" },
    "The headword to search for (regex)");

var contentOption = new Option<string?>(
    new string[] { "--content", "-c" },
    "The content to search for (regex)");

var tabWidthOption = new Option<int>(
    new string[] { "--tab-width", "-t" },
    () => 4,
    "Width of the tab character");

var rootCommand = new RootCommand();
rootCommand.Description = "Searches the Lewis and Short dictionary for a headword and/or content";
rootCommand.Add(headwordOption);
rootCommand.Add(contentOption);
rootCommand.Add(tabWidthOption);

rootCommand.SetHandler<string?, string?, int>((headwordOptionValue, contentOptionValue, tabWidthOptionValue) =>
{
    var dictionary = new LsDictionary(tabWidthOptionValue);
    dictionary.LookupHeadword(headwordOptionValue);
    // dictionary.LookupContent(contentOptionValue);
}, headwordOption, contentOption, tabWidthOption);

rootCommand.Invoke(args);
