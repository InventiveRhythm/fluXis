using fluXis.Game.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.HUD.Judgement;

public partial class JudgementCounterItem : Container
{
    public const int SIZE = 50;
    public const int MARGIN = 5;

    private readonly Performance performance;
    private readonly Judgements judgement;
    private int count;

    private Box background;
    private Box line;
    private SpriteText text;

    public JudgementCounterItem(Performance performance, Judgements judgement)
    {
        this.performance = performance;
        this.judgement = judgement;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(SIZE);
        Anchor = Anchor.TopRight;
        Origin = Anchor.CentreRight;
        CornerRadius = 5;
        Masking = true;

        HitWindow jud = HitWindow.FromKey(judgement);

        Children = new Drawable[]
        {
            background = new Box
            {
                Colour = jud.Color,
                RelativeSizeAxes = Axes.Both,
                Alpha = .2f
            },
            line = new Box
            {
                Colour = jud.Color,
                RelativeSizeAxes = Axes.Y,
                Width = 5,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight
            },
            text = new SpriteText
            {
                Font = new FontUsage("Quicksand", 24, "SemiBold"),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = Colour4.White,
                Y = -2
            }
        };
    }

    protected override void Update()
    {
        if (performance.Judgements.ContainsKey(judgement))
        {
            if (performance.Judgements[judgement] != count)
            {
                count = performance.Judgements[judgement];
                lightUp();
            }
        }
        else
        {
            count = 0;
        }

        text.Text = count.ToString();

        base.Update();
    }

    private void lightUp()
    {
        HitWindow jud = HitWindow.FromKey(judgement);

        background.FadeTo(.5f)
                  .FadeTo(.1f, 200);

        line.FadeColour(Colour4.White)
            .FadeColour(jud.Color, 200);
    }
}
