using System;
using System.Collections.Generic;
using fluXis.Game.Graphics.UserInterface.Context;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Scoring;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Screens.Select.Info.Scores;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;

namespace fluXis.Game.Tests.Select;

public partial class TestScoreEntry : FluXisTestScene
{
    public TestScoreEntry()
    {
        Add(new FluXisContextMenuContainer
        {
            RelativeSizeAxes = Axes.Both,
            Child = new Container
            {
                Width = 920,
                AutoSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Child = new ScoreListEntry
                {
                    Place = 1,
                    Player = APIUser.Dummy,
                    ReplayAction = () => Logger.Log("Replay requested"),
                    DeleteAction = () => Logger.Log("Delete requested"),
                    ScoreInfo = new ScoreInfo
                    {
                        Score = 1000000,
                        MaxCombo = 100,
                        Accuracy = 100,
                        Mods = new List<string> { "1.4x", "HD", "NF" },
                        Rank = ScoreRank.X,
                        Flawless = 100,
                        Perfect = 100,
                        Great = 100,
                        Alright = 100,
                        Okay = 100,
                        Miss = 0,
                        Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    }
                }
            }
        });
    }
}
