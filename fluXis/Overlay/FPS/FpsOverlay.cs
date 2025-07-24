using System.Linq;
using fluXis.Configuration;
using fluXis.Graphics;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Platform;

namespace fluXis.Overlay.FPS;

public partial class FpsOverlay : Container
{
    [Resolved]
    private GameHost host { get; set; }

    [Resolved]
    private FluXisConfig config { get; set; }

    private double lastTime;
    private FluXisTextFlow textFlow;
    private Bindable<bool> showFps;
    private bool visible;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Anchor = Anchor.BottomRight;
        Origin = Anchor.BottomRight;
        Margin = new MarginPadding(16);
        CornerRadius = 4;
        Masking = true;
        AlwaysPresent = true;
        Alpha = 0;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                Colour = Theme.Background4,
                RelativeSizeAxes = Axes.Both
            },
            new Container
            {
                AutoSizeAxes = Axes.Both,
                Padding = new MarginPadding(6),
                Child = textFlow = new FluXisTextFlow
                {
                    AutoSizeAxes = Axes.Both,
                    TextAnchor = Anchor.TopRight,
                    ParagraphSpacing = 0,
                    WebFontSize = 10
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

        if (Time.Current - lastTime < 500) return;

        lastTime = Time.Current;

        textFlow.Clear();

        foreach (var thread in host.Threads)
        {
            var clock = thread.Clock;
            var firstChar = thread.Name.First();

            if (firstChar is 'U' or 'D') // Update or Draw
                textFlow.AddParagraph($"{clock.FramesPerSecond} {firstChar}PS ({clock.ElapsedFrameTime:0.0}ms)");
        }
    }

    protected override bool OnHover(HoverEvent e)
    {
        // this.FadeIn(Styling.TRANSITION_FADE);
        return false;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        // this.Delay(1200).FadeTo(.6f, Styling.TRANSITION_FADE);
    }

    public override void Show()
    {
        this.FadeTo(.6f, Styling.TRANSITION_FADE)
            .MoveToX(0, Styling.TRANSITION_MOVE, Easing.OutQuint);
    }

    public override void Hide()
    {
        this.FadeOut(Styling.TRANSITION_FADE)
            .MoveToX(40, Styling.TRANSITION_MOVE, Easing.OutQuint);
    }
}
