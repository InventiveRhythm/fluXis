using System;
using fluXis.Audio;
using fluXis.Configuration;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Interaction;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Overlay.Toolbar;

public partial class ToolbarClock : Container
{
    [Resolved]
    private FluXisConfig config { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    private FluXisSpriteText localTime;
    private FluXisSpriteText gameTime;

    private Bindable<bool> useMeridianTime;

    private HoverLayer hover;
    private FlashLayer flash;
    private FillFlowContainer content;

    private double lastTime;

    [BackgroundDependencyLoader]
    private void load()
    {
        useMeridianTime = config.GetBindable<bool>(FluXisSetting.UseMeridianTime);

        RelativeSizeAxes = Axes.Y;
        Width = 120;

        InternalChildren = new Drawable[]
        {
            hover = new HoverLayer(),
            flash = new FlashLayer(),
            content = new FillFlowContainer()
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(-4),
                Children = new Drawable[]
                {
                    localTime = new FluXisSpriteText
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        WebFontSize = 14
                    },
                    gameTime = new FluXisSpriteText
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        WebFontSize = 10,
                        Alpha = .8f
                    }
                }
            }
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.Show();
        samples.Hover();
        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
        base.OnHoverLost(e);
    }

    protected override bool OnClick(ClickEvent e)
    {
        useMeridianTime.Value = !useMeridianTime.Value;
        updateLocalClock();

        flash.Show();
        samples.Click();

        return true;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        content.ScaleTo(.9f, 1000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        content.ScaleTo(1, 1000, Easing.OutElastic);
    }

    protected override void Update()
    {
        if (Time.Current - lastTime < 1000)
            return;

        lastTime = Time.Current;

        var secondsTotal = Time.Current / 1000;
        int hours = (int)secondsTotal / 3600;
        int minutes = (int)secondsTotal / 60 % 60;
        int seconds = (int)secondsTotal % 60;

        gameTime.Text = $"{hours:00}:{minutes:00}:{seconds:00}";
        updateLocalClock();
    }

    private void updateLocalClock() => localTime.Text = DateTime.Now.ToString(useMeridianTime.Value ? "hh:mm:ss tt" : "HH:mm:ss");
}
