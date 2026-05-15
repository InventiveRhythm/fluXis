using System.Reflection;
using System.Text;
using fluXis.Scripting.Attributes;
using NLua;

namespace LuaDefinitionMaker;

public class NamespaceTypes : LuaType
{
    private readonly IEnumerable<Type> types;
    private readonly string[] namespaces;
    private readonly List<BasicType> luaTypes = new();
    private readonly string fileName;

    public NamespaceTypes(Type[] types, string[] namespaces, string fileName)
        : base(typeof(object), string.Join(", ", namespaces), new LuaDefinitionAttribute(fileName) { Hide = true })
    {
        this.fileName = fileName;
        // will define types for all classes and structs
        this.types = types.Where(t =>
            !t.IsAbstract &&
            namespaces.Contains(t.Namespace) &&
            (t.IsClass || t is { IsValueType: true, IsPrimitive: false, IsEnum: false })
        );
        this.namespaces = namespaces;
        loadTypes();
    }

    public NamespaceTypes(Type[] types, string namespaceName, string fileName)
        : this(types, new[] { namespaceName }, fileName)
    {
    }

    private void loadTypes()
    {
        foreach (var type in types)
        {
            var existingDefAttr = type.GetCustomAttribute<LuaDefinitionAttribute>(false);
            var existingMemberAttr = type.GetCustomAttribute<LuaMemberAttribute>(false);

            if (existingDefAttr != null)
            {
                var name = existingDefAttr.Name ?? type.Name.Replace("Lua", "");
                luaTypes.Add(new BasicType(type, name, existingDefAttr, typeof(string)));
            }
            else if (existingMemberAttr != null)
            {
                var name = existingMemberAttr.Name ?? type.Name.Replace("Lua", "");
                luaTypes.Add(new BasicType(type, name, new LuaDefinitionAttribute(fileName), typeof(string)));
            }
            else
            {
                var attr = new LuaDefinitionAttribute(Attribute.FileName)
                {
                    Name = type.Name.Replace("Lua", ""),
                    Public = true
                };

                luaTypes.Add(new BasicType(type, attr.Name, attr, typeof(string)));
            }
        }
    }

    public override void Write(StringBuilder sb)
    {
        Program.Write($"Namespaces: {string.Join(", ", namespaces)} ({luaTypes.Count} types)");

        foreach (var type in luaTypes)
        {
            type.Write(sb);
        }
    }
}
