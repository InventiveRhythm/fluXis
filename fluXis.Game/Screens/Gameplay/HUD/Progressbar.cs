using fluXis.Game.Audio;
using fluXis.Game.Graphics;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Gameplay.HUD;

public partial class Progressbar : GameplayHUDElement
{
    [Resolved]
    private AudioClock clock { get; set; }

    public Progressbar(GameplayScreen screen)
        : base(screen)
    {
    }

    private Box bar;
    private FluXisSpriteText currentTimeText;
    private FluXisSpriteText percentText;
    private FluXisSpriteText timeLeftText;

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.TopLeft;
        Origin = Anchor.TopLeft;
        RelativeSizeAxes = Axes.X;
        Padding = new MarginPadding { Top = 10, Left = 10, Right = 10 };

        InternalChildren = new Drawable[]
        {
            new Container
            {
                CornerRadius = 5,
                Masking = true,
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Height = 10,
                RelativeSizeAxes = Axes.X,
                Children = new Drawable[]
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
            },
            currentTimeText = new FluXisSpriteText
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Y = 10,
                X = 10,
                FontSize = 32
            },
            percentText = new FluXisSpriteText
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Y = 10,
                FontSize = 32
            },
            timeLeftText = new FluXisSpriteText
            {
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Y = 10,
                X = -10,
                FontSize = 32
            }
        };
    }

    protected override void Update()
    {
        double speed = clock.Rate == 0 ? 1 : clock.Rate;
        int currentTime = (int)((clock.CurrentTime - Screen.Map.StartTime) / speed);
        int timeLeft = (int)((Screen.Map.EndTime - clock.CurrentTime) / speed);
        int totalTime = (int)((Screen.Map.EndTime - Screen.Map.StartTime) / speed);
        float percent = (float)currentTime / totalTime;
        if (percent < 0) percent = 0;

        if (Screen.Map.StartTime == Screen.Map.EndTime)
            percent = 1;

        bar.Width = percent;
        percentText.Text = $"{(int)(percent * 100)}%";
        currentTimeText.Text = TimeUtils.Format(currentTime, false);
        timeLeftText.Text = TimeUtils.Format(timeLeft, false);

        base.Update();
    }
}
