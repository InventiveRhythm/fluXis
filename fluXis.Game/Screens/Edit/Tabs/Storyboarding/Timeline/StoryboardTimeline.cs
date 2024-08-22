using System;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Screens.Edit.Tabs.Storyboarding.Timeline.Blueprints;
using fluXis.Game.Screens.Edit.Tabs.Storyboarding.Timeline.Elements;
using fluXis.Game.Storyboards;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Storyboarding.Timeline;

public partial class StoryboardTimeline : CompositeDrawable, ITimePositionProvider
{
    private const float min_height = 200;
    private const float max_height = 600;

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorSettings settings { get; set; }

    private Storyboard storyboard => map.Storyboard;

    private Container<TimelineElement> elementContainer;
    public TimelineBlueprintContainer Blueprints { get; set; } = new();

    private bool dragging;

    private FluXisSpriteText overlayText;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 400;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            new Box
            {
                Width = 4,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = .5f
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Top = 8 },
                Children = new Drawable[]
                {
                    elementContainer = new Container<TimelineElement>
                    {
                        RelativeSizeAxes = Axes.Both
                    },
                    Blueprints
                }
            },
            overlayText = new FluXisSpriteText
            {
                Y = 48,
                WebFontSize = 16,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Shadow = true
            },
            new Box
            {
                RelativeSizeAxes = Axes.X,
                Height = 4,
                Colour = FluXisColors.Background3
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        storyboard.ElementAdded += add;
        storyboard.ElementRemoved += remove;
        storyboard.Elements.ForEach(add);

        foreach (var element in storyboard.Elements)
        {
            elementContainer.Add(new TimelineElement(element));
        }
    }

    private void add(StoryboardElement element)
    {
        elementContainer.Add(new TimelineElement(element));
    }

    private void remove(StoryboardElement element)
    {
        var drawable = GetDrawable(element);
        elementContainer.Remove(drawable, true);
    }

    public TimelineElement GetDrawable(StoryboardElement element)
        => elementContainer.FirstOrDefault(e => e.Element == element);

    public float PositionAtTime(double time) => (float)(DrawWidth / 2 + .5f * ((time - clock.CurrentTime) * settings.Zoom));
    public float PositionAtZ(long index) => index * 48;

    public Vector2 ScreenSpacePositionAtTime(double time, int z)
        => ToScreenSpace(new Vector2(PositionAtTime(time), PositionAtZ(z) + 8));

    public double TimeAtPosition(float x) => (x - DrawWidth / 2) * 2 / settings.Zoom + clock.CurrentTime;
    public int ZAtPosition(float y) => (int)(y / 48);

    public double TimeAtScreenSpacePosition(Vector2 pos) => TimeAtPosition(ToLocalSpace(pos).X);
    public int ZAtScreenSpacePosition(Vector2 pos) => ZAtPosition(ToLocalSpace(pos).Y);

    protected override bool OnScroll(ScrollEvent e)
    {
        var scroll = e.ShiftPressed ? e.ScrollDelta.X : e.ScrollDelta.Y;
        var delta = scroll > 0 ? 1 : -1;

        if (e.ControlPressed)
        {
            settings.Zoom = Math.Clamp(settings.Zoom += delta * .1f, 1f, 5f);
            overlayText.Text = $"Zoom: {(settings.Zoom / 2f).ToStringInvariant("0.00")}x";
            overlayText.FadeIn().Delay(600).FadeOut(200);
        }

        return false;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (e.Button != MouseButton.Middle)
            return false;

        dragging = true;
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        dragging = false;
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        if (!dragging)
            return false;

        Height = Math.Clamp(Height -= e.Delta.Y, min_height, max_height);
        return true;
    }
}
