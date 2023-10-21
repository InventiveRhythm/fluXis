using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Scoring;
using fluXis.Game.Scoring.Processing;
using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Gameplay.HUD.Judgement;

public partial class JudgementCounterItem : Container
{
    private readonly ScoreProcessor scoreProcessor;
    private readonly Timing timing;

    private int count;

    private Box background;
    private FluXisSpriteText text;

    public JudgementCounterItem(ScoreProcessor scoreProcessor, Timing timing)
    {
        this.scoreProcessor = scoreProcessor;
        this.timing = timing;
    }

    [BackgroundDependencyLoader]
    private void load(SkinManager skinManager)
    {
        RelativeSizeAxes = Axes.X;
        Height = 50;

        Children = new Drawable[]
        {
            background = new Box
            {
                Colour = skinManager.SkinJson.GetColorForJudgement(timing.Judgement),
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            text = new FluXisSpriteText
            {
                Font = FluXisFont.YoureGone,
                FontSize = 24,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = skinManager.SkinJson.GetColorForJudgement(timing.Judgement)
            }
        };
    }

    private int getCount()
    {
        switch (timing.Judgement)
        {
            case Scoring.Enums.Judgement.Miss:
                return scoreProcessor.Miss;

            case Scoring.Enums.Judgement.Okay:
                return scoreProcessor.Okay;

            case Scoring.Enums.Judgement.Alright:
                return scoreProcessor.Alright;

            case Scoring.Enums.Judgement.Great:
                return scoreProcessor.Great;

            case Scoring.Enums.Judgement.Perfect:
                return scoreProcessor.Perfect;

            case Scoring.Enums.Judgement.Flawless:
                return scoreProcessor.Flawless;

            default:
                return 0;
        }
    }

    protected override void Update()
    {
        var actualCount = getCount();

        if (actualCount != 0)
        {
            if (actualCount != count)
            {
                count = actualCount;
                lightUp();
                text.FontSize = count switch
                {
                    > 0 and < 100 => 24,
                    < 1000 => 20,
                    < 10000 => 16,
                    _ => 12 // > 9999
                };
            }

            text.Text = count.ToString();
        }
        else
        {
            text.Text = timing.Judgement switch
            {
                Scoring.Enums.Judgement.Flawless => "FL",
                Scoring.Enums.Judgement.Perfect => "PF",
                Scoring.Enums.Judgement.Great => "GR",
                Scoring.Enums.Judgement.Alright => "AL",
                Scoring.Enums.Judgement.Okay => "OK",
                Scoring.Enums.Judgement.Miss => "MS",
                _ => "??"
            };
        }

        base.Update();
    }

    private void lightUp()
    {
        background.FadeTo(.25f)
                  .FadeOut(200);
    }
}
