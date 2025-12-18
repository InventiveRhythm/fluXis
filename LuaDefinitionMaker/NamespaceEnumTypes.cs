using System.Text;
using fluXis.Scripting.Attributes;

namespace LuaDefinitionMaker;

public class NamespaceEnumTypes : LuaType
{
    private readonly IEnumerable<Type> types;
    private readonly string[] namespaceNames;
    private readonly bool ctor;
    private readonly string ctorName;
    private readonly string? enumName;
    private readonly string? suffixTrim;

    public NamespaceEnumTypes(
        Type[] types, string[] namespaceNames, bool ctor = true,
        string? name = null, string file = "enums", string? ctorName = null, string? enumName = null, 
        bool useJsonFallback = false,string? suffixTrim = null)
        : base(typeof(object), name ?? namespaceNames.Last().Split('.').Last(), 
            new LuaDefinitionAttribute(file) { Name = name ?? namespaceNames.Last().Split('.').Last() }, 
            useJsonFallback)
    {
        this.namespaceNames = namespaceNames;
        this.types = types.Where(t => 
            namespaceNames.Contains(t.Namespace) && 
            t.IsClass && 
            !t.IsAbstract &&
            !t.IsNested);
        this.ctor = ctor;
        this.ctorName = ctorName ?? Name;
        this.enumName = enumName;
        this.suffixTrim = suffixTrim;
    }

    public override void Write(StringBuilder sb)
    {
        if (!types.Any())
            return;

        Program.Write($"{Attribute.FileName}: {string.Join(", ", namespaceNames)} ({types.Count()} classes)");

        var tName = enumName ?? $"{Name}{(ctor ? "Name" : "")}";

        sb.AppendLine($"---@alias {tName} string");

        foreach (var type in types)
        {
            var name = type.Name.Replace("Lua", "");
            
            if (!string.IsNullOrEmpty(suffixTrim) && name.EndsWith(suffixTrim))
            {
                name = name[..^suffixTrim.Length];
            }
            
            sb.AppendLine($"---| \"{name}\"");
        }

        sb.AppendLine();

        if (!ctor)
            return;

        sb.AppendLine($"---@param input {tName}");
        sb.AppendLine("---@return string");
        sb.AppendLine("---@nodiscard");
        sb.AppendLine($"function {ctorName}(input) end");
        sb.AppendLine();
    }
}
