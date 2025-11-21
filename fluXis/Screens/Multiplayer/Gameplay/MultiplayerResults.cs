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
        : base(map, scores.FirstOrDefault(s => s.Players.Any(p => p.PlayerID == client.Player.ID)))
    {
        this.scores = scores;
        this.client = client;

        //if we are playing a dual map, we want to dipslay the stats of the side in which the client's user players
        SelectedPlayer.Value = Score.Players.IndexOf(Score.Players.FirstOrDefault(x => x.PlayerID == client.Player.ID));
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
            //TODO: assume player 0 for now, needs more work for dual maps (maybe use sum of both sides)
            this.scores = scores.OrderByDescending(x => x.Players[0].Score).ToList();
            this.client = client;
        }

        protected override Drawable CreateContent() => new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(16),
            //TODO: assume player 0 for now, needs more work for dual maps
            ChildrenEnumerable = scores.Select((x, i) =>
            {
                var player = client.Room.Participants.FirstOrDefault(y => y.ID == x.Players[0].PlayerID); //find a way to display both names for dual maps
                var place = i + 1;
                return new ResultsSideDoubleText($"#{place} {player?.Player.PreferredName ?? "??"}", x.Players[0].Score.ToString("000000")); //display sum for dual maps?
            })
        };
    }
}
