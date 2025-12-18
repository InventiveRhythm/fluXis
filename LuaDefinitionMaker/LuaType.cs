using System.Text;
using fluXis.Scripting.Attributes;

namespace LuaDefinitionMaker;

public abstract class LuaType
{
    public Type BaseType { get; }
    public string Name { get; }
    public LuaDefinitionAttribute Attribute { get; }
    public bool UseJsonFallback { get; }

    protected LuaType(Type baseType, string name, LuaDefinitionAttribute attribute, bool useJsonFallback = false)
    {
        BaseType = baseType;
        Name = name;
        Attribute = attribute;
        UseJsonFallback = useJsonFallback;
    }

    public abstract void Write(StringBuilder sb);
}
