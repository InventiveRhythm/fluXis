using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API.Requests.Scores;
using fluXis.Online.API.Responses.Scores;
using fluXis.Utils;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Result.Sides.Types;

public partial class ResultsSideRankings : ResultsSideContainer
{
    protected override LocalisableString Title => "Rankings";

    private ScoreSubmitRequest request { get; }

    public ResultsSideRankings(ScoreSubmitRequest request)
    {
        this.request = request;

        if (request is null)
            Alpha = 0;
    }

    protected override Drawable CreateContent()
    {
        if (request is null)
            return Empty();

        if (!request.IsSuccessful)
        {
            return new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 50,
                Child = new TruncatingText
                {
                    Text = request.FailReason?.Message ?? "Something went wrong...",
                    WebFontSize = 16,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    MaxWidth = 380
                }
            };
        }

        return new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            Height = 50,
            Direction = FillDirection.Horizontal,
            Children = new Drawable[]
            {
                new Statistic("Overall Rating", request.Response.Data.OverallRating),
                new Statistic("Potential Rating", request.Response.Data.PotentialRating),
                new Statistic("Rank", request.Response.Data.Rank, true)
            }
        };
    }

    private partial class Statistic : FillFlowContainer
    {
        public Statistic(string title, ScoreSubmissionStats.StatisticChange change, bool asRank = false)
        {
            RelativeSizeAxes = Axes.Both;
            Direction = FillDirection.Vertical;
            Spacing = new Vector2(8);
            Width = 1f / 3;

            var diff = change.Current - change.Previous;
            var value = asRank ? $"#{(int)change.Current}" : change.Current.ToStringInvariant("00.00");
            var difference = "KEEP";
            var color = Theme.Text.Opacity(0.6f);

            var negativeColor = Colour4.FromHex("#FF5555");
            var positiveColor = Colour4.FromHex("#55FF55");

            switch (diff)
            {
                case > 0:
                    difference = asRank ? $"+{diff}" : $"+{diff.ToStringInvariant("00.00")}";
                    color = asRank ? negativeColor : positiveColor;
                    break;

                case < 0:
                    difference = asRank ? $"{diff}" : $"{diff.ToStringInvariant("00.00")}";
                    color = asRank ? positiveColor : negativeColor;
                    break;
            }

            InternalChildren = new Drawable[]
            {
                new ForcedHeightText
                {
                    Text = title,
                    WebFontSize = 12,
                    Height = 10,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = .8f
                },
                new FluXisSpriteText
                {
                    Text = value,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    UseFullGlyphHeight = false,
                    WebFontSize = 20
                },
                new FluXisSpriteText
                {
                    Text = difference,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    UseFullGlyphHeight = false,
                    Colour = color,
                    WebFontSize = 14
                }
            };
        }
    }
}

