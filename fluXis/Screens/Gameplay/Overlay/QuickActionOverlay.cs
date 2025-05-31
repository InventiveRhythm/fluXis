using System;
using fluXis.Configuration;
using fluXis.Graphics.Sprites.Text;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Gameplay.Overlay;

public partial class QuickActionOverlay : FillFlowContainer
{
    private bool isHolding;
    private double holdTime;
    private Bindable<float> holdToConfirm;
    private bool activated;
    private Box bar;

    public bool IsHolding
    {
        set
        {
            if (holdToConfirm.Value == 0 && value)
            {
                OnConfirm?.Invoke();
                return;
            }

            isHolding = value;

            if (value)
            {
                ClearTransforms();
                this.FadeIn(200);
            }
            else
            {
                this.TransformTo(nameof(holdTime), 0d, 400, Easing.OutCirc);
                this.FadeOut(400);
            }
        }
    }

    public Action OnConfirm { get; set; }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        holdToConfirm = config.GetBindable<float>(FluXisSetting.HoldToConfirm);

        AutoSizeAxes = Axes.Both;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(0, 5);
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Alpha = 0;
        Y = -200;

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = "Keep holding...",
                FontSize = 40,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre
            },
            new CircularContainer
            {
                Width = 400,
                Height = 10,
                Masking = true,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Children = new[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = .2f,
                        Blending = BlendingParameters.Additive
                    },
                    bar = new Box
                    {
                        RelativeSizeAxes = Axes.Both
                    }
                }
            }
        };
    }

    protected override void Update()
    {
        if (isHolding) holdTime += Time.Elapsed;
        bar.Width = (float)(holdTime / holdToConfirm.Value);

        if (holdTime >= holdToConfirm.Value && !activated)
        {
            activated = true;
            OnConfirm.Invoke();
        }
    }
}
