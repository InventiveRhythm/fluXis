using System.Text;
using fluXis.Scripting.Attributes;

namespace LuaDefinitionMaker;

public class CustomTextType : LuaType
{
    private readonly string text;

    public CustomTextType(string file, string text)
        : base(null!, "", new LuaDefinitionAttribute(file))
    {
        this.text = text;
    }

    public override void Write(StringBuilder sb)
    {
        sb.Append(text);
        sb.AppendLine();
    }
}
