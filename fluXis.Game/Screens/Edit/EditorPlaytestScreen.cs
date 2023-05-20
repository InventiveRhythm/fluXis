using System.Collections.Generic;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Mods;
using fluXis.Game.Screens.Gameplay;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Edit;

public partial class EditorPlaytestScreen : GameplayScreen
{
    private readonly MapInfo map;
    private readonly MapEvents events;

    public EditorPlaytestScreen(RealmMap realmMap, MapInfo info, MapEvents events)
        : base(realmMap, new List<IMod>())
    {
        map = info;
        this.events = events;
    }

    public override MapInfo LoadMap()
    {
        return map;
    }

    public override MapEvents LoadMapEvents()
    {
        return events;
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        base.OnExiting(e);
        Conductor.PauseTrack();
        return false;
    }

    public override void RestartMap()
    {
        Restart?.Play();
        this.Push(new EditorPlaytestScreen(RealmMap, map, events));
    }

    public override void Die()
    {
        Playfield.Manager.Dead = true;
        Conductor.PauseTrack();
        this.Exit();
    }

    public override void End()
    {
        Conductor.PauseTrack();
        this.Exit();
    }
}
