using System.Text;
using fluXis.Scripting.Attributes;

namespace LuaDefinitionMaker;

public abstract class LuaType
{
    public Type BaseType { get; }
    public string Name { get; }
    public LuaDefinitionAttribute Attribute { get; }

    protected LuaType(Type baseType, string name, LuaDefinitionAttribute attribute)
    {
        BaseType = baseType;
        Name = name;
        Attribute = attribute;
    }

    public abstract void Write(StringBuilder sb);
}
