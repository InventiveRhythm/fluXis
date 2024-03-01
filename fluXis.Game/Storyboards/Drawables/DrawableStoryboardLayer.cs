using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Storyboards.Drawables.Elements;
using fluXis.Game.Storyboards.Storage;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Storyboards.Drawables;

public partial class DrawableStoryboardLayer : Container<DrawableStoryboardElement>
{
    [Cached]
    private StoryboardStorage storage { get; }

    private List<StoryboardElement> elements { get; }
    private List<DrawableStoryboardElement> drawables { get; } = new();

    public DrawableStoryboardLayer(StoryboardStorage storage, List<StoryboardElement> elements)
    {
        this.storage = storage;
        this.elements = elements;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        foreach (var element in elements)
        {
            var drawable = element.Type switch
            {
                StoryboardElementType.Box => new DrawableStoryboardBox(element),
                StoryboardElementType.Sprite => new DrawableStoryboardSprite(element),
                _ => new DrawableStoryboardElement(element),
            };

            // preload to avoid lagspikes when loading sprites
            LoadComponent(drawable);
            drawables.Add(drawable);
        }

        drawables.Sort((a, b) => a.Element.StartTime.CompareTo(b.Element.StartTime));
    }

    protected override void Update()
    {
        base.Update();

        while (drawables.Count > 0 && drawables[0].Element.StartTime <= Time.Current)
        {
            AddInternal(drawables[0]);
            drawables.RemoveAt(0);
        }

        var toRemove = Children.Where(c => c.Element.EndTime < Time.Current).ToList();

        foreach (var drawable in toRemove)
            Remove(drawable, true);
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => new DependencyContainer(base.CreateChildDependencies(parent));
}
