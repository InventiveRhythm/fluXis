using System.Collections.Generic;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Online.Multiplayer;
using fluXis.Scoring;
using fluXis.Screens.Result.Sides;
using fluXis.Screens.Result.Sides.Presets;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Multiplayer.Gameplay;

public partial class MultiplayerResults : Result.Results
{
    private List<ScoreInfo> scores { get; }
    private MultiplayerClient client { get; }

    public MultiplayerResults(RealmMap map, List<ScoreInfo> scores, MultiplayerClient client)
        : base(map, scores.FirstOrDefault(x => x.PlayerID == client.Player.ID), client.Player)
    {
        this.scores = scores;
        this.client = client;
    }

    protected override Drawable[] CreateRightContent() => new Drawable[]
    {
        new Leaderboard(scores, client)
    };

    private partial class Leaderboard : ResultsSideContainer
    {
        protected override LocalisableString Title => "Leaderboard";

        private List<ScoreInfo> scores { get; }
        private MultiplayerClient client { get; }

        public Leaderboard(List<ScoreInfo> scores, MultiplayerClient client)
        {
            this.scores = scores.OrderByDescending(x => x.Score).ToList();
            this.client = client;
        }

        protected override Drawable CreateContent() => new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(16),
            ChildrenEnumerable = scores.Select((x, i) =>
            {
                var player = client.Room.Participants.FirstOrDefault(y => y.ID == x.PlayerID);
                var place = i + 1;
                return new ResultsSideDoubleText($"#{place} {player?.Player.PreferredName ?? "??"}", x.Score.ToString("000000"));
            })
        };
    }
}
