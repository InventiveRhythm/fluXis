using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using fluXis.Storyboards.Drawables.Elements;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
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

    public DrawableDynamicStoryboardLayer(IFrameBasedClock clock, DrawableDynamicStoryboard storyboard, StoryboardLayer layer)
    {
        this.clock = clock;
        this.storyboard = storyboard;
        this.layer = layer;
    }

    // Some clarification:
    // We call normal (non script created) elements as 'static' because we don't need to track it to rebuild it.

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

        var elementsList = elements.ToList();
        // micro optimization to speed up iteration instead of using LINQ
        var span = CollectionsMarshal.AsSpan(elementsList);
        var startTime = source.StartTime;

        // Relative time doesn't work very well for our use case here so we convert to absolute time.
        // While we do not inherently need to match the time of the elements created by scripts as of now but,
        // it might become necessary to do when we can access compounds inside scripts in the future
        // TODO: remove the above 2 comments after adding compounds to scripts
        foreach (ref var e in span)
        {
            e.StartTime -= startTime;
            e.EndTime -= startTime;
        }

        // we create a new compound for every script element. this doesn't match our normal DrawableStoryboardLayer,
        // but it's definitely cleaner and more effective to do so
        var compoundEl = createCompoundElement(source);
        var compound = new DrawableMutableStoryboardCompound(compoundEl, elementsList);
        scriptDrawables[source] = compound;
        masterCompound.AddDrawable(compound, source);
    }

    private void onScriptElementsRemoved(StoryboardLayer l, StoryboardElement source)
    {
        if (l != layer) return;

        if (scriptDrawables.Remove(source, out _))
            masterCompound.RemoveElement(source);
    }

    #endregion

    #region Creation

    /// <summary>
    /// Use source if you want the compound to inherit some of an element's properties like scripts' start time and zindex
    /// </summary>
    private StoryboardElement createCompoundElement(StoryboardElement source = null)
    {
        var element = new StoryboardElement
        {
            Type = StoryboardElementType.Compound,
            Width = TargetDrawSize.X,
            Height = TargetDrawSize.Y,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Layer = layer,

            EndTime = double.MaxValue
        };

        if (source != null)
        {
            element.StartTime = source.StartTime;
            element.EndTime = source.EndTime;
            element.ZIndex = source.ZIndex;
        }

        return element;
    }

    public void AddStaticElement(StoryboardElement element)
    {
        if (element.Layer == layer)
            masterCompound.AddElement(element);
    }

    public void RemoveStaticElement(StoryboardElement element) => masterCompound.RemoveElement(element);

    public void UpdateStaticElement(StoryboardElement element)
    {
        masterCompound.RemoveElement(element);
        AddStaticElement(element);
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
}
