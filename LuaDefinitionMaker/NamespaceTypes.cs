using System.Reflection;
using System.Text;
using fluXis.Scripting.Attributes;

namespace LuaDefinitionMaker;

public class NamespaceTypes : LuaType
{
    private readonly IEnumerable<Type> types;
    private readonly string namespaceName;
    private readonly List<BasicType> luaTypes = new();

    public NamespaceTypes(Type[] types, string namespaceName, string fileName)
        : base(typeof(object), namespaceName, new LuaDefinitionAttribute(fileName) { Hide = true })
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
        Program.Write($"Namespace: {namespaceName} ({luaTypes.Count} types)");

        foreach (var type in luaTypes)
        {
            type.Write(sb);
        }
    }
}
