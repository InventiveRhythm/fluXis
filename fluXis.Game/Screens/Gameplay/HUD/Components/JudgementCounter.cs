using System.Linq;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Scoring;
using fluXis.Game.Scoring.Processing;
using fluXis.Game.Skinning;
using fluXis.Shared.Scoring.Enums;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Gameplay.HUD.Components;

public partial class JudgementCounter : GameplayHUDComponent
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 50;
        AutoSizeAxes = Axes.Y;
        CornerRadius = 10;
        Masking = true;
        EdgeEffect = FluXisStyles.ShadowSmall;

        Children = new Drawable[]
        {
            new Box
            {
                Colour = FluXisColors.Background4,
                RelativeSizeAxes = Axes.Both
            },
            new FillFlowContainer<CounterItem>
            {
                Direction = FillDirection.Vertical,
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Children = Screen.HitWindows.GetTimings().Select(t => new CounterItem(Screen.ScoreProcessor, t)).ToArray()
            }
        };
    }

    private partial class CounterItem : Container
    {
        private readonly ScoreProcessor scoreProcessor;
        private readonly Timing timing;

        private int count;

        private Box background;
        private FluXisSpriteText text;

        public CounterItem(ScoreProcessor scoreProcessor, Timing timing)
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
                case Judgement.Miss:
                    return scoreProcessor.Miss;

                case Judgement.Okay:
                    return scoreProcessor.Okay;

                case Judgement.Alright:
                    return scoreProcessor.Alright;

                case Judgement.Great:
                    return scoreProcessor.Great;

                case Judgement.Perfect:
                    return scoreProcessor.Perfect;

                case Judgement.Flawless:
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
                    Judgement.Flawless => "FL",
                    Judgement.Perfect => "PF",
                    Judgement.Great => "GR",
                    Judgement.Alright => "AL",
                    Judgement.Okay => "OK",
                    Judgement.Miss => "MS",
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
}
