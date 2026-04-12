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

    // All conversion delegates to this
    private static object convertToLua(object value, Lua lua)
    {
        if (value == null) return null;

        var type = value.GetType();

        if (type.IsPrimitive || value is decimal || value is string)
            return value;

        if (value is IEnumerable enumerable)
            return enumerable.ToLuaTable(lua);

        return value.ToLuaTable(lua);
    }

    public static LuaTable ToLuaTable(this IEnumerable collection, Lua lua)
    {
        ArgumentNullException.ThrowIfNull(lua);

        var luaTable = lua.DoString("return {}")[0] as LuaTable;
        int index = 1;

        Debug.Assert(luaTable != null);

        foreach (var item in collection)
        {
            luaTable[index++] = convertToLua(item, lua);
        }

        return luaTable;
    }

    public static LuaTable ToLuaTable(this object obj, Lua lua)
    {
        ArgumentNullException.ThrowIfNull(lua);

        if (obj == null) return null;

        if (obj is IEnumerable enumerable and not string)
            return enumerable.ToLuaTable(lua);

        var type = obj.GetType();
        var typeCache = reflection_cache.GetOrAdd(type, generateTypeCache);

        var itemTable = lua.DoString("return {}")[0] as LuaTable;
        Debug.Assert(itemTable != null);

        foreach (var cachedProp in typeCache.Properties)
        {
            try
            {
                var value = cachedProp.Property.GetValue(obj);
                itemTable[cachedProp.LuaName] = convertToLua(value, lua);
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
                var luaValue = convertToLua(value, lua);

                itemTable[cachedField.LuaName] = luaValue;
                itemTable[cachedField.OriginalName] = luaValue;
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to get field {cachedField.Field.Name} for {type.Name}: {ex.Message}");
            }
        }

        foreach (var cachedMethod in typeCache.Methods)
        {
            try
            {
                string tempKey = $"__temp_func_{Guid.NewGuid():N}";
                lua.RegisterFunction(tempKey, obj, cachedMethod.Method);

                itemTable[cachedMethod.LuaName] = lua[tempKey];

                lua[tempKey] = null;
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to bind method {cachedMethod.Method.Name} for {type.Name}: {ex.Message}");
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
            BindingFlags.Static |
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

        var methods = type.GetMethods(
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.FlattenHierarchy
        );

        foreach (var method in methods)
        {
            if (method.IsSpecialName || method.DeclaringType == typeof(object)) continue;

            var attr = method.GetCustomAttribute<LuaMemberAttribute>(true);

            if (attr == null) continue;

            string luaName = !string.IsNullOrEmpty(attr.Name)
                ? attr.Name
                : (method.Name.IsUpperCase() ? method.Name.ToLower() : method.Name.Camelize());

            typeCache.Methods.Add(new LuaMethodsCache(method, luaName));
        }

        return typeCache;
    }

    private class LuaTypeCache
    {
        public List<LuaPropertiesCache> Properties { get; } = new();
        public List<LuaFieldsCache> Fields { get; } = new();
        public List<LuaMethodsCache> Methods { get; } = new();
    }

    private record struct LuaPropertiesCache(PropertyInfo Property, string LuaName);

    private record struct LuaFieldsCache(FieldInfo Field, string LuaName, string OriginalName);

    private record struct LuaMethodsCache(MethodInfo Method, string LuaName);
}
