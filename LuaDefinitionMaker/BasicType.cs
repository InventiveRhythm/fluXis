using System.Reflection;
using System.Text;
using fluXis.Scripting;
using fluXis.Scripting.Attributes;
using NLua;

namespace LuaDefinitionMaker;

public class BasicType : LuaType
{
    public BasicType(Type baseType, string name, LuaDefinitionAttribute attribute)
        : base(baseType, name, attribute)
    {
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
                    sb.Append($": {Program.GetLuaType(b)}");
            }

            sb.AppendLine();

            foreach (var prop in BaseType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                var memberAttr = prop.GetCustomAttribute<LuaMemberAttribute>();
                if (memberAttr is null) continue;

                sb.AppendLine($"---@field {memberAttr.Name} {Program.GetLuaType(prop.PropertyType)}");
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

            sb.AppendLine($"---@type {Program.GetLuaType(property.PropertyType)}");
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
                sb.AppendLine($"---@param {parameter.Name} {Program.GetLuaType(pType)}");
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

            foreach (var parameter in method.GetParameters())
            {
                var pType = parameter.GetCustomAttribute<LuaCustomType>()?.Target ?? parameter.ParameterType;
                var lua = Program.GetLuaType(pType, false);
                sb.AppendLine($"---@param {parameter.Name} {lua}");
            }

            var ret = method.ReturnType;

            if (ret != typeof(void))
            {
                sb.AppendLine($"---@return {Program.GetLuaType(method.ReturnType)}");
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
