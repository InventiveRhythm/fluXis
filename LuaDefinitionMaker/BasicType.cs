using System.Reflection;
using System.Text;
using fluXis.Scripting;
using fluXis.Scripting.Attributes;
using fluXis.Utils;
using Humanizer;
using Newtonsoft.Json;
using NLua;

namespace LuaDefinitionMaker;

public class BasicType : LuaType
{
    private readonly Type? fallbackType;

    public BasicType(Type baseType, string name, LuaDefinitionAttribute attribute, Type? fallbackType = null)
        : base(baseType, name, attribute)
    {
        this.fallbackType = fallbackType;
    }

    public override void Write(StringBuilder sb)
    {
        Program.Write($"{Attribute.FileName}: {BaseType.FullName}");

        if (!Attribute.Hide)
        {
            sb.Append($"---@class {Name}");

            if (BaseType.BaseType != null)
            {
                var b = BaseType.BaseType;

                if (b != typeof(ILuaModel) && b != typeof(object))
                    sb.Append($": {Program.GetLuaType(b, fallback: fallbackType)}");
            }

            sb.AppendLine();

            foreach (var prop in BaseType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                var memberAttr = prop.GetCustomAttribute<LuaMemberAttribute>();

                string? fieldName = null;

                if (memberAttr is not null)
                    fieldName = memberAttr.Name;

                if (prop.GetCustomAttribute<JsonPropertyAttribute>() != null)
                    fieldName = prop.Name.IsUpperCase() ? prop.Name.ToLower() : prop.Name.Camelize();

                if (fieldName is null)
                    continue;

                sb.Append($"---@field {StringUtils.ToCamelCase(fieldName)} {Program.GetLuaType(prop.PropertyType, fallback: fallbackType)}");

                var sum = Documentation.GetPropertySummary(prop);
                if (sum is not null) sb.Append($" {sum.ReplaceLineEndings(" ")}");

                sb.AppendLine();
            }

            if (Attribute.Public)
                sb.AppendLine($"{Name} = {{}}");
            else
                sb.AppendLine($"local __{Name} = {{}}");

            sb.AppendLine();
        }

        foreach (var property in BaseType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            var globalAttr = property.GetCustomAttribute<LuaGlobalAttribute>();
            if (globalAttr is null) continue;

            sb.AppendLine($"---@type {Program.GetLuaType(property.PropertyType, fallback: fallbackType)}");
            sb.AppendLine($"---@diagnostic disable-next-line: missing-fields");
            sb.AppendLine($"{globalAttr.Name ?? property.Name} = {{}}");
            sb.AppendLine();
        }

        foreach (var ctor in BaseType.GetConstructors())
        {
            var ctorAttr = ctor.GetCustomAttribute<LuaConstructorAttribute>();
            if (ctorAttr is null) continue;

            foreach (var parameter in ctor.GetParameters())
            {
                var pType = parameter.GetCustomAttribute<LuaCustomType>()?.Target ?? parameter.ParameterType;
                sb.AppendLine($"---@param {parameter.Name} {Program.GetLuaType(pType, fallback: fallbackType)}");
            }

            sb.AppendLine($"---@return {Name}");
            sb.AppendLine($"function {Name}({string.Join(", ", ctor.GetParameters().Select(x => x.Name))}) end");
            sb.AppendLine();
        }

        foreach (var method in BaseType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            var memberAttr = method.GetCustomAttribute<LuaMemberAttribute>();
            var globalAttr = method.GetCustomAttribute<LuaGlobalAttribute>();
            if (memberAttr is null && globalAttr is null) continue;

            var doc = Documentation.GetMethod(method);

            if (doc.Summary is not null)
            {
                var lines = doc.Summary.Split('\n');
                lines = lines.Select(x => x.Trim()).ToArray();
                foreach (var line in lines) sb.AppendLine($"---{line}");
            }

            foreach (var parameter in method.GetParameters())
            {
                var pType = parameter.GetCustomAttribute<LuaCustomType>()?.Target ?? parameter.ParameterType;
                bool isNullable = parameter.HasDefaultValue && parameter.DefaultValue == null ||
                          Nullable.GetUnderlyingType(pType) != null;

                var luaType = Program.GetLuaType(pType, false, typeof(string), isNullable);
                
                sb.Append($"---@param {parameter.Name} {luaType}");

                var desc = doc.GetParameterDescription(parameter.Name!);
                if (desc is not null) sb.Append($" {desc.ReplaceLineEndings(" ")}");

                sb.AppendLine();
            }

            var ret = method.ReturnType;

            if (ret != typeof(void))
            {
                var r = $"---@return {Program.GetLuaType(method.ReturnType, fallback: fallbackType)}";

                if (doc.Returns is not null)
                    r += $" # {doc.Returns.ReplaceLineEndings(" ")}";

                sb.AppendLine(r);
                sb.AppendLine($"---@nodiscard");
            }

            sb.Append("function ");

            if (globalAttr is not null)
                sb.Append(globalAttr.Name ?? method.Name);
            else if (memberAttr is not null)
                sb.Append($"{(Attribute.Public ? "" : "__")}{Name}:{memberAttr.Name}");

            sb.AppendLine($"({string.Join(", ", method.GetParameters().Select(x => x.Name))}) end");
            sb.AppendLine();
        }
    }
}
