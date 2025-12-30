using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using Humanizer;
using NLua;

namespace fluXis.Utils.Extensions;

public static class LuaExtensions
{
    public static LuaTable ToLuaTable(this IEnumerable collection, Lua lua)
    {
        ArgumentNullException.ThrowIfNull(lua);

        var luaTable = lua.DoString("return {}")[0] as LuaTable;
        int index = 1;

        Debug.Assert(luaTable != null);

        foreach (var item in collection)
        {
            if (item == null)
            {
                luaTable[index++] = null;
                continue;
            }

            var itemTable = item.ToLuaTable(lua);
            luaTable[index++] = itemTable;
        }

        return luaTable;
    }

    public static LuaTable ToLuaTable(this object obj, Lua lua)
    {
        ArgumentNullException.ThrowIfNull(lua);

        if (obj == null)
            return null;

        var itemTable = lua.DoString("return {}")[0] as LuaTable;
        var type = obj.GetType();

        Debug.Assert(itemTable != null);

        var properties = type.GetProperties(
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.FlattenHierarchy
        );

        foreach (var prop in properties)
        {
            try
            {
                if (prop.GetIndexParameters().Length > 0)
                    continue;

                var value = prop.GetValue(obj);
                var attr = prop.GetCustomAttribute<LuaMemberAttribute>(true);

                string luaName;

                if (attr != null)
                    luaName = attr.Name;
                else
                    luaName = prop.Name.IsUpperCase() ? prop.Name.ToLower() : prop.Name.Camelize();

                itemTable[luaName] = value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get property {prop.Name} for {type.Name}: {ex.Message}");
            }
        }

        var fields = type.GetFields(
            BindingFlags.Public |
            BindingFlags.Instance
        );

        foreach (var field in fields)
        {
            try
            {
                var value = field.GetValue(obj);
                string luaName = char.ToLowerInvariant(field.Name[0]) + field.Name[1..];
                itemTable[luaName] = value;
                itemTable[field.Name] = value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get field {field.Name} for {type.Name}: {ex.Message}");
            }
        }

        return itemTable;
    }
}
