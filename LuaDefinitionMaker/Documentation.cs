using System.Reflection;
using System.Xml;
using LuaDefinitionMaker.Docs;

namespace LuaDefinitionMaker;

public static class Documentation
{
    private static readonly Dictionary<string, XmlNode> entries = new();

    public static void Init(XmlDocument file)
    {
        foreach (XmlNode node in file.SelectNodes("/doc/members/member")!)
        {
            var name = node.Attributes?["name"]?.Value;
            if (name is null) continue;

            entries[name] = node;
        }
    }

    public static string? GetPropertySummary(PropertyInfo info)
    {
        var name = $"P:{info.DeclaringType?.FullName}.{info.Name}";
        var node = find(name);

        var summaryNode = node?.SelectSingleNode("summary");
        return summaryNode?.InnerText.Trim();
    }

    public static MethodDocumentation GetMethod(MethodInfo info)
    {
        var str = string.Join("", info.ToString()!.Split(" ")[1..]);
        var name = $"M:{info.DeclaringType?.FullName}.{info.Name}(";

        foreach (var parameter in info.GetParameters())
            name += $"{parameter.ParameterType},";

        if (name.EndsWith(',')) name = name[..^1];
        name += ")";

        var doc = new MethodDocumentation();
        var node = find(name);

        if (node is null) return doc;

        var summaryNode = node.SelectSingleNode("summary");
        if (summaryNode is not null)
            doc.Summary = summaryNode.InnerText.Trim();

        foreach (XmlNode paramNode in node.SelectNodes("param")!)
        {
            var paramName = paramNode.Attributes?["name"]?.Value;
            if (paramName is not null)
                doc.AddParameter(paramName, paramNode.InnerText.Trim());
        }

        var returnsNode = node.SelectSingleNode("returns");
        if (returnsNode is not null)
            doc.Returns = returnsNode.InnerText.Trim();

        return doc;
    }

    private static XmlNode? find(string name) => entries.TryGetValue(name, out var node) ? node : null;
}
