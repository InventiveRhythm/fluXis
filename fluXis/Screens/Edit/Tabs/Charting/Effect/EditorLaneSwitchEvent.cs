using System;
using System.Linq;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Screens.Edit.Tabs.Charting.Tools;
using fluXis.Screens.Edit.Tabs.Shared.Points;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Edit.Tabs.Charting.Effect;

public partial class EditorLaneSwitchEvent : Container
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorSettings settings { get; set; }

    [Resolved]
    private PointsSidebar points { get; set; }

    [Resolved]
    private ChartingBlueprintContainer blueprintContainer { get; set; }

    public LaneSwitchEvent Event { get; }

    private static readonly Colour4 default_color = Theme.Red;

    private bool canInteract => blueprintContainer.CurrentTool is SelectTool && settings.LaneSwitchInteraction.Value;

    private double length;
    private int hoveredColumns = 0;
    private bool allColumnsHidden = false;

    private const float hidden_alpha = .0002f;
    private const float visible_alpha = 0.6f;
    private const float hovered_alpha = 0.7f;

    private const float alpha_threshold = 0.05f; // definetely greater than 0.002
    private const int fade_duration = 100;

    private readonly FillFlowContainer columns;

    private Container columnsHiddenIndicator;
    private Box columnsHiddenIndicatorFull;
    private Box columnsHiddenIndicatorGradient;

    public EditorLaneSwitchEvent(LaneSwitchEvent laneSwitch)
    {
        Event = laneSwitch;

        InternalChildren = new Drawable[]
        {
            columns = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Direction = FillDirection.Horizontal,
            },
            columnsHiddenIndicator = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Alpha = 0,
                Children = new Drawable[]
                {
                    columnsHiddenIndicatorFull = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.White,
                    },
                    columnsHiddenIndicatorGradient = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = ColourInfo.GradientVertical(Colour4.White, Colour4.White.Opacity(0)),
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                    }
                }
            }
        };
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomLeft;

        for (int i = 0; i < map.RealmMap.KeyCount; i++)
        {
            columns.Add(new Column(
                onHovered: () => setHighlight(true),
                onHoverLost: () => setHighlight(false)
            ));
        }
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        blueprintContainer.CurrentToolChanged += updateHightlight;

        map.RegisterAddListener<LaneSwitchEvent>(updateWrapper);
        map.RegisterUpdateListener<LaneSwitchEvent>(updateWrapper);
        map.RegisterRemoveListener<LaneSwitchEvent>(onRemove);

        update();
    }

    protected override bool OnHover(HoverEvent e)
    {
        if (canInteract && allColumnsHidden)
            columnsHiddenIndicator.FadeTo(0.1f, fade_duration);

        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        base.OnHoverLost(e);
        columnsHiddenIndicator.FadeOut(fade_duration);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        if (map == null)
            return;

        blueprintContainer.CurrentToolChanged -= updateHightlight;

        map.DeregisterAddListener<LaneSwitchEvent>(updateWrapper);
        map.DeregisterUpdateListener<LaneSwitchEvent>(updateWrapper);
        map.DeregisterRemoveListener<LaneSwitchEvent>(onRemove);
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

        allColumnsHidden = true;

        for (int i = 0; i < map.RealmMap.KeyCount; i++)
        {
            var hidden = !current[i];
            var column = columns.Children[i] as Column;
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

            if (hidden) allColumnsHidden = false;

            columnsHiddenIndicatorFull.Height = 1 - (float)factor;
            columnsHiddenIndicatorGradient.Height = (float)factor;
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
        var mousePos = e.ScreenSpaceMousePosition;

        if (canInteract)
        {
            if (allColumnsHidden)
            {
                points.ShowPoint(Event);
                return true;
            }

            foreach (var column in columns.Children.OfType<Column>())
            {
                if (column.Alpha < alpha_threshold) continue;

                bool clickedFull = column.Full.ScreenSpaceDrawQuad.Contains(mousePos);
                bool clickedGradient = column.Gradient.ScreenSpaceDrawQuad.Contains(mousePos);

                if (clickedFull || clickedGradient)
                {
                    points.ShowPoint(Event);
                    return true;
                }
            }
        }
        return false;
    }

    #region Interaction

    private void fadeAll(float newAlpha, double duration = 0, Easing easing = Easing.None)
    {
        foreach (var column in columns.Children.OfType<Column>().Where(c => c.Alpha > hidden_alpha))
            column.FadeTo(newAlpha, duration, easing);
    }

    private void highlightAny(bool shouldHighlight)
    {
        if (!allColumnsHidden)
        {
            fadeAll(shouldHighlight ? hovered_alpha : visible_alpha, fade_duration);
        }
    }

    private void updateHightlight()
    {
        bool isHovered = columns.Children.OfType<Column>().Any(c => c.IsActuallyHovering);

        hoveredColumns = 0;

        if (isHovered && canInteract)
        {
            hoveredColumns = 1;
            highlightAny(true);
        }
        else
        {
            highlightAny(false);
        }
    }

    private void setHighlight(bool isHovered)
    {
        hoveredColumns = isHovered ? hoveredColumns + 1 : Math.Max(0, hoveredColumns - 1);

        bool shouldHighlight = hoveredColumns > 0;

        if (shouldHighlight && hoveredColumns == 1 && canInteract)
            highlightAny(true);
        else if (!shouldHighlight && hoveredColumns == 0)
            highlightAny(false);
    }

    #endregion

    private partial class Column : CompositeDrawable
    {
        public Box Full { get; private set; }
        public Box Gradient { get; private set; }

        private readonly Action onHovered;
        private readonly Action onHoverLost;
        public bool IsActuallyHovering = false; // Using regular hover isn't accurate as it triggers for the entire column.

        public Column(Action onHovered, Action onHoverLost)
        {
            this.onHovered = onHovered;
            this.onHoverLost = onHoverLost;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Width = EditorHitObjectContainer.NOTEWIDTH;
            RelativeSizeAxes = Axes.Y;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Colour = default_color;
            Masking = true;

            InternalChildren = new Drawable[]
            {
                Full = new Box { RelativeSizeAxes = Axes.Both },
                Gradient = new Box
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
                Full.Height = 0;
                Gradient.Height = -factor;
                Gradient.Origin = Anchor.TopLeft;
                return;
            }

            Full.Height = 1 - factor;
            Gradient.Height = factor;
            Gradient.Origin = Anchor.BottomLeft;
        }

        public override void Show() => this.FadeTo(visible_alpha);
        public override void Hide() => this.FadeTo(hidden_alpha);

        protected override bool OnHover(HoverEvent e)
        {
            if (Alpha < alpha_threshold)
                return base.OnHover(e);

            bool hoveringFull = Full.ScreenSpaceDrawQuad.Contains(e.ScreenSpaceMousePosition);
            bool hoveringGradient = Gradient.ScreenSpaceDrawQuad.Contains(e.ScreenSpaceMousePosition);
            bool isHoveringVisible = hoveringFull || hoveringGradient;

            if (isHoveringVisible && !IsActuallyHovering)
            {
                IsActuallyHovering = true;
                onHovered?.Invoke();
            }

            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            base.OnHoverLost(e);

            if (IsActuallyHovering)
            {
                IsActuallyHovering = false;
                onHoverLost?.Invoke();
            }
        }
    }

    private enum StateChange
    {
        TheSame,
        NowShowing,
        NowHiding
    }
}
