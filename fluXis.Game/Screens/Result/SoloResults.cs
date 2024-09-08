using fluXis.Game.Database.Maps;
using fluXis.Game.Online.API.Requests.Scores;
using fluXis.Game.Screens.Result.Sides.Types;
using fluXis.Shared.Components.Users;
using fluXis.Shared.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Result;

public partial class SoloResults : Results
{
    protected override bool PlayEnterAnimation => true;

    public ScoreSubmitRequest SubmitRequest { get; set; }

    public SoloResults(RealmMap map, ScoreInfo score, APIUser player)
        : base(map, score, player)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        ExtraDependencies.CacheAs(this);
    }

    protected override Drawable[] CreateRightContent() => new Drawable[]
    {
        new ResultsSideRankings(SubmitRequest)
    };
}
