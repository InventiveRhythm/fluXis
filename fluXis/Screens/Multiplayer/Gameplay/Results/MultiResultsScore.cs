using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Text;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Drawables.Images;
using fluXis.Scoring;
using fluXis.Scoring.Enums;
using fluXis.Skinning;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Multiplayer.Gameplay.Results;

public partial class MultiResultsScore : CompositeDrawable
{
    private ScoreInfo score { get; }
    private APIUser user { get; }
    private int rank { get; }

    public MultiResultsScore(ScoreInfo score, APIUser user, int rank)
    {
        this.score = score;
        this.user = user;
        this.rank = rank;
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
                        Child = new FluXisSpriteText
                        {
                            Text = $"#{rank}",
                            WebFontSize = 20,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        }
                    },
                    new LoadWrapper<DrawableAvatar>
                    {
                        Size = new Vector2(60),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        CornerRadius = 10,
                        Masking = true,
                        Margin = new MarginPadding { Right = 7 },
                        OnComplete = a => a.FadeInFromZero(400),
                        LoadContent = () => new DrawableAvatar(user)
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        }
                    },
                    new FluXisSpriteText
                    {
                        Text = user.PreferredName,
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
                //TODO: concider dual maps
                Children = new Drawable[]
                {
                    new TextContainer(score.Players[0].Flawless.ToString()) { Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Flawless) },
                    new TextContainer(score.Players[0].Perfect.ToString()) { Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Perfect) },
                    new TextContainer(score.Players[0].Great.ToString()) { Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Great) },
                    new TextContainer(score.Players[0].Alright.ToString()) { Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Alright) },
                    new TextContainer(score.Players[0].Okay.ToString()) { Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Okay) },
                    new TextContainer(score.Players[0].Miss.ToString()) { Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Miss) },
                    new TextContainer($"{score.Players[0].MaxCombo}x"),
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

            //TODO: concider dual maps
            InternalChildren = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = $"{score.Players[0].Score:0000000}",
                    WebFontSize = 24,
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight
                },
                new FluXisSpriteText
                {
                    Text = $"{score.Players[0].Accuracy.ToStringInvariant("00.00")}%",
                    WebFontSize = 16,
                    Alpha = .8f,
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight
                }
            };
        }
    }
}
