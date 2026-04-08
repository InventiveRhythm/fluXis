using System;
using fluXis.Database.Maps;
using fluXis.Screens.Select.Info.Tabs.Scores;
using JetBrains.Annotations;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Plugins.Capabilities;

public interface ISongSelectCapability : IPluginCapability
{
    [CanBeNull]
    (MenuItem Item, Func<bool> Predicate)[] ScoreListContextMenuItems => [];

    [CanBeNull]
    (MenuItem Item, Func<bool> Predicate)[] MapSetContextMenuItems => [];

    [CanBeNull]
    (MenuItem Item, Func<bool> Predicate)[] MapDifficultyContextMenuItems => [];

    void OnMapChanged(RealmMap map)
    {
    }

    void OnScoresChanged(ScoreListEntry[] scores)
    {
    }
}
