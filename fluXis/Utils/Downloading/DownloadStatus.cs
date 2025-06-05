using System;

namespace fluXis.Utils.Downloading;

public class DownloadStatus
{
    public long OnlineID { get; }

    public DownloadState State
    {
        get => state;
        set
        {
            state = value;
            StateChanged?.Invoke(state);
        }
    }

    public float Progress
    {
        get => progress;
        set
        {
            progress = value;
            OnProgress?.Invoke(progress);
        }
    }

    public event Action<DownloadState> StateChanged;
    public event Action<float> OnProgress;

    private DownloadState state = DownloadState.Downloading;
    private float progress;

    public DownloadStatus(long onlineID)
    {
        OnlineID = onlineID;
    }
}
