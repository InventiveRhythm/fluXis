using System;
using System.Linq;
using fluXis.Graphics.Background;
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
using osu.Framework.Utils;
using osuTK;

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

    private static Colour4 defaultColor = Colour4.FromHex("#FF5555"); // Theme.LaneSwitch is a different color.

    private bool canInteract => blueprintContainer.CurrentTool is SelectTool && settings.LaneSwitchInteraction.Value;

    private double length;
    private int hoveredColumns = 0;
    private bool allColumnsHidden = false;

    private const float hidden_alpha = .0002f;
    private const float visible_alpha = 0.6f;
    private const float hovered_alpha = 0.8f;
    private const float ind_visible_alpha = 0.2f;
    private const float ind_hovered_alpha = 0.4f;

    private const float alpha_threshold = 0.05f; // definetely greater than 0.002
    private const int fade_duration = 100;

    private const float indicator_width = 8f;
    private const float expanded_indicator_width = 20f;

    private readonly FillFlowContainer columns;
    private readonly SwitchIndicator leftIndicator;
    private readonly SwitchIndicator rightIndicator;

    private bool anyIndicatorHovered => rightIndicator.IsHovered || leftIndicator.IsHovered;

    public EditorLaneSwitchEvent(LaneSwitchEvent laneSwitch)
    {
        Event = laneSwitch;
        
        InternalChildren = new Drawable[]
        {
            leftIndicator = new SwitchIndicator(false, onHovered: () => setHighlight(true), onHoverLost: () => setHighlight(false))
            {
                Width = indicator_width,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Action = onIndicatorClick
            },
            columns = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                RelativePositionAxes = Axes.X,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Direction = FillDirection.Horizontal,
                X = indicator_width,
            },
            rightIndicator = new SwitchIndicator(true, onHovered: () => setHighlight(true), onHoverLost: () => setHighlight(false))
            {
                Width = indicator_width,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomRight,
                Action = onIndicatorClick
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

            if (hidden) 
            {
                allColumnsHidden = false;
            }
        }

        if (allColumnsHidden && Precision.AlmostEquals(Event.Duration, 0))
        {
            leftIndicator.FadeIn(fade_duration);
            rightIndicator.FadeIn(fade_duration);
            columns.RelativeSizeAxes = Axes.Y;
            columns.Width = 0;
        }
        else
        {
            leftIndicator.FadeOut(fade_duration);
            rightIndicator.FadeOut(fade_duration);
            columns.RelativeSizeAxes = Axes.Both;
            columns.Width = 1f - 2 * indicator_width;
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

    private void onIndicatorClick()
    {
        if (allColumnsHidden && canInteract)
            points.ShowPoint(Event);
    }
    
    private void expandIndicator()
    {
        if (!canInteract || !allColumnsHidden) return;
        
        rightIndicator.Expand();
        leftIndicator.Expand();
    }

    private void retractIndicator()
    {
        if (!allColumnsHidden) return;

        rightIndicator.Retract();
        leftIndicator.Retract();
    }

    private void fadeAll(float newAlpha, double duration = 0, Easing easing = Easing.None)
    {
        foreach (var column in columns.Children.OfType<Column>().Where(c => c.Alpha > hidden_alpha))
            column.FadeTo(newAlpha, duration, easing);
    }

    private void highlightAny(bool shouldHighlight)
    {
        if (allColumnsHidden)
        {
            if (shouldHighlight)
                expandIndicator();
            else
                retractIndicator();
        }
        else
        {
            fadeAll(shouldHighlight ? hovered_alpha : visible_alpha, fade_duration);
        }
    }

    private void updateHightlight()
    {
        bool isHovered = columns.Children.OfType<Column>().Any(c => c.IsActuallyHovering) || anyIndicatorHovered;
        
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
            Colour = defaultColor;
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

    private partial class SwitchIndicator : FillFlowContainer
    {
        private bool rightSide;
        private readonly Action onHovered;
        private readonly Action onHoverLost;
        public Action Action;

        private Box box;
        private StripeBackground stripes;

        public SwitchIndicator(bool RightSide, Action onHovered, Action onHoverLost)
        {
            this.onHovered = onHovered;
            this.onHoverLost = onHoverLost;
            rightSide = RightSide;

            RelativeSizeAxes = Axes.Y;
            Direction = FillDirection.Vertical;
            Masking = true;

            InternalChild = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                Children = new Drawable[]
                {
                    box = new Box { RelativeSizeAxes = Axes.Both, Colour = defaultColor, Alpha = ind_visible_alpha },
                    stripes = new StripeBackground()
                    {
                        RelativeSizeAxes = Axes.Both,
                        Scale = rightSide ? new Vector2(1, 1) : new Vector2(-1, 1),
                        StripesColor = defaultColor.Opacity(visible_alpha),
                        Angle = 25,
                        Thickness = 20
                    }
                }
            };
        }

        public void Expand()
        {
            box.FadeTo(ind_hovered_alpha, 100);
            this.ResizeWidthTo(expanded_indicator_width, 100, Easing.OutQuint);
        }

        public void Retract()
        {
            box.FadeTo(ind_visible_alpha, 100);
            this.ResizeWidthTo(indicator_width, 100, Easing.OutQuint);
        }

        protected override bool OnHover(HoverEvent e)
        {
            base.OnHover(e);
            onHovered?.Invoke();

            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            base.OnHoverLost(e);

            onHoverLost?.Invoke();
        }

        protected override bool OnClick(ClickEvent e)
        {
            base.OnClick(e);
            Action?.Invoke();
            return true;
        }
    }

    private enum StateChange
    {
        TheSame,
        NowShowing,
        NowHiding
    }
}
