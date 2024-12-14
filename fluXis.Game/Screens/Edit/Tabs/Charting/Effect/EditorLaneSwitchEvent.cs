using System.Linq;
using fluXis.Game.Map.Structures.Events;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Effect;

public partial class EditorLaneSwitchEvent : FillFlowContainer
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorSettings settings { get; set; }

    [Resolved]
    private PointsSidebar points { get; set; }

    public LaneSwitchEvent Event { get; }

    private double length;

    public EditorLaneSwitchEvent(LaneSwitchEvent laneSwitch)
    {
        Event = laneSwitch;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomLeft;
        Direction = FillDirection.Horizontal;

        for (int i = 0; i < map.RealmMap.KeyCount; i++)
        {
            Add(new Column());
        }
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        map.LaneSwitchEventAdded += updateWrapper;
        map.LaneSwitchEventUpdated += updateWrapper;
        map.LaneSwitchEventRemoved += onRemove;

        update();
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        if (map == null)
            return;

        map.LaneSwitchEventAdded -= updateWrapper;
        map.LaneSwitchEventUpdated -= updateWrapper;
        map.LaneSwitchEventRemoved -= onRemove;
    }

    private void updateWrapper(LaneSwitchEvent switchEvent) => update();

    private void update()
    {
        var nextEvent = map.MapEvents.LaneSwitchEvents.FirstOrDefault(e => e.Time > Event.Time);

        if (nextEvent != null)
            length = nextEvent.Time - Event.Time;
        else
            length = clock.TrackLength - Event.Time;

        var current = LaneSwitchEvent.GetRow(Event.Count, map.RealmMap.KeyCount, map.MapInfo.NewLaneSwitchLayout);
        StateChange[] states = new StateChange[map.RealmMap.KeyCount];

        var previousEvent = map.MapEvents.LaneSwitchEvents.LastOrDefault(e => e.Time < Event.Time);

        if (previousEvent != null)
        {
            var prev = LaneSwitchEvent.GetRow(previousEvent.Count, map.RealmMap.KeyCount, map.MapInfo.NewLaneSwitchLayout);

            for (int i = 0; i < map.RealmMap.KeyCount; i++)
            {
                if (prev[i] != current[i])
                {
                    if (current[i])
                        states[i] = StateChange.NowShowing;
                    else
                        states[i] = StateChange.NowHiding;
                }
                else
                    states[i] = StateChange.TheSame;
            }
        }

        var factor = Event.Duration / length;

        if (double.IsNaN(factor))
            factor = 0;

        for (int i = 0; i < map.RealmMap.KeyCount; i++)
        {
            var hidden = !current[i];
            var column = Children[i] as Column;
            var state = states[i];

            if (column == null)
                continue;

            if (state == StateChange.TheSame)
            {
                if (hidden)
                    column.Show();
                else
                    column.Hide();

                column.SetDuration(0);
            }
            else
            {
                column.Show();
                column.SetDuration((float)factor, state == StateChange.NowShowing);
            }
        }
    }

    private void onRemove(LaneSwitchEvent switchEvent)
    {
        if (switchEvent == Event)
            Expire();
        else
            update();
    }

    protected override void Update()
    {
        Height = (float)(.5f * (length * settings.Zoom));
        Y = (float)(-.5f * ((Event.Time - clock.CurrentTime) * settings.Zoom));
    }

    protected override bool OnClick(ClickEvent e)
    {
        points.ShowPoint(Event);
        return true;
    }

    private partial class Column : CompositeDrawable
    {
        private Box full;
        private Box gradient;

        [BackgroundDependencyLoader]
        private void load()
        {
            Width = EditorHitObjectContainer.NOTEWIDTH;
            RelativeSizeAxes = Axes.Y;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Colour = Colour4.FromHex("#FF5555");
            Masking = true;

            InternalChildren = new Drawable[]
            {
                full = new Box
                {
                    RelativeSizeAxes = Axes.Both
                },
                gradient = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = ColourInfo.GradientVertical(Colour4.White, Colour4.White.Opacity(0)),
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft
                }
            };
        }

        public void SetDuration(float factor, bool reverse = false)
        {
            if (reverse)
            {
                full.Height = 0;
                gradient.Height = -factor;
                gradient.Origin = Anchor.TopLeft;
                return;
            }

            full.Height = 1 - factor;
            gradient.Height = factor;
            gradient.Origin = Anchor.BottomLeft;
        }

        public override void Show() => this.FadeTo(0.6f);
        public override void Hide() => this.FadeTo(.0002f);
    }

    private enum StateChange
    {
        TheSame,
        NowShowing,
        NowHiding
    }
}
