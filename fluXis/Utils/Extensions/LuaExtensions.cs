using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Humanizer;
using NLua;
using osu.Framework.Logging;

namespace fluXis.Utils.Extensions;

public static class LuaExtensions
{
    private static readonly ConcurrentDictionary<Type, LuaTypeCache> reflection_cache = new();

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

        var type = obj.GetType();

        var typeCache = reflection_cache.GetOrAdd(type, generateTypeCache);

        var itemTable = lua.DoString("return {}")[0] as LuaTable;
        Debug.Assert(itemTable != null);

        foreach (var cachedProp in typeCache.Properties)
        {
            try
            {
                var value = cachedProp.Property.GetValue(obj);
                itemTable[cachedProp.LuaName] = value;
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to get property {cachedProp.Property.Name} for {type.Name}: {ex.Message}");
            }
        }

        foreach (var cachedField in typeCache.Fields)
        {
            try
            {
                var value = cachedField.Field.GetValue(obj);
                itemTable[cachedField.LuaName] = value;
                itemTable[cachedField.OriginalName] = value;
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to get field {cachedField.Field.Name} for {type.Name}: {ex.Message}");
            }
        }

        return itemTable;
    }

    private static LuaTypeCache generateTypeCache(Type type)
    {
        var typeCache = new LuaTypeCache();

        var properties = type.GetProperties(
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.FlattenHierarchy
        );

        foreach (var prop in properties)
        {
            if (prop.GetIndexParameters().Length > 0)
                continue;

            var attr = prop.GetCustomAttribute<LuaMemberAttribute>(true);
            string luaName;

            if (attr != null)
                luaName = attr.Name;
            else
                luaName = prop.Name.IsUpperCase() ? prop.Name.ToLower() : prop.Name.Camelize();

            typeCache.Properties.Add(new LuaPropertiesCache(prop, luaName));
        }

        var fields = type.GetFields(
            BindingFlags.Public |
            BindingFlags.Instance
        );

        foreach (var field in fields)
        {
            string luaName = char.ToLowerInvariant(field.Name[0]) + field.Name[1..];
            typeCache.Fields.Add(new LuaFieldsCache(field, luaName, field.Name));
        }

        return typeCache;
    }

    private class LuaTypeCache
    {
        public List<LuaPropertiesCache> Properties { get; } = new();
        public List<LuaFieldsCache> Fields { get; } = new();
    }

    private record struct LuaPropertiesCache(PropertyInfo Property, string LuaName);
    private record struct LuaFieldsCache(FieldInfo Field, string LuaName, string OriginalName);
}
