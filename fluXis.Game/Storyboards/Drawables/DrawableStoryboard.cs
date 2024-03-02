using System.Linq;
using fluXis.Game.Storyboards.Storage;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Platform;

namespace fluXis.Game.Storyboards.Drawables;

public partial class DrawableStoryboard : CompositeDrawable
{
    public Storyboard Storyboard { get; }
    private string path { get; }

    private StoryboardStorage storage { get; set; }

    public DrawableStoryboard(Storyboard storyboard, string path)
    {
        Storyboard = storyboard;
        this.path = path;
    }

    [BackgroundDependencyLoader]
    private void load(GameHost host)
    {
        storage = new StoryboardStorage(host, path);
    }

    public DrawableStoryboardLayer GetLayer(StoryboardLayer layer)
        => new(storage, Storyboard.Elements.Where(e => e.Layer == layer).ToList());
}
