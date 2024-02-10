using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Scoring;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Skinning;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Multiplayer.Gameplay.Results;

public partial class MultiResultsScore : CompositeDrawable
{
    private ScoreInfo score { get; }
    private APIUserShort user { get; }

    public MultiResultsScore(ScoreInfo score, APIUserShort user)
    {
        this.score = score;
        this.user = user;
    }

    [BackgroundDependencyLoader]
    private void load(SkinManager skinManager)
    {
        RelativeSizeAxes = Axes.X;
        Height = 60;

        InternalChildren = new Drawable[]
        {
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(8),
                Children = new Drawable[]
                {
                    new Container
                    {
                        Size = new Vector2(60),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        CornerRadius = 10,
                        Masking = true,
                        Margin = new MarginPadding { Right = 7 },
                        Child = new DrawableAvatar(user)
                        {
                            RelativeSizeAxes = Axes.Both,
                            FillMode = FillMode.Fill,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        }
                    },
                    new FluXisSpriteText
                    {
                        Text = user.GetName(),
                        WebFontSize = 24,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft
                    },
                    new FluXisSpriteText
                    {
                        Text = user.Username,
                        Alpha = string.IsNullOrEmpty(user.DisplayName) ? 0 : .8f,
                        WebFontSize = 16,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft
                    }
                }
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(20),
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Children = new Drawable[]
                {
                    new TextContainer(score.Flawless.ToString()) { Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Flawless) },
                    new TextContainer(score.Perfect.ToString()) { Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Perfect) },
                    new TextContainer(score.Great.ToString()) { Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Great) },
                    new TextContainer(score.Alright.ToString()) { Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Alright) },
                    new TextContainer(score.Okay.ToString()) { Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Okay) },
                    new TextContainer(score.Miss.ToString()) { Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Miss) },
                    new TextContainer($"{score.MaxCombo}x"),
                    new ScoreAcc(score)
                }
            }
        };
    }

    private partial class TextContainer : CompositeDrawable
    {
        public TextContainer(string text)
        {
            AutoSizeAxes = Axes.X;
            RelativeSizeAxes = Axes.Y;

            InternalChildren = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = text,
                    WebFontSize = 24,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            };
        }
    }

    private partial class ScoreAcc : FillFlowContainer
    {
        public ScoreAcc(ScoreInfo score)
        {
            AutoSizeAxes = Axes.X;
            RelativeSizeAxes = Axes.Y;
            Direction = FillDirection.Vertical;
            Spacing = new Vector2(-8);

            InternalChildren = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = $"{score.Score:0000000}",
                    WebFontSize = 24,
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight
                },
                new FluXisSpriteText
                {
                    Text = $"{score.Accuracy.ToStringInvariant("00.00")}%",
                    WebFontSize = 16,
                    Alpha = .8f,
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight
                }
            };
        }
    }
}
