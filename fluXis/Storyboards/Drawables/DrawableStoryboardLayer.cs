using System.Collections.Generic;
using System.Linq;
using fluXis.Storyboards.Drawables.Elements;
using fluXis.Storyboards.Storage;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Storyboards.Drawables;

public partial class DrawableStoryboardLayer : Container<DrawableStoryboardElement>
{
    [Cached]
    private StoryboardStorage storage { get; }

    private List<StoryboardElement> elements { get; }

    private readonly Stack<DrawableStoryboardElement> past = new();
    private readonly Stack<DrawableStoryboardElement> future = new();

    public DrawableStoryboardLayer(StoryboardStorage storage, List<StoryboardElement> elements)
    {
        this.storage = storage;
        this.elements = elements;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        var drawables = new List<DrawableStoryboardElement>();

        foreach (var element in elements)
        {
            var drawable = element.Type switch
            {
                StoryboardElementType.Box => new DrawableStoryboardBox(element),
                StoryboardElementType.Sprite => new DrawableStoryboardSprite(element),
                StoryboardElementType.Text => new DrawableStoryboardText(element),
                StoryboardElementType.Script => null,
                _ => new DrawableStoryboardElement(element)
            };

            if (drawable is null)
                continue;

            // preload to avoid lagspikes when loading sprites
            LoadComponent(drawable);
            drawables.Add(drawable);
        }

        drawables.OrderByDescending(x => x.Element.StartTime).ForEach(x => future.Push(x));
    }

    protected override void Update()
    {
        base.Update();

        while (future.Count > 0 && future.Peek().Element.StartTime <= Time.Current)
        {
            var drawable = future.Pop();
            AddInternal(drawable);
        }

        var tooEarly = Children.Where(c => c.Element.StartTime > Time.Current).ToList();

        foreach (var drawable in tooEarly.OrderByDescending(x => x.Element.StartTime))
        {
            Remove(drawable, false);
            future.Push(drawable);
        }

        var toRemove = Children.Where(c => c.Element.EndTime < Time.Current).ToList();

        foreach (var drawable in toRemove.OrderByDescending(x => x.Element.EndTime))
        {
            Remove(drawable, false);
            past.Push(drawable);
        }

        while (past.Count > 0 && past.Peek().Element.EndTime > Time.Current)
        {
            var drawable = past.Pop();
            AddInternal(drawable);
        }
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => new DependencyContainer(base.CreateChildDependencies(parent));

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        while (past.TryPop(out var p))
            p.Dispose();

        while (future.TryPop(out var f))
            f.Dispose();
    }
}
