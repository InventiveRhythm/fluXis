using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Utils;
using fluXis.Shared.Components.Users;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Overlay.User.Sections;

public partial class ProfileStats : Container
{
    private APIUserStatistics stats { get; }

    public ProfileStats(APIUserStatistics stats)
    {
        this.stats = stats;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 96;
        CornerRadius = 16;
        Masking = true;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                ColumnDimensions = new[]
                {
                    new Dimension(),
                    new Dimension(),
                    new Dimension(),
                    new Dimension(),
                    new Dimension()
                },
                RowDimensions = new[] { new Dimension() },
                Content = new[]
                {
                    new Drawable[]
                    {
                        new ProfileStat
                        {
                            Title = "Overall Rating",
                            Value = stats.OverallRating.ToStringInvariant("0.00")
                        },
                        new ProfileStat
                        {
                            Title = "Potential Rating",
                            Value = stats.PotentialRating.ToStringInvariant("0.00")
                        },
                        new ProfileStat
                        {
                            Title = "Overall Accuracy",
                            Value = stats.OverallAccuracy.ToStringInvariant("00.00") + "%"
                        },
                        new ProfileStat
                        {
                            Title = "Ranked Score",
                            Value = "---,---"
                        },
                        new ProfileStat
                        {
                            Title = "Max Combo",
                            Value = $"{stats.MaxCombo}x"
                        }
                    }
                }
            }
        };
    }

    private partial class ProfileStat : FillFlowContainer
    {
        public string Title { get; init; }
        public string Value { get; init; }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Direction = FillDirection.Vertical;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Spacing = new Vector2(-3);

            InternalChildren = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = Title,
                    Alpha = .8f,
                    WebFontSize = 16,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre
                },
                new FluXisSpriteText
                {
                    Text = Value,
                    WebFontSize = 24,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre
                }
            };
        }
    }
}
