using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Skinning;
using fluXis.Game.Utils;
using fluXis.Shared.Scoring;
using fluXis.Shared.Scoring.Enums;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Result.Extended;

public partial class ExtendedResultsInfo : CompositeDrawable
{
    [Resolved]
    private ScoreInfo score { get; set; }

    [BackgroundDependencyLoader]
    private void load(SkinManager skinManager)
    {
        RelativeSizeAxes = Axes.Both;
        CornerRadius = 20;
        Masking = true;
        EdgeEffect = FluXisStyles.ShadowMedium;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(40),
                Spacing = new Vector2(20),
                Children = new Drawable[]
                {
                    new Entry("Accuracy", score.Accuracy.ToStringInvariant("0.00") + "%"),
                    new Entry("Score", score.Score.ToString("N0")),
                    new Entry("Max Combo", $"{score.MaxCombo}x"),
                    // new Entry("Performance Rating", "0pr"),
                    new Entry("Flawless Ratio", getRatio()),
                    // new Entry("Raw Score", "1000000"),
                    new Entry("Flawless", $"{score.Flawless}") { Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Flawless) },
                    new Entry("Perfect", $"{score.Perfect}") { Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Perfect) },
                    new Entry("Great", $"{score.Great}") { Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Great) },
                    new Entry("Alright", $"{score.Alright}") { Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Alright) },
                    new Entry("Okay", $"{score.Okay}") { Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Okay) },
                    new Entry("Miss", $"{score.Miss}") { Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Miss) }
                }
            }
        };
    }

    private string getRatio()
    {
        return score.Flawless switch
        {
            0 => "0",
            > 0 when score.Perfect == 0 => "Full Flawless",
            _ => $"{(score.Flawless / (float)score.Perfect).ToStringInvariant("0.0")}:1"
        };
    }

    private partial class Entry : CompositeDrawable
    {
        public Entry(string title, string content)
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;

            InternalChildren = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = title,
                    WebFontSize = 20,
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft
                },
                new FluXisSpriteText
                {
                    Text = content,
                    WebFontSize = 20,
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight
                }
            };
        }
    }
}
