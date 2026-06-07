using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using fluXis.Scripting.Models;
using Humanizer;
using Midori.Utils.Extensions;
using Newtonsoft.Json;
using NLua;
using osu.Framework.Logging;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Utils.Extensions;

public static class LuaExtensions
{
    private static readonly ConcurrentDictionary<Type, LuaTypeCache> reflection_cache = new();

    private const int max_depth = 32;

    [ThreadStatic]
    private static HashSet<object> visitedObjects;

    [ThreadStatic]
    private static int currentDepth;

    // All conversion delegates to this
    private static object convertToLua(object value, Lua lua)
    {
        if (value == null) return null;

        var type = value.GetType();

        if (type.IsPrimitive || value is decimal || value is string)
            return value;

        // Explicit conversion
        // do this for types we don't have control over
        // or types that are better for use as wrappers in lua rather than relying on auto table generation
        switch (value)
        {
            case Vector2 v:
                return new LuaVector2(v);

            case Color4 c:
                return new LuaColor4(c);
        }

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

        currentDepth++;
        Debug.Assert(currentDepth <= max_depth,
            $"Max recursion depth ({max_depth}) hit for {obj.GetType().FullName}");

        bool isValueType = obj.GetType().IsValueType;

        // ValueTypes (structs) can't form an infinite loop because they are copied/cloned instead of referenced unless you use `ref` excplicitly
        if (!isValueType)
        {
            visitedObjects ??= new HashSet<object>(ReferenceEqualityComparer.Instance);
            bool alreadyPresent = !visitedObjects.Add(obj);
            Debug.Assert(!alreadyPresent,
                $"Circular reference detected for type {obj.GetType().FullName} at depth {currentDepth}");
        }

        try
        {
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
        finally
        {
            if (!isValueType)
                visitedObjects?.Remove(obj);

            currentDepth--;

            if (currentDepth == 0)
                visitedObjects = null;
        }
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

            // if we can't get it through json then it's a good idea not to be able to get it through lua also
            // this also saves us from stack overflows as some auto generated fields may have references to the same type
            if (prop.GetCustomAttribute<JsonIgnoreAttribute>(true) != null)
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
            if (field.GetCustomAttribute<JsonIgnoreAttribute>(true) != null)
                continue;

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
