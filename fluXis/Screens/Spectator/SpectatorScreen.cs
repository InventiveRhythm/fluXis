using System.Linq;
using fluXis.Graphics;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Types;
using fluXis.Input;
using fluXis.Map;
using fluXis.Online;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Spectator;
using fluXis.Online.Spectator.Models;
using fluXis.Screens.Gameplay;
using fluXis.Screens.Multiplayer;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Screens.Spectator;

public partial class SpectatorScreen : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override float BackgroundBlur => .25f;
    public override float BackgroundDim => .5f;
    public override float Zoom => 1.1f;
    public override bool ApplyValuesAfterLoad => true;
    public override bool AutoPlayNext => true;

    [Resolved]
    private MapStore maps { get; set; }

    [Resolved]
    private PanelContainer panels { get; set; }

    [Resolved]
    private SpectatorClient spectator { get; set; }

    private readonly long userid;
    private APIUser user;

    private ScreenHeader header;
    private SpectatorFooter footer;

    public SpectatorScreen(long userid)
    {
        this.userid = userid;
    }

    [BackgroundDependencyLoader]
    private void load(UserCache cache)
    {
        spectator.StartWatching(userid).Wait();
        user = cache.Get(userid) ?? APIUser.CreateUnknown(userid);

        InternalChildren =
        [
            header = new ScreenHeader
            {
                Title = "Spectating",
                SubTitle = user.PreferredName
            },
            footer = new SpectatorFooter(this.Exit)
        ];
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

    private void onDisconnect() => panels.Add(new DisconnectedPanel(() =>
    {
        this.MakeCurrent();
        this.Exit();
    }));

    private void enterAnimation()
    {
        this.FadeInFromZero(Styling.TRANSITION_FADE);
        footer.Show();
        header.Show();
    }

    private void exitAnimation()
    {
        this.FadeOut(Styling.TRANSITION_FADE);
        footer.Hide();
        header.Hide();
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        enterAnimation();

        spectator.OnStartedPlaying += onStartPlaying;
        spectator.OnStoppedPlaying += onStopPlaying;
        spectator.OnDisconnect += onDisconnect;
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        base.OnSuspending(e);
        exitAnimation();
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        base.OnResuming(e);
        enterAnimation();
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        exitAnimation();

        spectator.OnStartedPlaying -= onStartPlaying;
        spectator.OnStoppedPlaying -= onStopPlaying;
        spectator.OnDisconnect -= onDisconnect;

        spectator.StopWatching(userid);
        return base.OnExiting(e);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back:
                this.Exit();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e)
    {
    }
}
