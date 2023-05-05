namespace fluXis.Game.Overlay.Notification;

public partial class LoadingNotification : SimpleNotification
{
    public LoadingState State
    {
        get => state;
        set
        {
            state = value;

            switch (state)
            {
                case LoadingState.Loading:
                    Text = TextLoading;
                    break;

                case LoadingState.Loaded:
                    Text = TextSuccess;
                    break;

                case LoadingState.Failed:
                    Text = TextFailure;
                    break;
            }
        }
    }

    private LoadingState state = LoadingState.Loading;

    public string TextLoading { get; set; } = "Loading...";
    public string TextSuccess { get; set; } = "Loaded!";
    public string TextFailure { get; set; } = "Failed!";

    public LoadingNotification()
    {
        Text = TextLoading;
    }

    protected override void Update()
    {
        if (State != LoadingState.Loading)
            base.Update();
    }
}

public enum LoadingState
{
    Loading,
    Loaded,
    Failed
}
