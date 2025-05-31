using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Screens.Edit.Tabs.Design;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Screens.Edit.Tabs;

public partial class DesignTab : EditorTab
{
    public override IconUsage Icon => FontAwesome6.Solid.Palette;
    public override string TabName => "Design";

    private LoadingIcon loadingIcon;

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = loadingIcon = new LoadingIcon
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Size = new Vector2(50)
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        LoadComponentAsync(new DesignContainer(), container =>
        {
            loadingIcon.FadeOut(200);

            AddInternal(container);
            container.FadeInFromZero(200);
        });
    }
}
