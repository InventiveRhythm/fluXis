using System;
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
    public override bool HasLoading => true;

    private LoadingIcon loadingIcon;
    public DesignContainer Container { get; private set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Container = new DesignContainer();
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

        LoadComponentAsync(Container, container =>
        {
            loadingIcon.FadeOut(200);

            AddInternal(container);
            container.FadeInFromZero(200);
        });
    }

    public override void ScheduleAfterLoad(Action act)
    {
        if (Container.IsLoaded)
            act.Invoke();
        else
            Container.OnLoadComplete += _ => act();
    }
}
