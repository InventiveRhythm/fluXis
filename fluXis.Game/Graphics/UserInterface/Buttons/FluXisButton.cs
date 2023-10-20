using System;
using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace fluXis.Game.Graphics.UserInterface.Buttons;

public partial class FluXisButton : ClickableContainer, IHasTextTooltip
{
    public string Tooltip => TooltipText;
    public string TooltipText { get; set; }

    public int FontSize { get; set; } = 24;
    public string Text { get; set; } = "Default Text";
    public Colour4 Color { get; set; } = FluXisColors.Background4;
    public bool HoldToConfirm { get; set; }

    public ButtonData Data
    {
        set
        {
            Text = value.Text;
            Action = value.Action;
            Color = value.Color;
            HoldToConfirm = value.HoldToConfirm;
        }
    }

    [Resolved]
    private UISamples samples { get; set; }

    public new bool Enabled
    {
        get => EnabledBindable.Value;
        set
        {
            base.Enabled.Value = value;
            EnabledBindable.Value = value;

            if (IsLoaded)
                this.FadeTo(value ? 1 : 0.5f, 200);
            else
                Alpha = value ? 1 : 0.5f;
        }
    }

    public Bindable<bool> EnabledBindable => base.Enabled;

    private Box hoverBox;
    private Box holdBox;
    private Box flashBox;
    private CircularContainer content;

    private HoldToConfirmHandler holdToConfirmHandler;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        InternalChild = content = new CircularContainer
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Masking = true,
            Children = new Drawable[]
            {
                holdToConfirmHandler = new HoldToConfirmHandler
                {
                    Action = () => Action?.Invoke(),
                    HoldTime = config.GetBindable<float>(FluXisSetting.HoldToConfirm),
                    AutoActivate = false,
                    Interpolate = true
                },
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color
                },
                hoverBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                holdBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = .4f,
                    Width = 0
                },
                flashBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = new FluXisSpriteText
                    {
                        Text = Text,
                        FontSize = FontSize,
                        Shadow = true,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                }
            }
        };
    }

    protected override void Update()
    {
        base.Update();
        holdBox.Width = (float)holdToConfirmHandler.Progress;
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (!Enabled) return false;

        if (HoldToConfirm)
        {
            if (holdToConfirmHandler.Finished) triggerClick();
        }
        else triggerClick();

        return true;
    }

    private void triggerClick(ClickEvent e = null)
    {
        flashBox.FadeOutFromOne(1000, Easing.OutQuint);
        base.OnClick(e);
        samples.Click();
    }

    protected override bool OnHover(HoverEvent e)
    {
        if (!Enabled) return false;

        hoverBox.FadeTo(0.2f, 50);
        samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hoverBox.FadeTo(0, 200);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (!Enabled) return false;

        content.ScaleTo(0.95f, 1000, Easing.OutQuint);

        if (HoldToConfirm)
            holdToConfirmHandler.StartHold();

        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        content.ScaleTo(1, 1000, Easing.OutElastic);

        if (HoldToConfirm)
            holdToConfirmHandler.StopHold();
    }
}

public class ButtonData
{
    public string Text { get; init; } = "Default Text";
    public Colour4 Color { get; init; } = FluXisColors.Background2;
    public Action Action { get; init; } = () => { };
    public bool HoldToConfirm { get; init; }
}
