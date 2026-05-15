using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Timing;

namespace fluXis.Storyboards.Drawables.Elements;

public partial class DrawableStoryboardCompound : DrawableStoryboardElement
{
    [Resolved]
    private IFrameBasedClock clock { get; set; }

    [Resolved]
    private DrawableStoryboard storyboard { get; set; }

    private int depth;

    private readonly string id = string.Empty;
    private readonly List<StoryboardElement> children;

    private readonly Stack<DrawableStoryboardElement> past = new();
    private readonly Stack<DrawableStoryboardElement> future = new();

    private SortingContainer container;

    public DrawableStoryboardCompound(StoryboardElement element)
        : this(element, [])
    {
        id = element.GetParameter("id", "");
    }

    public DrawableStoryboardCompound(StoryboardElement element, List<StoryboardElement> children)
        : base(element)
    {
        this.children = children;

        if (element.Type != StoryboardElementType.Compound)
            throw new ArgumentException($"Element provided is not {nameof(StoryboardElementType.Compound)}", nameof(element));
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        if (depth >= 10)
        {
            Logger.Log("Nested compound depth is over 10, no more elements will be created.", LoggingTarget.Runtime, LogLevel.Important);
            return;
        }

        if (!string.IsNullOrWhiteSpace(id) && storyboard.Storyboard.Compounds.TryGetValue(id, out var sub))
            children.AddRange(sub);

        var drawables = new List<DrawableStoryboardElement>();

        foreach (var el in children)
        {
            var element = el.JsonCopy();
            element.StartTime += Element.StartTime;
            element.EndTime += Element.StartTime;

            var drawable = element.Type switch
            {
                StoryboardElementType.Box => new DrawableStoryboardBox(element),
                StoryboardElementType.Sprite => new DrawableStoryboardSprite(element),
                StoryboardElementType.Text => new DrawableStoryboardText(element),
                StoryboardElementType.Script => null,
                StoryboardElementType.Circle => new DrawableStoryboardCircle(element),
                StoryboardElementType.OutlineCircle => new DrawableStoryboardOutlineCircle(element),
                StoryboardElementType.SkinSprite => new DrawableStoryboardSkinSprite(element),
                StoryboardElementType.OutlineBox => new DrawableStoryboardOutlineBox(element),
                StoryboardElementType.Compound => new DrawableStoryboardCompound(element) { depth = depth + 1 },
                _ => new DrawableStoryboardElement(element)
            };

            if (drawable is null)
                continue;

            // preload to avoid lagspikes when loading sprites
            drawable.Clock = clock;
            LoadComponent(drawable);
            drawables.Add(drawable);
        }

        InternalChild = container = new SortingContainer
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Clock = clock
        };

        drawables.OrderByDescending(x => x.Element.StartTime).ForEach(x => future.Push(x));
    }

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

        foreach (var drawable in toRemove.OrderBy(x => x.Element.EndTime))
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
