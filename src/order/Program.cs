// See https://aka.ms/new-console-template for more information
using System.Xml.Linq;

var dictionary = XElement.Load("Dictionaries/lat.ls.perseus-eng2.xml");

var taglist = new List<List<string>>();
var leveltags = new List<string>();
var finished = false;

do
{
    leveltags = new List<string>();
    taglist.Add(leveltags);
    finished = Analyze(dictionary);

    foreach (var tags in taglist.SkipLast(1))
    {
        var remove = new List<string>();

        foreach (var tag in tags)
        {
            if (leveltags.Contains(tag))
            {
                remove.Add(tag);
            }
        }

        foreach (var tag in remove)
        {
            tags.Remove(tag);
        }
    }
} while (!finished);

foreach (var tags in taglist)
{
    if (tags.Count > 0)
    {
        Console.WriteLine(String.Join(", ", tags));
    }
}

bool Analyze(XElement node)
{
    var descendants = node.Elements();

    if (descendants.Count() > 0)
    {
        foreach (var child in descendants)
        {
            Analyze(child);
        }

        return false;
    }
    else
    {
        if (!leveltags.Contains(node.Name.ToString()))
        {
            leveltags.Add(node.Name.ToString());
        }

        if (node.Parent != null)
            node.Remove();

        return true;
    }
}