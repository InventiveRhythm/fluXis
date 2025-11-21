using fluXis.Database.Score;

namespace fluXis.Scoring;

//this is needed by ScoreListTab in order to actually obtain the PlayerScoreInfos for each scores
public class ScorePair
{
    public ScoreInfo ScoreInfo;
    public RealmScore RealmScore;

    public ScorePair(ScoreInfo scoreInfo, RealmScore realmScore)
    {
        ScoreInfo = scoreInfo;
        RealmScore = realmScore;
    }
}
