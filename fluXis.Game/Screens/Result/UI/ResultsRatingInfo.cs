using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Online.API;
using fluXis.Game.Utils;
using fluXis.Shared.API.Responses.Scores;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Result.UI;

public partial class ResultsRatingInfo : Container
{
    private readonly bool showPlayData;

    public APIResponse<ScoreSubmissionStats> ScoreResponse
    {
        set => Schedule(() =>
        {
            if (!showPlayData)
                return;

            loadingIcon.FadeOut(200);
            ratingContainer.FadeOut();
            text.FadeOut();

            if (!value.Success)
            {
                text.Text = value.Message;
                text.FadeIn(200);
                return;
            }

            var ovrc = value.Data.OverallRatingChange;
            var pvrc = value.Data.PotentialRatingChange;

            ovr.Text = value.Data.OverallRating.ToStringInvariant();
            pvr.Text = value.Data.PotentialRating.ToStringInvariant();

            switch (ovrc)
            {
                case > 0:
                    ovrChange.Colour = Colour4.FromHex("#55FF55");
                    ovrChange.Text = "+" + ovrc.ToStringInvariant("0.00");
                    break;

                case < 0:
                    ovrChange.Colour = Colour4.FromHex("#FF5555");
                    ovrChange.Text = ovrc.ToStringInvariant("0.00");
                    break;

                default:
                    ovrChange.Colour = FluXisColors.Text2;
                    ovrChange.Text = "SAME";
                    break;
            }

            switch (pvrc)
            {
                case > 0:
                    pvrChange.Colour = Colour4.FromHex("#55FF55");
                    pvrChange.Text = "+" + pvrc.ToStringInvariant("0.00");
                    break;

                case < 0:
                    pvrChange.Colour = Colour4.FromHex("#FF5555");
                    pvrChange.Text = pvrc.ToStringInvariant("0.00");
                    break;

                default:
                    pvrChange.Colour = FluXisColors.Text2;
                    pvrChange.Text = "KEEP";
                    break;
            }

            ratingContainer.FadeIn(200);
        });
    }

    private FluXisSpriteText text;
    private LoadingIcon loadingIcon;
    private FillFlowContainer ratingContainer;
    private FluXisSpriteText ovr;
    private FluXisSpriteText ovrChange;
    private FluXisSpriteText pvr;
    private FluXisSpriteText pvrChange;

    public ResultsRatingInfo(bool showPlayData)
    {
        this.showPlayData = showPlayData;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 80;

        Children = new Drawable[]
        {
            text = new FluXisSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Text = showPlayData ? "" : "Rating data not available!",
                Alpha = showPlayData ? 0 : 1
            },
            loadingIcon = new LoadingIcon
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(40),
                Alpha = showPlayData ? 1 : 0
            },
            ratingContainer = new FillFlowContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(10),
                Alpha = 0,
                Children = new Drawable[]
                {
                    new Container
                    {
                        Width = 200,
                        AutoSizeAxes = Axes.Y,
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Text = "Overall Rating",
                                FontSize = 14,
                                Colour = FluXisColors.Text2
                            },
                            ovr = new FluXisSpriteText
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Text = "0.00",
                                FontSize = 32,
                                Y = 10
                            },
                            ovrChange = new FluXisSpriteText
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Text = "",
                                FontSize = 20,
                                Y = 38
                            }
                        }
                    },
                    new Container
                    {
                        Width = 200,
                        AutoSizeAxes = Axes.Y,
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Text = "Potential Rating",
                                FontSize = 14,
                                Colour = FluXisColors.Text2
                            },
                            pvr = new FluXisSpriteText
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Text = "0.00",
                                FontSize = 32,
                                Y = 10
                            },
                            pvrChange = new FluXisSpriteText
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Text = "",
                                FontSize = 20,
                                Y = 38
                            }
                        }
                    }
                }
            }
        };
    }
}
