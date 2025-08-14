using System;
using fluXis.Audio;
using fluXis.Configuration;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

namespace fluXis.Overlay.Toolbar;

public partial class ToolbarClock : Container
{
    [Resolved]
    private FluXisConfig config { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    private FluXisSpriteText localTime;
    private FluXisSpriteText gameTime;

    private Bindable<bool> useMeridianTime = new(false);

    private HoverLayer hover;
    private FlashLayer flash;
    private Container clockContainer;

    private double lastTime;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Y;
        Width = 100;
        Padding = new MarginPadding { Vertical = 5 };

        InternalChildren = new Drawable[]
        {
            clockContainer = new()
            {
                RelativeSizeAxes = Axes.Both,
                CornerRadius = 5,
                Masking = true,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    hover = new HoverLayer(),
                    flash = new FlashLayer(),
                    localTime = new FluXisSpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.BottomCentre,
                        Y = 5
                    },
                    gameTime = new FluXisSpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.TopCentre,
                        FontSize = 14,
                        Colour = Theme.Text2
                    }
                }
            }

        };

        useMeridianTime = config.GetBindable<bool>(FluXisSetting.UseMeridianTime);
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
        clockContainer.ScaleTo(.9f, 1000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        clockContainer.ScaleTo(1, 1000, Easing.OutElastic);
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
    
    private void updateLocalClock()
    {
        if (useMeridianTime.Value)
            localTime.Text = DateTime.Now.ToString("hh:mm:ss tt");
        else
            localTime.Text = DateTime.Now.ToString("HH:mm:ss");
    }
}