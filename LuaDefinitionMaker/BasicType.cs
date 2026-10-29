using System.Reflection;
using System.Text;
using fluXis.Scripting;
using fluXis.Scripting.Attributes;
using fluXis.Utils;
using Humanizer;
using Midori.Utils.Extensions;
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

    private static void writeDeprecated(StringBuilder sb, MemberInfo member, bool withMessage = true)
    {
        var obs = member.GetCustomAttribute<ObsoleteAttribute>(false);
        if (obs is null) return;

        if (withMessage && !string.IsNullOrWhiteSpace(obs.Message))
            sb.AppendLine($"---{obs.Message.ReplaceLineEndings(" ")}");

        sb.AppendLine("---@deprecated");
    }

    private static string? getObsoleteMessage(MemberInfo member)
    {
        var obs = member.GetCustomAttribute<ObsoleteAttribute>(false);
        return string.IsNullOrWhiteSpace(obs?.Message) ? null : obs.Message.ReplaceLineEndings(" ");
    }

    private static IEnumerable<(T Member, string Name)> getNewestMember<T>(IEnumerable<(T Member, string Name)> items)
        where T : MemberInfo
        => items.GroupBy(x => x.Name)
                .Select(g =>
                    g.OrderByDescending(x => x.Member.GetCustomAttribute<LuaVersionAttribute>()?.Since ?? 0)
                     .First()
                );

    public override void Write(StringBuilder sb)
    {
        Program.Write($"{Attribute.FileName}: {BaseType.FullName}");

        if (!Attribute.Hide)
        {
            writeDeprecated(sb, BaseType);
            sb.Append($"---@class {Name}");

            if (BaseType.BaseType != null)
            {
                var b = BaseType.BaseType;

                if (b != typeof(ILuaModel) && b != typeof(object) && b != typeof(ValueType)) // exclude ValueType cuz of structs
                    sb.Append($": {Program.GetLuaType(b, fallback: fallbackType)}");
            }

            sb.AppendLine();

            var propFields = new List<(PropertyInfo Member, string Name)>();

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

                propFields.Add((prop, StringUtils.ToCamelCase(fieldName)));
            }

            foreach (var (prop, fieldName) in getNewestMember(propFields))
            {
                writeDeprecated(sb, prop, withMessage: false);
                sb.Append($"---@field {fieldName} {Program.GetLuaType(prop.PropertyType, fallback: fallbackType)}");

                var sum = Documentation.GetPropertySummary(prop);
                if (sum is not null) sb.Append($" {sum.ReplaceLineEndings(" ")}");

                var obsMsg = getObsoleteMessage(prop);
                if (obsMsg is not null) sb.Append($" (deprecated: {obsMsg})");

                sb.AppendLine();
            }

            var fieldMembers = BaseType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                                       .Select(f => (Member: f, Attr: f.GetCustomAttribute<LuaMemberAttribute>()))
                                       .Where(x => x.Attr is not null)
                                       .Select(x => (x.Member, x.Attr!.Name))
                                       .ToList();

            foreach (var (field, name) in getNewestMember(fieldMembers))
            {
                writeDeprecated(sb, field, withMessage: false);
                sb.Append($"---@field {name} {Program.GetLuaType(field.FieldType, fallback: fallbackType)}");
                sb.AppendLine();
            }

            if (Attribute.Public)
                sb.AppendLine($"{Name} = {{}}");
            else
                sb.AppendLine($"local __{Name} = {{}}");

            sb.AppendLine();
        }

        var instanceGlobals = BaseType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                                      .Select(p => (Member: p, Attr: p.GetCustomAttribute<LuaGlobalAttribute>()))
                                      .Where(x => x.Attr is not null)
                                      .Select(x => (x.Member, Name: x.Attr!.Name ?? x.Member.Name))
                                      .ToList();

        foreach (var (property, globalName) in getNewestMember(instanceGlobals))
        {
            writeDeprecated(sb, property);
            sb.AppendLine($"---@type {Program.GetLuaType(property.PropertyType, fallback: fallbackType)}");
            sb.AppendLine($"---@diagnostic disable-next-line: missing-fields");
            sb.AppendLine($"{globalName} = {{}}");
            sb.AppendLine();
        }

        // handle static (global) properties separately
        var staticGlobals = BaseType.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
                                    .Select(p => (Member: p, Attr: p.GetCustomAttribute<LuaGlobalAttribute>()))
                                    .Where(x => x.Attr is not null)
                                    .Select(x => (x.Member, Name: x.Attr!.Name ?? x.Member.Name))
                                    .ToList();

        foreach (var (prop, fieldName) in getNewestMember(staticGlobals))
        {
            var sum = Documentation.GetPropertySummary(prop);
            if (sum is not null) sb.AppendLine($"---{sum.ReplaceLineEndings(" ")}");

            writeDeprecated(sb, prop);
            sb.AppendLine($"---@type {Program.GetLuaType(prop.PropertyType, fallback: fallbackType)}");
            sb.AppendLine($"---@diagnostic disable-next-line: missing-fields");
            sb.AppendLine($"{Name}.{fieldName} = {{}}");
            sb.AppendLine();
        }

        foreach (var ctor in BaseType.GetConstructors())
        {
            var ctorAttr = ctor.GetCustomAttribute<LuaConstructorAttribute>();
            if (ctorAttr is null) continue;

            writeDeprecated(sb, ctor);

            foreach (var parameter in ctor.GetParameters())
            {
                var pType = parameter.GetCustomAttribute<LuaCustomType>()?.Target ?? parameter.ParameterType;
                sb.AppendLine($"---@param {parameter.Name} {Program.GetLuaType(pType, fallback: fallbackType)}");
            }

            sb.AppendLine($"---@return {Name}");
            sb.AppendLine($"function {Name}({string.Join(", ", ctor.GetParameters().Select(x => x.Name))}) end");
            sb.AppendLine();
        }

        var methods = BaseType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                              .Select(m => (Method: m, Member: m.GetCustomAttribute<LuaMemberAttribute>(), Global: m.GetCustomAttribute<LuaGlobalAttribute>()))
                              .Where(x => x.Member is not null || x.Global is not null)
                              .Select(x => (x.Method, Name: x.Global is not null
                                  ? x.Global.Name ?? x.Method.Name
                                  : $"{(Attribute.Public ? "" : "__")}{Name}:{x.Member!.Name}"))
                              .ToList();

        foreach (var (method, funcName) in getNewestMember(methods))
        {
            var doc = Documentation.GetMethod(method);

            if (doc.Summary is not null)
            {
                var lines = doc.Summary.Split('\n');
                lines = lines.Select(x => x.Trim()).ToArray();
                foreach (var line in lines) sb.AppendLine($"---{line}");
            }

            writeDeprecated(sb, method);

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

            sb.AppendLine($"function {funcName}({string.Join(", ", method.GetParameters().Select(x => x.Name))}) end");
            sb.AppendLine();
        }
    }
}
