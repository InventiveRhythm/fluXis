using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Multiplayer;

public partial class MultiplayerMenuMusic : Container<DrawableTrack>
{
    private DrawableTrack baseTrack;

    private DrawableTrack rankedMain;
    private DrawableTrack rankedPrepare;

    private DrawableTrack lobbyList;
    private DrawableTrack lobbyPrepare;

    private DrawableTrack win;
    private DrawableTrack lose;

    [BackgroundDependencyLoader]
    private void load(ITrackStore trackStore)
    {
        InternalChildren = new[]
        {
            baseTrack = new DrawableTrack(trackStore.Get("Menu/Multiplayer/base.mp3")),
            rankedMain = new DrawableTrack(trackStore.Get("Menu/Multiplayer/ranked-main.mp3")),
            rankedPrepare = new DrawableTrack(trackStore.Get("Menu/Multiplayer/ranked-prepare.mp3")),
            lobbyList = new DrawableTrack(trackStore.Get("Menu/Multiplayer/lobby-list.mp3")),
            lobbyPrepare = new DrawableTrack(trackStore.Get("Menu/Multiplayer/lobby-prepare.mp3")),
            win = new DrawableTrack(trackStore.Get("Menu/Multiplayer/win.mp3")),
            lose = new DrawableTrack(trackStore.Get("Menu/Multiplayer/lose.mp3"))
        };
    }

    protected override void LoadComplete()
    {
        foreach (var track in Children)
        {
            track.Looping = true;
            track.VolumeTo(0);
            track.Start();
        }
    }

    public void GoToLayer(int layer, int mode, int alt = 0)
    {
        const float duration = FluXisScreen.FADE_DURATION;

        baseTrack.VolumeTo(1, duration);

        switch (mode)
        {
            case -1:
                rankedMain.VolumeTo(0, duration);
                rankedPrepare.VolumeTo(0, duration);

                lobbyList.VolumeTo(0, duration);
                lobbyPrepare.VolumeTo(0, duration);

                win.VolumeTo(0, duration);
                lose.VolumeTo(0, duration);
                break;

            case 0: // Ranked
                rankedMain.VolumeTo(layer >= 0 ? 1 : 0, duration);
                rankedPrepare.VolumeTo(layer == 1 ? 1 : 0, duration);

                if (layer == 2)
                {
                    win.VolumeTo(alt == 0 ? 1 : 0, duration);
                    lose.VolumeTo(alt == 1 ? 1 : 0, duration);
                }
                else
                {
                    win.VolumeTo(0, duration);
                    lose.VolumeTo(0, duration);
                }

                break;

            case 1: // OpenLobby
                lobbyList.VolumeTo(layer >= 0 ? 1 : 0, duration);
                lobbyPrepare.VolumeTo(layer == 1 ? 1 : 0, duration);

                if (layer == 2)
                {
                    win.VolumeTo(alt == 0 ? 1 : 0, duration);
                    lose.VolumeTo(alt == 1 ? 1 : 0, duration);
                }
                else
                {
                    win.VolumeTo(0, duration);
                    lose.VolumeTo(0, duration);
                }

                break;
        }
    }

    public void StopAll()
    {
        foreach (var track in Children)
            track.VolumeTo(0, FluXisScreen.FADE_DURATION);
    }
}
