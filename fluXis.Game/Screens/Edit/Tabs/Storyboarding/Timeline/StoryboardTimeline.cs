using System;
using System.Linq;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Screens.Edit.Tabs.Storyboarding.Timeline.Blueprints;
using fluXis.Game.Screens.Edit.Tabs.Storyboarding.Timeline.Elements;
using fluXis.Game.Storyboards;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
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

    private Storyboard storyboard => map.Storyboard;

    private DependencyContainer dependencies;
    private Container<TimelineElement> elementContainer;

    private float zoom = 2;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 400;

        dependencies.CacheAs(storyboard);
        dependencies.CacheAs(this);
        dependencies.CacheAs<ITimePositionProvider>(this);

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
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
                    new TimelineBlueprintContainer(),
                }
            },
            new Box
            {
                Width = 4,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            new DragHandle(y => Height = Math.Clamp(Height -= y, min_height, max_height))
        };
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    protected override void LoadComplete()
    {
        base.LoadComplete();

        foreach (var element in storyboard.Elements)
        {
            elementContainer.Add(new TimelineElement(element));
        }
    }

    public Drawable GetDrawable(StoryboardElement element)
        => elementContainer.FirstOrDefault(e => e.Element == element);

    public float PositionAtTime(double time) => (float)(DrawWidth / 2 + .5f * ((time - clock.CurrentTime) * zoom));
    public float PositionAtZ(long index) => index * 48;

    public Vector2 ScreenSpacePositionAtTime(double time, int z)
        => ToScreenSpace(new Vector2(PositionAtTime(time), PositionAtZ(z) + 8));

    public double TimeAtPosition(float x) => (x - DrawWidth / 2) * 2 / zoom + clock.CurrentTime;

    public double TimeAtScreenSpacePosition(Vector2 pos) => TimeAtPosition(ToLocalSpace(pos).X);

    protected override bool OnScroll(ScrollEvent e)
    {
        var scroll = e.ShiftPressed ? e.ScrollDelta.X : e.ScrollDelta.Y;
        var delta = scroll > 0 ? 1 : -1;

        if (e.ControlPressed)
        {
            var newZoom = Math.Clamp(zoom += delta * .1f, 1f, 5f);
            this.TransformTo(nameof(zoom), newZoom, 400, Easing.OutQuint);
        }

        return false;
    }

    protected override bool OnClick(ClickEvent e)
    {
        Logger.Log($"{TimeAtPosition(e.MousePosition.X)}");
        return base.OnClick(e);
    }

    private partial class DragHandle : Box
    {
        private Action<float> dragged { get; }

        public DragHandle(Action<float> dragged)
        {
            RelativeSizeAxes = Axes.X;
            Height = 8;
            Colour = FluXisColors.Background4;
            this.dragged = dragged;
        }

        protected override bool OnDragStart(DragStartEvent e) => e.Button == MouseButton.Left;

        protected override void OnDrag(DragEvent e)
        {
            dragged?.Invoke(e.Delta.Y);
        }
    }
}
