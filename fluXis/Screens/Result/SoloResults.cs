using System.Collections.Generic;
using fluXis.Database.Maps;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Requests.Scores;
using fluXis.Scoring;
using fluXis.Screens.Result.Sides.Types;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Screens.Result;

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

    protected override Drawable[] CreateRightContent()
    {
        var list = new List<Drawable>();

        if (Map.OnlineID > 0)
            list.Add(new ResultsSideVoting());

        list.Add(new ResultsSideRankings(SubmitRequest));
        return list.ToArray();
    }
}
