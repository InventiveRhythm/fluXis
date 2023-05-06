using System.IO;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Input;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs;
using fluXis.Game.Screens.Edit.Timeline;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Edit;

public partial class Editor : FluXisScreen, IKeyBindingHandler<FluXisKeybind>
{
    public override bool ShowToolbar => false;

    public RealmMap Map;
    public MapInfo OriginalMapInfo;
    public MapInfo MapInfo;

    private Container tabs;
    private int currentTab;

    private ComposeTab composeTab;
    private EditorBottomBar bottomBar;

    private EditorClock clock;
    private Bindable<Waveform> waveform = new();
    private EditorChangeHandler changeHandler = new();
    private EditorValues values = new();

    private DependencyContainer dependencies;

    public Editor(RealmMap realmMap = null, MapInfo map = null)
    {
        Map = realmMap ?? RealmMap.CreateNew();
        OriginalMapInfo = map ?? new MapInfo(new MapMetadata());
        MapInfo = OriginalMapInfo.Clone();

        Conductor.ResetLoop();
        Conductor.PauseTrack(); // the editor clock will handle this
    }

    [BackgroundDependencyLoader]
    private void load(BackgroundStack backgrounds, AudioManager audioManager, Storage storage)
    {
        backgrounds.AddBackgroundFromMap(Map);

        clock = new EditorClock(MapInfo);
        clock.ChangeSource(loadMapTrack(audioManager.GetTrackStore(new StorageBackedResourceStore(storage.GetStorageForDirectory("files")))));
        dependencies.CacheAs(clock);

        dependencies.CacheAs(waveform);
        dependencies.CacheAs(changeHandler);
        dependencies.CacheAs(values);

        InternalChildren = new Drawable[]
        {
            clock,
            tabs = new Container
            {
                Padding = new MarginPadding(10) { Top = 50, Bottom = 60 },
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new SetupTab(this),
                    composeTab = new ComposeTab(this),
                    new TimingTab(this)
                }
            },
            new EditorToolbar(this),
            bottomBar = new EditorBottomBar { Editor = this }
        };
    }

    private Track loadMapTrack(ITrackStore trackStore)
    {
        string path = Map.MapSet?.GetFile(Map.Metadata?.Audio)?.GetPath();

        Waveform w = null;

        if (!string.IsNullOrEmpty(path))
        {
            Stream s = trackStore.GetStream(path);

            if (s != null)
                w = new Waveform(s);
        }

        waveform.Value = w;
        return trackStore.Get(path) ?? trackStore.GetVirtual(10000);
    }

    protected override void LoadComplete()
    {
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
        clock.Stop();
        return false;
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
}
