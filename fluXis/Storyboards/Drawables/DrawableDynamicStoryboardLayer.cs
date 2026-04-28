using System.Collections.Generic;
using System.Linq;
using fluXis.Storyboards.Drawables.Elements;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Threading;
using osu.Framework.Timing;

namespace fluXis.Storyboards.Drawables;

public partial class DrawableDynamicStoryboardLayer : DrawSizePreservingFillContainer
{
    private IFrameBasedClock clock { get; }
    private DrawableDynamicStoryboard storyboard { get; }
    private StoryboardLayer layer { get; }
    private DependencyContainer dependencies;

    private DrawableMutableStoryboardCompound masterCompound;
    private Dictionary<StoryboardElement, DrawableMutableStoryboardCompound> scriptDrawables { get; } = new();

    private readonly Dictionary<StoryboardElement, PendingAction> pendingActions = new();
    private ScheduledDelegate pendingFlushDelegate;

    private const float static_debounce = 1000 / 60f; // debounced at 60 fps

    public DrawableDynamicStoryboardLayer(IFrameBasedClock clock, DrawableDynamicStoryboard storyboard, StoryboardLayer layer)
    {
        this.clock = clock;
        this.storyboard = storyboard;
        this.layer = layer;
    }

    // Some clarification:
    // We call normal (non script created) elements as 'static' because we don't need to track it to rebuild it.
    // Static elements are debounced here but script elements are debounced in storyboard tab via an IdleTracker; This is so that updating static elements is self-contained and as reactive as possible.

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        TargetDrawSize = storyboard.BindableStoryboard.Value.Resolution;

        dependencies.CacheAs(clock);
        dependencies.CacheAs<DrawableStoryboard>(storyboard);
        dependencies.CacheAs(storyboard.Storage);

        Child = masterCompound = new DrawableMutableStoryboardCompound(createCompoundElement(), []);
        buildStatic();
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        storyboard.ScriptElementsAdded += onScriptElementsAdded;
        storyboard.ScriptElementsRemoved += onScriptElementsRemoved;
    }

    #region Building

    private void buildStatic()
    {
        var elements = storyboard.BindableStoryboard.Value.Elements
                                 .Where(x => x.Layer == layer && x.Type != StoryboardElementType.Script);

        foreach (var element in elements)
            AddStaticElement(element);
    }

    public void RebuildStatic() => buildStatic();

    #endregion

    #region Event Handling

    private void onScriptElementsAdded(StoryboardLayer l, StoryboardElement source, IEnumerable<StoryboardElement> elements)
    {
        if (l != layer) return;

        // We have to remove old elements (if it exists) to prevent duplication
        onScriptElementsRemoved(l, source);

        // we create a new compound for every script element. this doesn't match our normal DrawableStoryboardLayer,
        // but it's definitely cleaner and more effective to do so
        var compound = new DrawableMutableStoryboardCompound(createCompoundElement(), elements.ToList());
        scriptDrawables[source] = compound;
        Add(compound);
    }

    private void onScriptElementsRemoved(StoryboardLayer l, StoryboardElement source)
    {
        if (l != layer) return;

        if (scriptDrawables.Remove(source, out var existing))
            existing.Expire();
    }

    #endregion

    #region Creation

    private StoryboardElement createCompoundElement() => new()
    {
        Type = StoryboardElementType.Compound,
        Width = TargetDrawSize.X,
        Height = TargetDrawSize.Y,
        Anchor = Anchor.Centre,
        Origin = Anchor.Centre
    };

    public void AddStaticElement(StoryboardElement element)
    {
        if (element.Layer == layer)
            masterCompound.AddElement(element);
    }

    public void RemoveStaticElement(StoryboardElement element) => masterCompound.RemoveElement(element);

    public void UpdateStaticElement(StoryboardElement element)
    {
        pendingActions[element] = PendingAction.Update;
        schedulePendingFlush();
    }

    public void QueueAddStaticElement(StoryboardElement element)
    {
        pendingActions[element] = PendingAction.Add;
        schedulePendingFlush();
    }

    public void QueueRemoveStaticElement(StoryboardElement element)
    {
        pendingActions[element] = PendingAction.Remove;
        schedulePendingFlush();
    }

    private void schedulePendingFlush()
    {
        pendingFlushDelegate?.Cancel();
        pendingFlushDelegate = Scheduler.AddDelayed(executePendingActions, static_debounce);
    }

    private void executePendingActions()
    {
        foreach (var (element, action) in pendingActions)
        {
            switch (action)
            {
                case PendingAction.Remove:
                    masterCompound.RemoveElement(element);
                    break;

                case PendingAction.Update:
                    masterCompound.RemoveElement(element);
                    if (element.Layer == layer)
                        masterCompound.AddElement(element);
                    break;

                case PendingAction.Add:
                    if (element.Layer == layer)
                        masterCompound.AddElement(element);
                    break;
            }
        }

        pendingActions.Clear();
    }

    #endregion

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        storyboard.ScriptElementsAdded -= onScriptElementsAdded;
        storyboard.ScriptElementsRemoved -= onScriptElementsRemoved;
    }

    private enum PendingAction
    {
        Add,
        Remove,
        Update
    }
}
