using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Overlay.Notification;

public partial class LoadingNotification : SimpleNotification
{
    public LoadingState State
    {
        get => state;
        set
        {
            state = value;
            Schedule(() =>
            {
                switch (state)
                {
                    case LoadingState.Loading:
                        Text = TextLoading;
                        background.FadeColour(Colour4.LightBlue, 500);
                        background.FadeTo(.25f, 300).Then().FadeTo(.15f, 300).Loop();
                        break;

                    case LoadingState.Loaded:
                        Text = TextSuccess;
                        background.FadeColour(Colour4.LightGreen, 500);
                        background.FadeTo(.25f, 200);
                        break;

                    case LoadingState.Failed:
                        Text = TextFailure;
                        background.FadeColour(Colour4.Red, 500);
                        background.FadeTo(.25f, 200);
                        break;
                }
            });
        }
    }

    private LoadingState state = LoadingState.Loading;
    private readonly Box background;

    public string TextLoading { get; set; } = "Loading...";
    public string TextSuccess { get; set; } = "Loaded!";
    public string TextFailure { get; set; } = "Failed!";

    public LoadingNotification()
    {
        Background.Add(background = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Colour4.LightBlue,
            Alpha = .25f
        });
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Text = state switch
        {
            LoadingState.Loaded => TextSuccess,
            LoadingState.Failed => TextFailure,
            _ => TextLoading
        };

        if (State == LoadingState.Loading)
            background.FadeTo(.25f, 300).Then().FadeTo(.15f, 300).Loop();
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
