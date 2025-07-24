using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Scoring;
using fluXis.Scoring.Enums;
using fluXis.Scoring.Processing;
using fluXis.Scoring.Structs;
using fluXis.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Screens.Gameplay.HUD.Components;

public partial class JudgementCounter : GameplayHUDComponent
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 50;
        AutoSizeAxes = Axes.Y;
        CornerRadius = 10;
        Masking = true;
        EdgeEffect = Styling.ShadowSmall;

        var dict = new Dictionary<Judgement, string>
        {
            { Judgement.Flawless, "FL" },
            { Judgement.Perfect, "PF" },
            { Judgement.Great, "GR" },
            { Judgement.Alright, "AL" },
            { Judgement.Okay, "OK" },
            { Judgement.Miss, "MS" },
        };

        Children = new Drawable[]
        {
            new Box
            {
                Colour = Theme.Background4,
                RelativeSizeAxes = Axes.Both
            },
            new FillFlowContainer<CounterItem>
            {
                Direction = FillDirection.Vertical,
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Children = HitWindows.GetTimings().Select(t => new CounterItem(JudgementProcessor, t, dict[t.Judgement])).ToArray()
            }
        };
    }

    private partial class CounterItem : Container
    {
        private JudgementProcessor processor { get; }
        private Timing timing { get; }
        private string defaultText { get; }

        private int count;

        private Box background;
        private FluXisSpriteText text;

        public CounterItem(JudgementProcessor processor, Timing timing, string defaultText)
        {
            this.processor = processor;
            this.timing = timing;
            this.defaultText = defaultText;
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
                    Text = defaultText,
                    Font = FluXisFont.YoureGone,
                    FontSize = 24,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = skinManager.SkinJson.GetColorForJudgement(timing.Judgement)
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            processor.ResultAdded += addResult;
            processor.ResultReverted += revertResult;
        }

        protected override void Dispose(bool isDisposing)
        {
            processor.ResultAdded -= addResult;
            processor.ResultReverted -= revertResult;

            base.Dispose(isDisposing);
        }

        private void addResult(HitResult result)
        {
            if (result.Judgement != timing.Judgement)
                return;

            count++;
            updateText();
            lightUp();
        }

        private void revertResult(HitResult result)
        {
            if (result.Judgement != timing.Judgement)
                return;

            count--;
            updateText();
        }

        private void updateText()
        {
            if (count > 0)
            {
                text.FontSize = count switch
                {
                    > 0 and < 100 => 24,
                    < 1000 => 20,
                    < 10000 => 16,
                    _ => 12 // > 9999
                };
                text.Text = count.ToString();
            }
            else
                text.Text = defaultText;
        }

        private void lightUp()
        {
            background.FadeTo(.25f)
                      .FadeOut(200);
        }
    }
}
