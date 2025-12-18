using System;
using System.Collections.Generic;
using NLua;

namespace fluXis.Utils.Extensions;

public static class LuaExtensions
{
    public static LuaTable ToLuaTable<T>(this IEnumerable<T> collection, Lua lua)
    {
        ArgumentNullException.ThrowIfNull(lua);

        var luaTable = lua.DoString("return {}")[0] as LuaTable;
        int index = 1;
        
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
        
        var properties = type.GetProperties(
            System.Reflection.BindingFlags.Public | 
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.FlattenHierarchy
        );
        
        foreach (var prop in properties)
        {
            try
            {
                if (prop.GetIndexParameters().Length > 0)
                    continue;
                    
                var value = prop.GetValue(obj);
                
                string luaName = char.ToLowerInvariant(prop.Name[0]) + prop.Name[1..];
                
                itemTable[luaName] = value;
                itemTable[prop.Name] = value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get property {prop.Name} for {type.Name}: {ex.Message}");
            }
        }
        
        var fields = type.GetFields(
            System.Reflection.BindingFlags.Public | 
            System.Reflection.BindingFlags.Instance
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