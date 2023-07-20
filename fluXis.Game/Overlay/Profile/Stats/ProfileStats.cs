using fluXis.Game.Graphics;
using fluXis.Game.Online.API.Users;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Overlay.Profile.Stats;

public partial class ProfileStats : Container
{
    private UserStat globalRank;
    private UserStat countryRank;
    private UserStat overallRating;
    private UserStat potentialRating;
    private UserStat accuracy;
    private UserStat rankedScore;
    private UserStat maxCombo;

    private APIUser user;

    public APIUser User
    {
        get => user;
        set
        {
            user = value;

            if (!IsLoaded) return;

            globalRank.Value = value.GlobalRank == 0 ? "#---" : $"#{value.GlobalRank}";
            countryRank.Value = value.CountryRank == 0 ? "#---" : $"#{value.CountryRank}";
            overallRating.Value = value.OverallRating == 0 ? "--.--" : value.OverallRating.ToStringInvariant("N2");
            potentialRating.Value = value.PotentialRating == 0 ? "--.--" : value.PotentialRating.ToStringInvariant("N2");
            accuracy.Value = value.OverallAccuracy == 0 ? "--.--%" : $"{value.OverallAccuracy.ToStringInvariant("P2").Replace(" ", "")}";
            rankedScore.Value = value.RankedScore == 0 ? "---,---" : value.RankedScore.ToString("N0");
            maxCombo.Value = value.MaxCombo == 0 ? "---x" : value.MaxCombo.ToString("N0") + "x";
        }
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding { Vertical = 10 },
                Spacing = new Vector2(20),
                Direction = FillDirection.Vertical,
                Children = new FillFlowContainer[]
                {
                    new()
                    {
                        Direction = FillDirection.Horizontal,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        AutoSizeAxes = Axes.Both,
                        Spacing = new Vector2(20),
                        Children = new[]
                        {
                            globalRank = new UserStat { Title = "Global Rank" },
                            countryRank = new UserStat { Title = "Country Rank" }
                        }
                    },
                    new()
                    {
                        Direction = FillDirection.Horizontal,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        AutoSizeAxes = Axes.Both,
                        Spacing = new Vector2(20),
                        Children = new[]
                        {
                            overallRating = new UserStat { Title = "Overall Rating" },
                            potentialRating = new UserStat { Title = "Potential Rating" },
                            accuracy = new UserStat { Title = "Accuracy" }
                        }
                    },
                    new()
                    {
                        Direction = FillDirection.Horizontal,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        AutoSizeAxes = Axes.Both,
                        Spacing = new Vector2(20),
                        Children = new[]
                        {
                            rankedScore = new UserStat { Title = "Ranked Score" },
                            maxCombo = new UserStat { Title = "Max Combo" }
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        User = user;
    }

    private partial class UserStat : FillFlowContainer
    {
        private string value;
        public string Title { get; init; }

        public string Value
        {
            get => value;
            set
            {
                this.value = value;
                if (IsLoaded) valueText.Text = value;
            }
        }

        private FluXisSpriteText valueText;

        [BackgroundDependencyLoader]
        private void load()
        {
            AutoSizeAxes = Axes.Y;
            Direction = FillDirection.Vertical;
            Width = 200;

            InternalChildren = new[]
            {
                new()
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Text = Title,
                    FontSize = 24
                },
                valueText = new FluXisSpriteText
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Text = Value,
                    FontSize = 24,
                    Colour = FluXisColors.Text2
                }
            };
        }
    }
}
