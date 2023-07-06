using fluXis.Game.Graphics;
using fluXis.Game.Scoring;
using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Gameplay.HUD.Judgement;

public partial class JudgementCounterItem : Container
{
    private readonly Performance performance;
    private readonly Scoring.Judgement judgement;

    private HitWindow hitWindow;
    private int count;

    private Box background;
    private FluXisSpriteText text;

    public JudgementCounterItem(Performance performance, Scoring.Judgement judgement)
    {
        this.performance = performance;
        this.judgement = judgement;
    }

    [BackgroundDependencyLoader]
    private void load(SkinManager skinManager)
    {
        RelativeSizeAxes = Axes.X;
        Height = 50;

        hitWindow = HitWindow.FromKey(judgement);

        Children = new Drawable[]
        {
            background = new Box
            {
                Colour = skinManager.CurrentSkin.GetColorForJudgement(hitWindow.Key),
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            text = new FluXisSpriteText
            {
                Font = FluXisFont.YoureGone,
                FontSize = 24,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = skinManager.CurrentSkin.GetColorForJudgement(hitWindow.Key)
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
                text.FontSize = count switch
                {
                    > 0 and < 100 => 24,
                    > 99 and < 1000 => 20,
                    > 999 and < 10000 => 16,
                    _ => 12 // > 9999
                };
            }

            text.Text = count.ToString();
        }
        else
        {
            text.Text = judgement switch
            {
                Scoring.Judgement.Flawless => "FL",
                Scoring.Judgement.Perfect => "PF",
                Scoring.Judgement.Great => "GR",
                Scoring.Judgement.Alright => "AL",
                Scoring.Judgement.Okay => "OK",
                Scoring.Judgement.Miss => "MS",
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
