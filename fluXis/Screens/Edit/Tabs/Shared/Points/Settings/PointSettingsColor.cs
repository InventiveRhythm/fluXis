using System;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;

namespace fluXis.Screens.Edit.Tabs.Shared.Points.Settings;

public partial class PointSettingsColor : PointSettingsBase, IHasPopover, IHasTooltip
{
    public string Text { get; set; } = "Color";
    public LocalisableString TooltipText { get; init; } = string.Empty;
    public Colour4 Color { get; set; } = Colour4.White;
    public Action<Colour4> OnColorChanged { get; set; } = _ => { };
    public Bindable<Colour4> Bindable { get; set; }

    public Bindable<bool> Enabled { get; init; } = new(true);
    public bool HideWhenDisabled { get; init; } = false;

    private ClickableContainer colorContainer;
    private Circle textBackground;
    private FluXisSpriteText text;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 32;

        Bindable ??= new Bindable<Colour4>(Color);

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = Text,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                WebFontSize = 16
            },
            colorContainer = new ClickableContainer
            {
                RelativeSizeAxes = Axes.Y,
                Width = 100,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                CornerRadius = 5,
                Masking = true,
                Action = this.ShowPopover,
                Children = new Drawable[]
                {
                    textBackground = new Circle { RelativeSizeAxes = Axes.Both },
                    text = new FluXisSpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        FontSize = 16,
                        FixedWidth = true
                    },
                }
            },
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        updateColors(Color);
        Bindable.BindValueChanged(valueChanged);
        Enabled.BindValueChanged(e =>
        {
            this.FadeTo(e.NewValue ? 1f : HideWhenDisabled ? 0 : .4f, 200);
            colorContainer.Action = e.NewValue ? this.ShowPopover : () => {};
        }, true);
        FinishTransforms();
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        Bindable.ValueChanged -= valueChanged;
    }

    private void valueChanged(ValueChangedEvent<Colour4> e)
    {
        updateColors(e.NewValue);
        OnColorChanged?.Invoke(e.NewValue);
    }

    private void updateColors(Colour4 color)
    {
        textBackground.Colour = color;
        text.Text = color.ToHex();
        text.Colour = Theme.IsBright(color) ? Theme.TextDark : Theme.Text;
    }

    public Popover GetPopover() => new FluXisPopover { Child = new FluXisColorPicker { Current = { BindTarget = Bindable } } };
}
