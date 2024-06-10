using System.Collections.Generic;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Screens.Edit.Tabs.Design.Effects;
using fluXis.Game.Screens.Edit.Tabs.Design.Playfield;
using fluXis.Game.Screens.Edit.Tabs.Design.Points;
using fluXis.Game.Screens.Edit.Tabs.Design.Toolbox;
using fluXis.Game.Screens.Edit.Tabs.Shared;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points;
using fluXis.Game.Screens.Edit.Tabs.Shared.Toolbox;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Design;

public partial class DesignContainer : EditorTabContainer
{
    private BackgroundVideo backgroundVideo;
    private bool showingVideo;

    protected override IEnumerable<Drawable> CreateContent()
    {
        return new Drawable[]
        {
            backgroundVideo = new BackgroundVideo
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                VideoClock = EditorClock,
                ShowDim = false
            },
            new EditorFlashLayer { InBackground = true },
            new EditorDesignPlayfield(),
            new EditorFlashLayer()
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        backgroundVideo.Map = Map.RealmMap;
        backgroundVideo.Info = Map.MapInfo;
        backgroundVideo.LoadVideo();
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Key == Key.V)
        {
            showingVideo = !showingVideo;

            if (showingVideo)
                backgroundVideo.Start();
            else
                backgroundVideo.Stop();

            return true;
        }

        return base.OnKeyDown(e);
    }

    protected override EditorToolbox CreateToolbox() => new DesignToolbox();
    protected override PointsSidebar CreatePointsSidebar() => new DesignSidebar();
}
