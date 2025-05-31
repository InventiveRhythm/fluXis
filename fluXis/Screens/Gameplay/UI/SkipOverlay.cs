using fluXis.Graphics.Sprites.Text;
using fluXis.Screens.Gameplay.Audio;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Gameplay.UI;

public partial class SkipOverlay : Container
{
    [Resolved]
    private GameplayScreen screen { get; set; }

    [Resolved]
    private GameplayClock clock { get; set; }

    private bool visible;
    private FillFlowContainer content;
    private CircularContainer bar;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        AlwaysPresent = true;

        InternalChild = content = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            RelativePositionAxes = Axes.Both,
            Y = .15f,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(3),
            Alpha = 0,
            Children = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Text = "Skip",
                    FontSize = 32,
                    Shadow = true
                },
                new CircularContainer
                {
                    Width = 200,
                    Height = 10,
                    Masking = true,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Alpha = .2f,
                            Blending = BlendingParameters.Additive
                        },
                        bar = new CircularContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Masking = true,
                            Child = new Box { RelativeSizeAxes = Axes.Both }
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        clock.OnBeat += onBeat;
    }

    private void onBeat(int beat)
    {
        if (!visible) return;

        double timeLeft = screen.Map.StartTime - (clock.CurrentTime + 2000);
        var progress = (float)(timeLeft / screen.Map.StartTime);

        if (float.IsFinite(progress))
            bar.ResizeWidthTo(progress, clock.BeatTime, Easing.OutQuint);
        else
            bar.Width = 0;
    }

    protected override void Update()
    {
        base.Update();

        switch (screen.Map.StartTime - clock.CurrentTime > 2000)
        {
            case true when !visible:
                visible = true;
                content.FadeIn(200);
                break;

            case false when visible:
                visible = false;
                content.FadeOut(200);
                break;
        }
    }
}
