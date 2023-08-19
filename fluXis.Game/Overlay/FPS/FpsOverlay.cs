using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Platform;
using osuTK;

namespace fluXis.Game.Overlay.FPS;

public partial class FpsOverlay : Container
{
    [Resolved]
    private GameHost host { get; set; }

    [Resolved]
    private FluXisConfig config { get; set; }

    private double lastTime;
    private FluXisTextFlow textFlow;
    private Bindable<bool> showFps;
    private bool visible = true;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Anchor = Anchor.BottomRight;
        Origin = Anchor.BottomRight;
        AlwaysPresent = true;
        Margin = new MarginPadding(20);
        CornerRadius = 5;
        Masking = true;
        Y = -80;
        EdgeEffect = new EdgeEffectParameters
        {
            Type = EdgeEffectType.Shadow,
            Colour = Colour4.Black.Opacity(0.25f),
            Radius = 5,
            Offset = new Vector2(0, 1)
        };

        InternalChildren = new Drawable[]
        {
            new Box
            {
                Colour = FluXisColors.Background4,
                RelativeSizeAxes = Axes.Both
            },
            new Container
            {
                AutoSizeAxes = Axes.Both,
                Padding = new MarginPadding(5),
                Child = textFlow = new FluXisTextFlow
                {
                    AutoSizeAxes = Axes.Both,
                    TextAnchor = Anchor.TopRight,
                    FontSize = 16
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        showFps = config.GetBindable<bool>(FluXisSetting.ShowFps);
    }

    protected override void Update()
    {
        base.Update();

        if (showFps.Value != visible)
        {
            visible = showFps.Value;

            if (visible)
                Show();
            else
                Hide();
        }

        if (Time.Current - lastTime < 1000) return;

        lastTime = Time.Current;

        textFlow.Clear();

        foreach (var thread in host.Threads)
        {
            var clock = thread.Clock;
            var firstChar = thread.Name.First();

            if (firstChar is 'U' or 'D') // Update or Draw
                textFlow.AddParagraph($"{clock.FramesPerSecond} {firstChar}PS");
        }
    }

    public override void Show()
    {
        this.FadeIn(200, Easing.OutQuint);
        this.MoveToY(-80, 400, Easing.OutQuint);
    }

    public override void Hide()
    {
        this.FadeOut(200, Easing.OutQuint);
        this.MoveToY(-40, 400, Easing.OutQuint);
    }
}
