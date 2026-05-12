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

    private double lastTime;

    protected override void Update()
    {
        base.Update();
        double currentTime = FramedClock.CurrentTime;

        // this is only used to guarantee that our state will always be correct when seeking
        // this is still performant since seeking is a one time thing as opposed to constantly updating hundreds of times a second
        // hardcoded to 2 seconds as we assume that there is no continuous seek/play that is greater than that
        // we might consider using the current snap divisor (with respect to BPM) when in editor though
        bool isSeek = Math.Abs(currentTime - lastTime) > 2000 || currentTime < lastTime;

        if (isSeek)
        {
            handleSeek(currentTime);
        }
        else
        {
            while (Future.Count > 0 && Future.Peek().Element.StartTime <= currentTime)
            {
                Container.Add(Future.Pop());
            }

            for (int i = Container.Count - 1; i >= 0; i--)
            {
                var drawable = Container[i];

                if (drawable.Element.EndTime < currentTime)
                {
                    Container.Remove(drawable, false);
                    Past.Push(drawable);
                }
            }
        }

        lastTime = currentTime;
    }

    private void handleSeek(double currentTime)
    {
        var allDrawables = new List<DrawableStoryboardElement>();
        allDrawables.AddRange(Container.Children);

        while (Past.TryPop(out var p)) allDrawables.Add(p);
        while (Future.TryPop(out var f)) allDrawables.Add(f);

        Container.Clear(false);

        var futureElements = new List<DrawableStoryboardElement>();
        var pastElements = new List<DrawableStoryboardElement>();

        foreach (var drawable in allDrawables)
        {
            if (drawable.Element.StartTime > currentTime)
                futureElements.Add(drawable);
            else if (drawable.Element.EndTime < currentTime)
                pastElements.Add(drawable);
            else
                Container.Add(drawable);
        }

        foreach (var f in futureElements.OrderByDescending(x => x.Element.StartTime))
            Future.Push(f);

        foreach (var p in pastElements.OrderBy(x => x.Element.EndTime))
            Past.Push(p);
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
