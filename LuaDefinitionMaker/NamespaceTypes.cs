using System.Reflection;
using System.Text;
using fluXis.Scripting.Attributes;
using Newtonsoft.Json;

namespace LuaDefinitionMaker;

public class NamespaceTypes : LuaType
{
    private readonly IEnumerable<Type> types;
    private readonly string namespaceName;
    private readonly List<BasicType> luaTypes = new();

    public NamespaceTypes(Type[] types, string namespaceName, string fileName, bool useJsonFallback = false)
        : base(typeof(object), namespaceName, new LuaDefinitionAttribute(fileName) { Hide = true }, useJsonFallback)
    {
        this.types = types.Where(t => t.Namespace == namespaceName && t.IsClass && !t.IsAbstract);
        this.namespaceName = namespaceName;
        loadTypes();
    }

    private void loadTypes()
    {
        foreach (var type in types)
        {
            var existingAttr = type.GetCustomAttribute<LuaDefinitionAttribute>(false);

            if (existingAttr != null)
            {
                var name = existingAttr.Name ?? type.Name.Replace("Lua", "");
                luaTypes.Add(new BasicType(type, name, existingAttr, UseJsonFallback, typeof(string)));
            }
            else if (UseJsonFallback && hasJsonAttributes(type))
            {
                var attr = new LuaDefinitionAttribute(Attribute.FileName)
                {
                    Name = type.Name.Replace("Lua", ""),
                    Public = true
                };
                luaTypes.Add(new BasicType(type, attr.Name, attr, UseJsonFallback, typeof(string)));
            }
            else if (!UseJsonFallback)
            {
                var attr = new LuaDefinitionAttribute(Attribute.FileName)
                {
                    Name = type.Name.Replace("Lua", ""),
                    Public = true
                };
                luaTypes.Add(new BasicType(type, attr.Name, attr, UseJsonFallback, typeof(string)));
            }
        }
    }

    private static bool hasJsonAttributes(Type type)
    {
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        
        foreach (var prop in properties)
        {
            if (prop.GetCustomAttribute<JsonPropertyAttribute>() != null ||
                prop.GetCustomAttribute<JsonIgnoreAttribute>() != null)
            {
                return true;
            }
        }

        return false;
    }

    public override void Write(StringBuilder sb)
    {
        Program.Write($"Namespace: {namespaceName} ({luaTypes.Count} types)");

        foreach (var type in luaTypes)
        {
            type.Write(sb);
        }
    }
}
