using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Input;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Edit;

public partial class Editor : Screen, IKeyBindingHandler<FluXisKeybind>
{
    public RealmMap Map;
    public MapInfo OriginalMapInfo;
    public MapInfo MapInfo;

    private Container tabs;
    private int currentTab;

    public Editor(RealmMap realmMap = null, MapInfo map = null)
    {
        Map = realmMap ?? RealmMap.CreateNew();
        OriginalMapInfo = map ?? new MapInfo(new MapMetadata());
        MapInfo = OriginalMapInfo.Clone();
    }

    [BackgroundDependencyLoader]
    private void load(BackgroundStack backgrounds)
    {
        backgrounds.AddBackgroundFromMap(Map);

        InternalChildren = new Drawable[]
        {
            tabs = new Container
            {
                Padding = new MarginPadding(10) { Top = 50 },
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new SetupTab(this),
                    new ComposeTab(this),
                    new TimingTab(this),
                }
            },
            new EditorToolbar(this)
        };

        ChangeTab(0);
    }

    public void ChangeTab(int to)
    {
        currentTab = to;

        if (currentTab < 0)
            currentTab = 0;
        if (currentTab >= tabs.Count)
            currentTab = tabs.Count - 1;

        for (var i = 0; i < tabs.Children.Count; i++)
        {
            Drawable tab = tabs.Children[i];
            tab.FadeTo(i == currentTab ? 1 : 0);
        }
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisKeybind.Back:
                this.Exit();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e)
    {
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.FadeInFromZero(100);
        base.OnEntering(e);
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        this.FadeOut(100);
        return base.OnExiting(e);
    }
}
