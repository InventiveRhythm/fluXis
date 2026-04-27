using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Graphics;
using fluXis.Map;
using fluXis.Screens.Edit;
using fluXis.Scripting.Runners;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Logging;

namespace fluXis.Storyboards.Drawables;

public partial class DrawableDynamicStoryboard : DrawableStoryboard
{
    [Resolved]
    private EditorClock clock { get; set; }

    /// <summary>
    /// (time, endtime, isLoaded)
    /// </summary>
    public BindableList<(float, float, bool)> LoadedRanges { get; } = new();

    /// <summary>
    /// ALWAYS use this instead of <see cref="DrawableStoryboard.Storyboard"/> for dynamic loading!
    /// If you don't you'll encounter desync issues and element duplication.
    /// </summary>
    public Bindable<Storyboard> BindableStoryboard { get; }

    public Action<StoryboardLayer, StoryboardElement, IEnumerable<StoryboardElement>> ScriptElementsAdded;
    public Action<StoryboardLayer, StoryboardElement> ScriptElementsRemoved;
    public Action<StoryboardLayer> Updated;

    private Dictionary<string, StoryboardScriptRunner> scriptsLookup { get; } = new();
    private Queue<Func<Task>> loadQueue { get; } = new();
    private HashSet<ScriptState> pendingStates { get; } = new();
    private List<ScriptState> scripts { get; } = new();

    public DrawableDynamicStoryboard(Bindable<MapInfo> map, Bindable<Storyboard> storyboard, string assetPath)
        : base(map.Value, storyboard.Value.JsonCopy(), assetPath)
    {
        BindableStoryboard = storyboard;
    }

    public void RebuildElements(IEnumerable<StoryboardElement> elements)
    {
        foreach (var element in elements.Where(e => e.Type == StoryboardElementType.Script))
        {
            var state = scripts.FirstOrDefault(s => s.SourceElement == element);

            if (state == null)
            {
                state = new ScriptState
                {
                    SourceElement = element,
                    Loaded = false
                };

                state.OnRebuildStart = () =>
                {
                    state.Loaded = false;
                    syncLoadedRanges();
                };

                state.OnRebuildFinish = () =>
                {
                    state.Loaded = true;
                    syncLoadedRanges();

                    foreach (var l in state.Elements.Select(e => e.Layer).Distinct())
                        Updated?.Invoke(l);
                };

                scripts.Add(state);
            }

            if (state.Loaded)
            {
                foreach (var l in state.Elements.Select(e => e.Layer).Distinct())
                    ScriptElementsRemoved?.Invoke(l, state.SourceElement);
            }

            state.Elements.Clear();
            state.Time = (float)element.StartTime;
            state.Endtime = (float)element.EndTime;

            var path = element.GetParameter("path", "");
            var script = LoadScript(path, state.Elements);

            if (script != null)
            {
                script.Process(element);
                state.Script = script;
            }

            state.Loaded = false;
            syncLoadedRanges();

            pendingStates.Add(state);
            loadQueue.Enqueue(() => activateScriptAsync(state));
        }
    }

    public void RemoveElements(IEnumerable<StoryboardElement> elements)
    {
        foreach (var element in elements.Where(e => e.Type == StoryboardElementType.Script))
        {
            var state = scripts.FirstOrDefault(s => s.SourceElement == element);
            if (state == null) continue;

            if (state.Loaded)
            {
                foreach (var l in state.Elements.Select(e => e.Layer).Distinct())
                    ScriptElementsRemoved?.Invoke(l, state.SourceElement);
            }

            scripts.Remove(state);
            pendingStates.Remove(state);
        }

        syncLoadedRanges();
    }

    protected override void LoadScripts()
    {
        var elements = BindableStoryboard.Value.Elements
                                         .Where(e => e.Type == StoryboardElementType.Script)
                                         .ToList();

        foreach (var element in elements)
        {
            var path = element.GetParameter("path", "");
            var generatedElements = new List<StoryboardElement>();
            var script = LoadScript(path, generatedElements);

            script?.Process(element);

            var state = new ScriptState
            {
                SourceElement = element,
                Time = (float)element.StartTime,
                Endtime = (float)element.EndTime,
                Script = script,
                Elements = generatedElements,
                Loaded = false
            };

            state.OnRebuildStart = () =>
            {
                state.Loaded = false;
                syncLoadedRanges();
            };

            state.OnRebuildFinish = () =>
            {
                state.Loaded = true;
                syncLoadedRanges();

                foreach (var l in state.Elements.Select(e => e.Layer).Distinct())
                    Updated?.Invoke(l);
            };

            scripts.Add(state);
            pendingStates.Add(state);
            loadQueue.Enqueue(() => activateScriptAsync(state));
        }

        syncLoadedRanges();
    }

    protected override void Update()
    {
        base.Update();

        processLoadQueue();
    }

    private void processLoadQueue()
    {
        while (loadQueue.Count > 0)
        {
            var work = loadQueue.Dequeue();
            work().ContinueWith(t =>
            {
                if (t.IsFaulted)
                    Logger.Error(t.Exception, "Error occured during processing load queue for dynamic storyboard");
            });
        }
    }

    private async Task activateScriptAsync(ScriptState state)
    {
        if (state.Loaded) return;

        Schedule(() => state.OnRebuildStart?.Invoke());

        try
        {
            var byLayer = await Task.Run(() =>
                state.Elements
                     .GroupBy(e => e.Layer)
                     .Select(g => (Layer: g.Key, Elements: g.ToList()))
                     .ToList()
            );

            Schedule(() =>
            {
                foreach (var (l, elems) in byLayer)
                    ScriptElementsAdded?.Invoke(l, state.SourceElement, elems);

                state.OnRebuildFinish?.Invoke();
                pendingStates.Remove(state);
            });
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Failed to activate storyboard script '{state.SourceElement.GetParameter("path", "")}'.");
            Schedule(() => pendingStates.Remove(state));
        }
    }

    // TODO:
    // Currently this is very manual and tedious.
    // It should probably be revisited in the future to prevent a desynced state for what relies on LoadedRanges.
    private void syncLoadedRanges()
    {
        LoadedRanges.Clear();
        LoadedRanges.AddRange(scripts.Select(s => (s.Time, s.Endtime, s.Loaded)));
    }

    private class ScriptState : IHasLoadedValue
    {
        public StoryboardElement SourceElement { get; set; }
        public float Time;
        public float Endtime;
        public float Duration => Endtime - Time;

        public bool Loaded { get; set; }

        public Action OnRebuildStart;
        public Action OnRebuildFinish;

        public StoryboardScriptRunner Script;
        public List<StoryboardElement> Elements { get; set; } = new();
    }
}
