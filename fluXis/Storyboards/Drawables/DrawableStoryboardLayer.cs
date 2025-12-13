using System.Collections.Generic;
using System.Linq;
using fluXis.Storyboards.Drawables.Elements;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Timing;

namespace fluXis.Storyboards.Drawables;

public partial class DrawableStoryboardLayer : DrawSizePreservingFillContainer
{
    private IFrameBasedClock clock { get; }
    private DrawableStoryboard storyboard { get; }
    private StoryboardLayer layer { get; }

    private readonly Stack<DrawableStoryboardElement> past = new();
    private readonly Stack<DrawableStoryboardElement> future = new();

    private SortingContainer container;
    private DependencyContainer dependencies;

    public DrawableStoryboardLayer(IFrameBasedClock clock, DrawableStoryboard storyboard, StoryboardLayer layer)
    {
        this.clock = clock;
        this.storyboard = storyboard;
        this.layer = layer;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        TargetDrawSize = storyboard.Storyboard.Resolution;

        dependencies.CacheAs(storyboard.Storage);

        var drawables = new List<DrawableStoryboardElement>();
        var elements = storyboard.Storyboard.Elements.Where(x => x.Layer == layer).ToList();

        foreach (var element in elements)
        {
            var drawable = element.Type switch
            {
                StoryboardElementType.Box => new DrawableStoryboardBox(element),
                StoryboardElementType.Sprite => new DrawableStoryboardSprite(element),
                StoryboardElementType.Text => new DrawableStoryboardText(element),
                StoryboardElementType.Script => null,
                StoryboardElementType.Circle => new DrawableStoryboardCircle(element),
                StoryboardElementType.OutlineCircle => new DrawableStoryboardOutlineCircle(element),
                StoryboardElementType.SkinSprite => new DrawableStoryboardSkinSprite(element),
                _ => new DrawableStoryboardElement(element)
            };

            if (drawable is null)
                continue;

            // preload to avoid lagspikes when loading sprites
            drawable.Clock = clock;
            LoadComponent(drawable);
            drawables.Add(drawable);
        }

        Child = container = new SortingContainer
        {
            Size = TargetDrawSize,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Masking = true,
            Clock = clock
        };

        drawables.OrderByDescending(x => x.Element.StartTime).ForEach(x => future.Push(x));
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    protected override void Update()
    {
        base.Update();

        while (future.Count > 0 && future.Peek().Element.StartTime <= clock.CurrentTime)
        {
            var drawable = future.Pop();
            container.Add(drawable);
        }

        var tooEarly = container.Children.Where(c => c.Element.StartTime > clock.CurrentTime).ToList();

        foreach (var drawable in tooEarly.OrderByDescending(x => x.Element.StartTime))
        {
            container.Remove(drawable, false);
            future.Push(drawable);
        }

        var toRemove = container.Children.Where(c => c.Element.EndTime < clock.CurrentTime).ToList();

        foreach (var drawable in toRemove.OrderByDescending(x => x.Element.EndTime))
        {
            container.Remove(drawable, false);
            past.Push(drawable);
        }

        while (past.Count > 0 && past.Peek().Element.EndTime > clock.CurrentTime)
        {
            var drawable = past.Pop();
            container.Add(drawable);
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        while (past.TryPop(out var p))
            p.Dispose();

        while (future.TryPop(out var f))
            f.Dispose();
    }

    private partial class SortingContainer : Container<DrawableStoryboardElement>
    {
        protected override void AddInternal(Drawable drawable)
        {
            var last = this.LastOrDefault();
            base.AddInternal(drawable);

            if (last is null || drawable is not DrawableStoryboardElement el)
                return;

            if (last.Element.ZIndex >= el.Element.ZIndex)
                SortInternal();
        }

        protected override int Compare(Drawable x, Drawable y)
        {
            var a = (DrawableStoryboardElement)x;
            var b = (DrawableStoryboardElement)y;

            var result = a.Element.ZIndex.CompareTo(b.Element.ZIndex);
            if (result != 0) return result;

            result = a.Element.StartTime.CompareTo(b.Element.StartTime);
            if (result != 0) return result;

            return -a.GetHashCode().CompareTo(b.GetHashCode());
        }
    }
}
