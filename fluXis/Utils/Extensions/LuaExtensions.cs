using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Reflection;
using fluXis.Map.Structures;
using fluXis.Scripting;
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
    private static object convertToLua(object value, Lua lua, int version = 1)
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

            case HitObject o:
                return new LuaHitObject(o);
        }

        if (value is IEnumerable enumerable)
            return enumerable.ToLuaTable(lua, version);

        return value.ToLuaTable(lua, version);
    }

    public static LuaTable ToLuaTable(this IEnumerable collection, Lua lua, int version = 1)
    {
        ArgumentNullException.ThrowIfNull(lua);

        var luaTable = lua.DoString("return {}")[0] as LuaTable;
        int index = 1;

        Debug.Assert(luaTable != null);

        foreach (var item in collection)
        {
            luaTable[index++] = convertToLua(item, lua, version);
        }

        return luaTable;
    }

    public static LuaTable ToLuaTable(this object obj, Lua lua, int version = 1)
    {
        ArgumentNullException.ThrowIfNull(lua);

        if (obj == null) return null;

        if (obj is IEnumerable enumerable and not string)
            return enumerable.ToLuaTable(lua, version);

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

            foreach (var propGroup in typeCache.Properties)
            {
                var cachedProp = LuaVersionResolver.SelectBestMatch(propGroup.Value, version, $"{type.Name}.{propGroup.Key}");
                if (cachedProp == null) continue;

                try
                {
                    var value = cachedProp.Property.GetValue(obj);
                    itemTable[cachedProp.LuaName] = convertToLua(value, lua, version);
                }
                catch (Exception ex)
                {
                    Logger.Log($"Failed to get property {cachedProp.Property.Name} for {type.Name}: {ex.Message}");
                }
            }

            foreach (var fieldGroup in typeCache.Fields)
            {
                var cachedField = LuaVersionResolver.SelectBestMatch(fieldGroup.Value, version, $"{type.Name}.{fieldGroup.Key}");
                if (cachedField == null) continue;

                try
                {
                    var value = cachedField.Field.GetValue(obj);
                    var luaValue = convertToLua(value, lua, version);

                    itemTable[cachedField.LuaName] = luaValue;
                    itemTable[cachedField.OriginalName] = luaValue;
                }
                catch (Exception ex)
                {
                    Logger.Log($"Failed to get field {cachedField.Field.Name} for {type.Name}: {ex.Message}");
                }
            }

            foreach (var methodGroup in typeCache.Methods)
            {
                var cachedMethod = LuaVersionResolver.SelectBestMatch(methodGroup.Value, version, $"{type.Name}.{methodGroup.Key}");
                if (cachedMethod == null) continue;

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
            if (prop.GetCustomAttribute<JsonIgnoreAttribute>(true) != null
                || prop.GetCustomAttribute<NotMappedAttribute>(true) != null)
                continue;

            var attr = prop.GetCustomAttribute<LuaMemberAttribute>(true);
            string luaName;

            if (attr != null && !string.IsNullOrEmpty(attr.Name))
                luaName = attr.Name;
            else
                luaName = prop.Name.IsUpperCase() ? prop.Name.ToLower() : prop.Name.Camelize();

            if (!typeCache.Properties.TryGetValue(luaName, out var list))
                typeCache.Properties[luaName] = list = new();

            var ver = prop.GetCustomAttribute<LuaVersionAttribute>(true);
            list.Add(new LuaPropertiesCache(prop, luaName, ver));
        }

        var fields = type.GetFields(
            BindingFlags.Public |
            BindingFlags.Instance
        );

        foreach (var field in fields)
        {
            if (field.GetCustomAttribute<JsonIgnoreAttribute>(true) != null
                || field.GetCustomAttribute<NotMappedAttribute>(true) != null)
                continue;

            string luaName = char.ToLowerInvariant(field.Name[0]) + field.Name[1..];

            if (!typeCache.Fields.TryGetValue(luaName, out var list))
                typeCache.Fields[luaName] = list = new();

            var ver = field.GetCustomAttribute<LuaVersionAttribute>(true);
            list.Add(new LuaFieldsCache(field, luaName, field.Name, ver));
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

            if (!typeCache.Methods.TryGetValue(luaName, out var list))
                typeCache.Methods[luaName] = list = new();

            var ver = method.GetCustomAttribute<LuaVersionAttribute>(true);
            list.Add(new LuaMethodsCache(method, luaName, ver));
        }

        return typeCache;
    }

    private class LuaTypeCache
    {
        public Dictionary<string, List<LuaPropertiesCache>> Properties { get; } = new();
        public Dictionary<string, List<LuaFieldsCache>> Fields { get; } = new();
        public Dictionary<string, List<LuaMethodsCache>> Methods { get; } = new();
    }

    private record LuaPropertiesCache(PropertyInfo Property, string LuaName, LuaVersionAttribute Version) : IVersionedCandidate;

    private record LuaFieldsCache(FieldInfo Field, string LuaName, string OriginalName, LuaVersionAttribute Version) : IVersionedCandidate;

    private record LuaMethodsCache(MethodInfo Method, string LuaName, LuaVersionAttribute Version) : IVersionedCandidate;
}
