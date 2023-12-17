using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Multiplayer;

public partial class MultiplayerMenuMusic : Container
{
    private MultiplayerTrack baseTrack;

    private MultiplayerTrack rankedMain;
    private MultiplayerTrack rankedPrepare;

    private MultiplayerTrack lobbyList;
    private MultiplayerTrack lobbyPrepare;

    private MultiplayerTrack win;
    private MultiplayerTrack lose;

    [BackgroundDependencyLoader]
    private void load(ITrackStore trackStore)
    {
        InternalChildren = new[]
        {
            baseTrack = trackStore.Get("Menu/Multiplayer/base.mp3"),
            rankedMain = trackStore.Get("Menu/Multiplayer/ranked-main.mp3"),
            rankedPrepare = trackStore.Get("Menu/Multiplayer/ranked-prepare.mp3"),
            lobbyList = trackStore.Get("Menu/Multiplayer/lobby-list.mp3"),
            lobbyPrepare = trackStore.Get("Menu/Multiplayer/lobby-prepare.mp3"),
            win = trackStore.Get("Menu/Multiplayer/win.mp3"),
            lose = trackStore.Get("Menu/Multiplayer/lose.mp3")
        };
    }

    protected override void LoadComplete()
    {
        baseTrack.Start();

        // Ranked
        rankedMain.Start();
        rankedPrepare.Start();

        // OpenLobby
        lobbyList.Start();
        lobbyPrepare.Start();

        // Win/Lose
        win.Start();
        lose.Start();
    }

    public void GoToLayer(int layer, int mode, int alt = 0)
    {
        baseTrack.VolumeTo(1);

        switch (mode)
        {
            case -1:
                rankedMain.VolumeTo(0);
                rankedPrepare.VolumeTo(0);

                lobbyList.VolumeTo(0);
                lobbyPrepare.VolumeTo(0);

                win.VolumeTo(0);
                lose.VolumeTo(0);
                break;

            case 0: // Ranked
                rankedMain.VolumeTo(layer >= 0 ? 1 : 0);
                rankedPrepare.VolumeTo(layer == 1 ? 1 : 0);

                if (layer == 2)
                {
                    win.VolumeTo(alt == 0 ? 1 : 0);
                    lose.VolumeTo(alt == 1 ? 1 : 0);
                }
                else
                {
                    win.VolumeTo(0);
                    lose.VolumeTo(0);
                }

                break;

            case 1: // OpenLobby
                lobbyList.VolumeTo(layer >= 0 ? 1 : 0);
                lobbyPrepare.VolumeTo(layer == 1 ? 1 : 0);

                if (layer == 2)
                {
                    win.VolumeTo(alt == 0 ? 1 : 0);
                    lose.VolumeTo(alt == 1 ? 1 : 0);
                }
                else
                {
                    win.VolumeTo(0);
                    lose.VolumeTo(0);
                }

                break;
        }
    }

    public void StopAll()
    {
        baseTrack.VolumeTo(0);

        rankedMain.VolumeTo(0);
        rankedPrepare.VolumeTo(0);

        lobbyList.VolumeTo(0);
        lobbyPrepare.VolumeTo(0);

        win.VolumeTo(0);
        lose.VolumeTo(0);
    }

    private partial class MultiplayerTrack : Component
    {
        private Track track { get; init; }

        private double volume
        {
            get => track?.Volume.Value ?? 0;
            set
            {
                if (track == null) return;

                track.Volume.Value = value;
            }
        }

        public void Start()
        {
            if (track == null) return;

            volume = 0;
            track.Looping = true;
            track.Start();
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            track?.Dispose();
        }

        public void VolumeTo(double volume, int duration = 400) => this.TransformTo(nameof(volume), volume, duration);

        public static implicit operator MultiplayerTrack(Track track) => new() { track = track };
    }
}
