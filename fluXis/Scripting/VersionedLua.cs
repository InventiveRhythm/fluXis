using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using NLua;

namespace fluXis.Scripting;

public class VersionedLua
{
    private readonly Lua lua;
    public required int Version;

    private readonly Dictionary<string, List<FunctionCandidate>> functions = new();
    private readonly Dictionary<string, ModelEntry> models = new();
    private readonly HashSet<string> registered = new();

    private readonly LuaFunction skipSelf;

    public VersionedLua(Lua lua)
    {
        this.lua = lua;

        // Methods in the table are bound to C# statically and have no 'self' parameter,
        // so this wraps them to make the ':' syntax work. '.' calls work too without 'self'.
        skipSelf = (LuaFunction)lua.DoString(@"
        return function(t, f)
            return function(self, ...)
                if self == t then return f(...) end
                return f(self, ...)
            end
        end")[0];
    }

    public void AddFunction(string name, params List<Delegate> candidates)
    {
        getList(functions, name).AddRange
        (
            candidates.Select(d => new FunctionCandidate
            (
                d,
                d.Method.GetCustomAttribute<LuaVersionAttribute>()
            ))
        );
    }

    public void AddField(string name, ILuaModel model)
    {
        var members = model.GetType()
                           .GetMembers(BindingFlags.Public | BindingFlags.Instance)
                           .Where(m => m is MethodInfo or PropertyInfo or FieldInfo)
                           .Select(m => (Member: m, Attr: m.GetCustomAttribute<LuaMemberAttribute>()))
                           .Where(x => x.Attr is not null)
                           .Select(x => new MemberCandidate(
                               x.Member,
                               x.Attr!.Name ?? x.Member.Name,
                               x.Member.GetCustomAttribute<LuaVersionAttribute>()))
                           .ToList();

        models[name] = new ModelEntry(model, members);
    }

    public void Resolve()
    {
        // Find the best matches for the version
        var resolvedFns = functions
                          .Select(p => getResolvedFunction(p.Key, p.Value))
                          .Where(p => p is not null)
                          .ToList();

        var resolvedModels = models
                             .Select(p => getResolvedModel(p.Key, p.Value))
                             .ToList();

        // Clear the previous registry for the new candidates
        foreach (var name in registered) lua[name] = null;
        registered.Clear();

        // Register functions
        foreach (var (name, fn) in resolvedFns)
        {
            lua.RegisterFunction(name, fn.Target, fn.Method);
            registered.Add(name);
        }

        // Register models
        foreach (var (name, model, isVersioned, members) in resolvedModels)
        {
            registered.Add(name);

            if (!isVersioned)
            {
                lua[name] = model;
                continue;
            }

            lua.NewTable(name);
            foreach (var m in members) registerMember(name, model, m);
        }
    }

    #region Resolution

    [CanBeNull]
    private ResolvedFunction getResolvedFunction(string name, List<FunctionCandidate> candidates)
    {
        var best = LuaVersionResolver.SelectBestMatch(candidates, Version, name);
        return best is null ? null : new ResolvedFunction(name, best.Fn);
    }

    private ResolvedModel getResolvedModel(string name, ModelEntry entry)
    {
        var bestMembers = entry.Members
                               .GroupBy(m => m.LuaName)
                               .Select(g => LuaVersionResolver.SelectBestMatch(g, Version, $"{name}.{g.Key}"))
                               .Where(m => m is not null)
                               .ToList();

        var isVersioned = entry.Members.Any(m => m.Version is not null);

        return new ResolvedModel(name, entry.Model, isVersioned, bestMembers);
    }

    private void registerMember(string modelName, ILuaModel model, MemberCandidate member)
    {
        var path = $"{modelName}.{member.LuaName}";

        switch (member.Member)
        {
            case MethodInfo mi:
            {
                var raw = lua.RegisterFunction(path, model, mi);
                lua[path] = skipSelf.Call(lua.GetTable(modelName), raw)[0];
                break;
            }

            case PropertyInfo pi: lua[path] = pi.GetValue(model); break;

            case FieldInfo fi: lua[path] = fi.GetValue(model); break;
        }
    }

    #endregion

    private static List<T> getList<T>(Dictionary<string, List<T>> d, string key)
    {
        if (!d.TryGetValue(key, out var list)) d[key] = list = new();
        return list;
    }

    #region Records

    private record FunctionCandidate(Delegate Fn, [CanBeNull] LuaVersionAttribute Version) : IVersionedCandidate;

    private record MemberCandidate(MemberInfo Member, string LuaName, [CanBeNull] LuaVersionAttribute Version) : IVersionedCandidate;

    private record ModelEntry(ILuaModel Model, List<MemberCandidate> Members);

    private record ResolvedFunction(string Name, Delegate Fn);

    private record ResolvedModel(string Name, ILuaModel Model, bool IsVersioned, IReadOnlyList<MemberCandidate> Members);

    #endregion
}
