using System.Threading.Tasks;
using fluXis.Audio;
using fluXis.Database.Maps;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map;
using fluXis.Utils.Extensions;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Screens.Edit;

public partial class EditorLoader : FluXisScreen
{
    public override bool AllowMusicControl => false;
    public override bool PlayBackSound => false;
    public override bool ShowToolbar => false;

    [Resolved]
    private MapStore maps { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private GlobalClock clock { get; set; }

    private RealmMap map { get; set; }
    private MapInfo mapInfo { get; set; }

    /// <summary>
    /// see <see cref="Editor.StartTabIndex"/>
    /// </summary>
    public int StartTabIndex { get; set; } = -1;

    public const float DURATION = 900;
    private const float circle_size = 2500;
    private const float border_size = circle_size * .55f;

    private bool switching;

    private CircularContainer circle;

    public EditorLoader(RealmMap realmMap = null, MapInfo map = null)
    {
        this.map = realmMap;
        mapInfo = map;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Depth = -1;

        InternalChildren = new Drawable[]
        {
            circle = new CircularContainer
            {
                Size = new Vector2(circle_size),
                Scale = new Vector2(0),
                BorderColour = Theme.Background3,
                BorderThickness = border_size,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Masking = true,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.Transparent
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        pushEditor();
    }

    public void CreateNewDifficulty(RealmMap realmMap, MapInfo refInfo, CreateNewMapParameters param)
    {
        switching = true;
        this.MakeCurrent();

        var set = maps.GetFromGuid(realmMap.MapSet.ID);
        map = maps.CreateNewDifficulty(set, realmMap, refInfo, param);
        mapInfo = map.GetMapInfo();

        pushEditor();
    }

    public void SwitchTo(RealmMap realmMap)
    {
        switching = true;
        this.MakeCurrent();

        map = realmMap;
        mapInfo = map.GetMapInfo();

        pushEditor();
    }

    private void pushEditor() => Task.Run(() =>
    {
        var editorMap = map?.GetMapInfo<EditorMap.EditorMapInfo>();
        var events = mapInfo?.GetMapEvents();
        var sb = mapInfo?.GetStoryboard();

        if (editorMap != null && events != null)
            editorMap.MapEvents = events;
        if (editorMap != null && sb != null)
            editorMap.Storyboard = sb;

        Schedule(() => LoadComponentAsync(new Editor(this, map, editorMap) { StartTabIndex = StartTabIndex }, s => this.Delay(DURATION).FadeIn().OnComplete(_ => this.Push(s))));
        switching = false;
    });

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.Delay(DURATION).FadeIn();
        circle.ScaleTo(1f, DURATION, Easing.OutQuint);
        clock.VolumeOut(DURATION)?.OnComplete(c => c.Stop());
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        if (!switching)
        {
            circle.BorderTo(border_size, DURATION, Easing.OutQuint)
                  .OnComplete(_ => this.Exit());

            return;
        }

        circle.BorderTo(border_size).ScaleTo(0)
              .ScaleTo(1f, DURATION, Easing.OutQuint);
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        this.Delay(DURATION).FadeIn();
        circle.BorderTo(0f, DURATION, Easing.OutQuint);
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        this.FadeIn().Then(DURATION).FadeOut();
        circle.ScaleTo(0f, DURATION, Easing.OutQuint);
        clock?.Start();
        clock?.VolumeOut();
        clock?.VolumeIn(DURATION);

        return false;
    }
}
