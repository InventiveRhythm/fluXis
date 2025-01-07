using System;
using fluXis.Audio;
using fluXis.Configuration;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;

namespace fluXis.Graphics.UserInterface.Buttons;

public partial class FluXisButton : ClickableContainer, IHasTooltip
{
    public LocalisableString TooltipText { get; init; }

    public float FontSize { get; init; } = 24;
    public LocalisableString Text { get; set; } = "Default Text";
    public Colour4 Color { get; set; } = FluXisColors.Background4;
    public Colour4 TextColor { get; set; } = FluXisColors.Text;
    public bool HoldToConfirm { get; set; }

    public ButtonData Data
    {
        set
        {
            Text = value.Text;
            Action = value.Action;
            Color = value.Color;
            TextColor = value.TextColor;
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

    private HoverLayer hoverBox;
    private Box holdBox;
    private FlashLayer flashBox;
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
                hoverBox = new HoverLayer(),
                holdBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = .4f,
                    Width = 0
                },
                flashBox = new FlashLayer(),
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = new FluXisSpriteText
                    {
                        Text = Text,
                        FontSize = FontSize,
                        Colour = TextColor,
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
        samples.Click(!Enabled);
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
        flashBox.Show();
        base.OnClick(e);
    }

    protected override bool OnHover(HoverEvent e)
    {
        samples.Hover();
        if (!Enabled) return false;

        hoverBox.Show();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hoverBox.Hide();
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
    public LocalisableString Text { get; init; } = "Default Text";
    public Colour4 TextColor { get; init; } = FluXisColors.Text;
    public Colour4 Color { get; init; } = FluXisColors.Background4;
    public Action Action { get; init; } = () => { };
    public bool HoldToConfirm { get; init; }
}
