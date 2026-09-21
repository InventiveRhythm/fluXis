using System.Collections.Generic;
using fluXis.Map;
using fluXis.Online.API.Models.Users;

namespace fluXis.Replays;

// TODO: make per-mode auto generators
public abstract class AutoGenerator
{
    /// <summary>
    /// How long the key is held down for
    /// </summary>
    protected const float KEY_DOWN_TIME = 50;

    protected PlayableMap Map { get; }
    private List<ReplayFrame> frames { get; } = new();

    protected AutoGenerator(PlayableMap map)
    {
        Map = map;
    }

    public Replay Generate()
    {
        frames.Clear();
        frames.AddRange(GenerateFrames());

        var replay = new Replay
        {
            PlayerID = APIUser.AutoPlay.ID,
            Frames = frames
        };

        return replay;
    }

    protected abstract IEnumerable<ReplayFrame> GenerateFrames();

    protected interface IAction
    {
        double Time { get; }
        int Lane { get; }
    }

    protected class PressAction : IAction
    {
        public double Time { get; }
        public int Lane { get; }

        public PressAction(double time, int lane)
        {
            Time = time;
            Lane = lane;
        }
    }

    protected class ReleaseAction : IAction
    {
        public double Time { get; }
        public int Lane { get; }

        public ReleaseAction(double time, int lane)
        {
            Time = time;
            Lane = lane;
        }
    }
}
