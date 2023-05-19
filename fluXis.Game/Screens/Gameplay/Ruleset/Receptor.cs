using fluXis.Game.Graphics;
using fluXis.Game.Map.Events;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Ruleset;

public partial class Receptor : CompositeDrawable
{
    public static readonly Vector2 SIZE = new(114, 114);
    public readonly float HitPosition = 130;
    public Playfield Playfield;

    private readonly int id;
    private readonly Colour4 color;

    private Container diamond;
    private Box hitLighting;

    private int currentKeyCount;
    private bool visible;

    public bool IsDown;

    public Receptor(Playfield playfield, int id)
    {
        this.id = id;
        Playfield = playfield;
        currentKeyCount = playfield.Map.InitialKeyCount;
        color = FluXisColors.GetLaneColor(id + 1, currentKeyCount);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Origin = Anchor.BottomCentre;
        Anchor = Anchor.BottomCentre;
        Width = SIZE.X;
        RelativeSizeAxes = Axes.Y;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = HitPosition,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Surface
                    },
                    diamond = new Container
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(24, 24),
                        Masking = true,
                        CornerRadius = 5,
                        BorderColour = FluXisColors.Background,
                        BorderThickness = 5,
                        Rotation = 45,
                        Child = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            AlwaysPresent = true,
                            Alpha = 0
                        }
                    }
                }
            },
            hitLighting = new Box
            {
                Colour = ColourInfo.GradientVertical(color.Opacity(0), color),
                Margin = new MarginPadding { Bottom = HitPosition },
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.X,
                Height = 232,
                Alpha = 0
            }
        };

        updateKeyCount(true);
    }

    protected override void Update()
    {
        if (!Playfield.Manager.AutoPlay)
            IsDown = Playfield.Screen.Input.Pressed[id];

        if (visible)
        {
            diamond.BorderColour = IsDown ? color : FluXisColors.Background;
            hitLighting.FadeTo(IsDown ? 0.5f : 0, IsDown ? 0 : 100);
        }

        if (currentKeyCount != Playfield.Manager.CurrentKeyCount)
        {
            currentKeyCount = Playfield.Manager.CurrentKeyCount;
            updateKeyCount(false);
        }

        base.Update();
    }

    private void updateKeyCount(bool instant)
    {
        // Since the current count is the same as the maximum,
        // every receptor should be visible
        if (currentKeyCount == Playfield.Map.KeyCount)
            visible = true;
        else
        {
            bool[][] mode = LaneSwitchEvent.SWITCH_VISIBILITY[Playfield.Map.KeyCount - 5];
            bool[] current = mode[currentKeyCount - 4];
            visible = current[id];
        }

        if (instant)
        {
            this.ResizeWidthTo(visible ? SIZE.X : 0);
        }
        else
        {
            float duration = Playfield.Manager.CurrentLaneSwitchEvent?.Speed ?? 200;
            this.ResizeWidthTo(visible ? SIZE.X : 0, duration, Easing.OutQuint);
        }
    }
}
