using System;
using fluXis.Audio;
using fluXis.Database.Maps;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Screens.Select.Footer.Practice;

public partial class FooterPracticePlayhead : Container
{   
    [Resolved]
    private MapStore maps { get; set; }

    [Resolved]
    private GlobalClock globalClock { get; set; }

    private BindableNumber<int> start { get; }
    private BindableNumber<int> end { get; }

    private int endTime = 1;

    private Drawable playhead;

    public FooterPracticePlayhead(BindableNumber<int> start, BindableNumber<int> end)
    {
        this.start = start;
        this.end = end;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomLeft;
        Children = new Drawable[]
        {
            playhead = new Circle
            {
                RelativeSizeAxes = Axes.Y,
                Width = 3,
                Colour = Theme.Aqua,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
            }
        };
    }

    protected override void LoadComplete()
    {
        maps.MapBindable.BindValueChanged(mapChanged, true);
    }

    private void mapChanged(ValueChangedEvent<RealmMap> v)
    {
        var info = v.NewValue.GetMapInfo();
        endTime = (int)info.EndTime;
    }

    protected override void Update()
    {
        base.Update();
        if (globalClock.CurrentTrack == null) return;

        int currentTime = (int)globalClock.CurrentTrack.CurrentTime;

        if (currentTime < start.Value * 1000 || currentTime > end.Value * 1000)
            playhead.Alpha = .25f;
        else
            playhead.Alpha = 0.7f;

        float currentTimePercetange = Math.Clamp((float)currentTime / endTime, 0f, 1f);
        playhead.MoveToX(currentTimePercetange * DrawWidth);
    }
}