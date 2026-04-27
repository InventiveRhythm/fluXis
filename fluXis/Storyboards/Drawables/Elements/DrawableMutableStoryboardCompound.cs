using System.Collections.Generic;
using System.Linq;

namespace fluXis.Storyboards.Drawables.Elements;

public partial class DrawableMutableStoryboardCompound : DrawableStoryboardCompound
{
    private readonly Dictionary<StoryboardElement, DrawableStoryboardElement> elements = new();

    public DrawableMutableStoryboardCompound(StoryboardElement element, List<StoryboardElement> children)
        : base(element, children)
    {
    }

    public void AddElement(StoryboardElement el) => AddDrawable(CreateDrawableFor(el), el);

    public void AddDrawable(DrawableStoryboardElement drawable, StoryboardElement source)
    {
        RemoveElement(source);
        elements[source] = drawable;

        LoadComponentAsync(drawable, d =>
        {
            // VERY IMPORTANT If the user updated this element again while we were loading in the background,
            // this drawable is now obsolete and ee must abort and destroy it so it doesn't duplicate
            if (elements.GetValueOrDefault(source) != d)
            {
                d.Dispose();
                return;
            }

            if (d.Element.EndTime < FramedClock.CurrentTime)
                Past.Push(d);
            else if (d.Element.StartTime > FramedClock.CurrentTime)
            {
                Future.Push(d);
                sortFuture();
            }
            else
                Container.Add(d);
        });
    }

    public void RemoveElement(StoryboardElement el)
    {
        if (!elements.Remove(el, out var drawable)) return;

        if (Container.Contains(drawable))
            Container.Remove(drawable, true);
        else
        {
            if (!removeFromStack(Past, drawable))
            {
                if (removeFromStack(Future, drawable))
                    sortFuture();
            }

            // If it was stuck in a stack or still loading, dispose of it manually
            drawable.Expire();
            drawable.Dispose();
        }
    }

    public DrawableStoryboardElement GetDrawable(StoryboardElement el) => elements.GetValueOrDefault(el);

    private static bool removeFromStack(Stack<DrawableStoryboardElement> stack, DrawableStoryboardElement item)
    {
        if (!stack.Contains(item)) return false;

        var list = stack.ToList();
        list.Remove(item);
        stack.Clear();

        // .ToList() reads from top to bottom
        // We MUST reverse it to bottom to top before pushing back
        list.Reverse();

        foreach (var d in list) stack.Push(d);
        return true;
    }

    private void sortFuture()
    {
        var list = Future.OrderByDescending(x => x.Element.StartTime).ToList();
        Future.Clear();
        foreach (var d in list) Future.Push(d);
    }
}
