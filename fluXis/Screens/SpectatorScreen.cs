using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Types;
using fluXis.Map;
using fluXis.Online.Spectator;
using fluXis.Online.Spectator.Models;
using fluXis.Screens.Gameplay;
using fluXis.Screens.Gameplay.Spectator;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Screens;

namespace fluXis.Screens;

public partial class SpectatorScreen : FluXisScreen
{
    [Resolved]
    private MapStore maps { get; set; }

    [Resolved]
    private PanelContainer panels { get; set; }

    [Resolved]
    private SpectatorClient spectator { get; set; }

    private readonly long userid;

    public SpectatorScreen(long userid)
    {
        this.userid = userid;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        spectator.StartWatching(userid).Wait();
    }

    private void onStartPlaying(long id, SpectatorState state)
    {
        if (id != userid)
            return;

        var map = maps.GetMapFromOnlineID(state.MapID!.Value);

        if (map is null)
        {
            // TODO: download map
            panels.Add(new SingleButtonPanel(Phosphor.Bold.Warning, "The player you are spectating is playing a map you dont have downloaded.", ""));
            return;
        }

        var mods = state.Mods.Select(ModUtils.GetFromAcronym).Where(x => x != null).ToList();
        this.Push(new GameplayLoader(map, mods, () => new SpectatorGameplay(map, mods, spectator.Replays[userid])));
    }

    private void onStopPlaying(long id)
    {
        if (id != userid)
            return;

        this.MakeCurrent();
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);
        spectator.OnStartedPlaying += onStartPlaying;
        spectator.OnStoppedPlaying += onStopPlaying;
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        spectator.OnStartedPlaying -= onStartPlaying;
        spectator.OnStoppedPlaying -= onStopPlaying;
        spectator.StopWatching(userid);
        return base.OnExiting(e);
    }
}
