using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.UI;

public partial class SkipOverlay : Container
{
    private GameplayScreen gameplayScreen { get; }

    private bool visible;
    private FillFlowContainer content;
    private CircularContainer bar;

    public SkipOverlay(GameplayScreen gameplayScreen)
    {
        this.gameplayScreen = gameplayScreen;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

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

    protected override void Update()
    {
        base.Update();

        switch (gameplayScreen.Map.StartTime - gameplayScreen.AudioClock.CurrentTime > 2000)
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

        if (!visible) return;

        double timeLeft = gameplayScreen.Map.StartTime - (gameplayScreen.AudioClock.CurrentTime + 2000);
        var progress = (float)(timeLeft / gameplayScreen.Map.StartTime);

        if (float.IsFinite(progress))
            bar.ResizeWidthTo(progress, 400, Easing.OutQuint);
        else
            bar.Width = 0;
    }
}
