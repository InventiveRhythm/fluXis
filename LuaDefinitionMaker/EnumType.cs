using System.Text;
using fluXis.Scripting.Attributes;

namespace LuaDefinitionMaker;

public class EnumType<T> : LuaType
    where T : struct, Enum
{
    public T[]? Values { get; init; }
    private readonly bool ctor;

    public EnumType(bool ctor, string? name = null, string file = "enums")
        : base(typeof(T), name ?? typeof(T).Name, new LuaDefinitionAttribute(file) { Name = name ?? typeof(T).Name })
    {
        this.ctor = ctor;
    }

    public override void Write(StringBuilder sb)
    {
        Program.Write($"{Attribute.FileName}: {BaseType.FullName}");

        var tName = $"{Name}{(ctor ? "Name" : "")}";

        sb.AppendLine($"---@alias {tName} string");

        foreach (var name in (Values?.Select(x => x.ToString()) ?? Enum.GetNames<T>()))
            sb.AppendLine($"---| \"{name}\"");

        sb.AppendLine();

        if (!ctor)
            return;

        sb.AppendLine($"---@param input {tName}");
        sb.AppendLine("---@return number");
        sb.AppendLine("---@nodiscard");
        sb.AppendLine($"function {Name}(input) end");
        sb.AppendLine();
    }
}
