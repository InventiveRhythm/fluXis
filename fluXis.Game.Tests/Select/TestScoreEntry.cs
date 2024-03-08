using System;
using System.Collections.Generic;
using fluXis.Game.Graphics.UserInterface.Context;
using fluXis.Game.Screens.Select.Info.Scores;
using fluXis.Shared.Components.Users;
using fluXis.Shared.Scoring;
using fluXis.Shared.Scoring.Enums;
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
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Width = 0.5f,
                Child = new ScoreListEntry
                {
                    Place = 1,
                    Player = APIUserShort.Dummy,
                    ReplayAction = () => Logger.Log("Replay requested"),
                    DeleteAction = () => Logger.Log("Delete requested"),
                    ScoreInfo = new ScoreInfo
                    {
                        Score = 1000000,
                        MaxCombo = 100,
                        Accuracy = 100,
                        Mods = new List<string> { "HD", "NF" },
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
