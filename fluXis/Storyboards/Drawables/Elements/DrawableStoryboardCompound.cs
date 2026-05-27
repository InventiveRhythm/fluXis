using System;
using System.Collections.Generic;
using System.Linq;
using Midori.Utils;
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
    protected IFrameBasedClock FramedClock { get; private set; }

    [Resolved]
    protected DrawableStoryboard Storyboard { get; private set; }

    protected int ChildrenDepth;

    protected readonly string ID = string.Empty;
    protected readonly List<StoryboardElement> Children;

    protected readonly Stack<DrawableStoryboardElement> Past = new();
    protected readonly Stack<DrawableStoryboardElement> Future = new();

    protected SortingContainer Container;

    public DrawableStoryboardCompound(StoryboardElement element)
        : this(element, [])
    {
        ID = element.GetParameter("id", "");
    }

    public DrawableStoryboardCompound(StoryboardElement element, List<StoryboardElement> children)
        : base(element)
    {
        Children = children;

        if (element.Type != StoryboardElementType.Compound)
            throw new ArgumentException($"Element provided is not {nameof(StoryboardElementType.Compound)}", nameof(element));
    }

    protected DrawableStoryboardElement CreateDrawableFor(StoryboardElement el)
    {
        var element = el.JsonCopy();
        element.StartTime += Element.StartTime;
        element.EndTime += Element.StartTime;

        var drawable = element.Type switch
        {
            StoryboardElementType.Box => new DrawableStoryboardBox(element),
            StoryboardElementType.Sprite => new DrawableStoryboardSprite(element),
            StoryboardElementType.Text => new DrawableStoryboardText(element),
            StoryboardElementType.Circle => new DrawableStoryboardCircle(element),
            StoryboardElementType.OutlineCircle => new DrawableStoryboardOutlineCircle(element),
            StoryboardElementType.SkinSprite => new DrawableStoryboardSkinSprite(element),
            StoryboardElementType.OutlineBox => new DrawableStoryboardOutlineBox(element),
            StoryboardElementType.Compound => new DrawableMutableStoryboardCompound(element, []),
            _ => new DrawableStoryboardElement(element)
        };

        drawable.Clock = FramedClock;
        return drawable;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        if (ChildrenDepth >= 10)
        {
            Logger.Log("Nested compound depth is over 10, no more elements will be created.", LoggingTarget.Runtime, LogLevel.Important);
            return;
        }

        if (!string.IsNullOrWhiteSpace(ID) && Storyboard.Storyboard.Compounds.TryGetValue(ID, out var sub))
            Children.AddRange(sub);

        var drawables = new List<DrawableStoryboardElement>();

        foreach (var el in Children)
        {
            var drawable = CreateDrawableFor(el);

            if (drawable is null)
                continue;

            // preload to avoid lagspikes when loading sprites
            LoadComponent(drawable);
            drawables.Add(drawable);
        }

        InternalChild = Container = new SortingContainer
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Clock = FramedClock
        };

        drawables.OrderByDescending(x => x.Element.StartTime).ForEach(x => Future.Push(x));
    }

    protected override void Update()
    {
        base.Update();

        while (Future.Count > 0 && Future.Peek().Element.StartTime <= FramedClock.CurrentTime)
        {
            var drawable = Future.Pop();
            Container.Add(drawable);
        }

        var tooEarly = Container.Children.Where(c => c.Element.StartTime > FramedClock.CurrentTime).ToList();

        foreach (var drawable in tooEarly.OrderByDescending(x => x.Element.StartTime))
        {
            Container.Remove(drawable, false);
            Future.Push(drawable);
        }

        var toRemove = Container.Children.Where(c => c.Element.EndTime < FramedClock.CurrentTime).ToList();

        foreach (var drawable in toRemove.OrderBy(x => x.Element.EndTime))
        {
            Container.Remove(drawable, false);
            Past.Push(drawable);
        }

        while (Past.Count > 0 && Past.Peek().Element.EndTime > FramedClock.CurrentTime)
        {
            var drawable = Past.Pop();
            Container.Add(drawable);
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        while (Past.TryPop(out var p))
            p.Dispose();

        while (Future.TryPop(out var f))
            f.Dispose();
    }

    protected partial class SortingContainer : Container<DrawableStoryboardElement>
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
