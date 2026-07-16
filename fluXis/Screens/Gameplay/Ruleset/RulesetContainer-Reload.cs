using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Map.Structures.Bases;

namespace fluXis.Screens.Gameplay.Ruleset;

public partial class RulesetContainer
{
    private Dictionary<Type, IReloadHandler> handlers { get; } = new();

    public bool HasReloadListener(Type t) => handlers.ContainsKey(t);

    public void RegisterReload<T>(Action<List<T>> callback) where T : ITimedObject
    {
        if (!handlers.TryGetValue(typeof(T), out var h))
            handlers[typeof(T)] = h = new ReloadHandler<T>();

        var handler = (ReloadHandler<T>)h;
        handler.OnReload += callback;
    }

    public bool TriggerReload<T>(List<T> objects) where T : ITimedObject
        => TriggerReload(typeof(T), objects.Cast<object>().ToList());

    public bool TriggerReload(Type type, List<object> objects)
    {
        if (!handlers.TryGetValue(type, out var h))
            return false;

        h.Reload(objects);
        return true;
    }

    private class ReloadHandler<T> : IReloadHandler where T : ITimedObject
    {
        public event Action<List<T>> OnReload;

        public void Reload(List<object> objs) => OnReload?.Invoke(objs.Cast<T>().ToList());
    }

    private interface IReloadHandler
    {
        void Reload(List<object> objs);
    }
}
