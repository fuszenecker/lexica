// See https://aka.ms/new-console-template for more information
using System.Xml.Linq;

var dictionary = XElement.Load("Dictionaries/lat.ls.perseus-eng2.xml");

var tagsList = new List<HashSet<string>>();
var leafTags = new HashSet<string>();
var finished = false;

do
{
    leafTags = new HashSet<string>();
    tagsList.Add(leafTags);
 
    finished = Analyze(dictionary, leafTags);

    foreach (var tags in tagsList.SkipLast(1))
    {
        tags.ExceptWith(leafTags);
    }
} while (!finished);

foreach (var tags in tagsList)
{
    if (tags.Any())
    {
        Console.WriteLine(String.Join(", ", tags));
    }
}

static bool Analyze(XElement node, HashSet<string> leafTags)
{
    var descendants = node.Elements();

    if (descendants.Any())
    {
        foreach (var child in descendants)
        {
            Analyze(child, leafTags);
        }

        return false;
    }
    else
    {
        if (!leafTags.Contains(node.Name.ToString()))
        {
            leafTags.Add(node.Name.ToString());
        }

        if (node.Parent != null)
            node.Remove();

        return true;
    }
}