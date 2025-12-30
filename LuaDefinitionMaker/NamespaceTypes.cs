using System.Reflection;
using System.Text;
using fluXis.Scripting.Attributes;

namespace LuaDefinitionMaker;

public class NamespaceTypes : LuaType
{
    private readonly IEnumerable<Type> types;
    private readonly string[] namespaces;
    private readonly List<BasicType> luaTypes = new();

    public NamespaceTypes(Type[] types, string[] namespaces, string fileName)
        : base(typeof(object), string.Join(", ", namespaces), new LuaDefinitionAttribute(fileName) { Hide = true })
    {
        this.types = types.Where(t => t.IsClass && !t.IsAbstract && namespaces.Contains(t.Namespace));
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
            var existingAttr = type.GetCustomAttribute<LuaDefinitionAttribute>(false);

            if (existingAttr != null)
            {
                var name = existingAttr.Name ?? type.Name.Replace("Lua", "");
                luaTypes.Add(new BasicType(type, name, existingAttr, typeof(string)));
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
