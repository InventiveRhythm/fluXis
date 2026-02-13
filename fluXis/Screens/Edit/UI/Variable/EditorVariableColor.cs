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

namespace fluXis.Screens.Edit.UI.Variable;

public partial class EditorVariableColor : EditorVariableBase, IHasPopover
{
    public Colour4 CurrentValue { get; set; } = Colour4.White;
    public Action<Colour4> OnValueChanged { get; set; } = _ => { };
    public Bindable<Colour4> Bindable { get; set; }

    private ClickableContainer colorContainer;
    private Circle textBackground;
    private FluXisSpriteText text;

    public EditorVariableColor()
    {
        Text = "Color";
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 32;

        Bindable ??= new Bindable<Colour4>(CurrentValue);

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
        updateColors(CurrentValue);
        Bindable.BindValueChanged(valueChanged);

        base.LoadComplete();
    }

    protected override void UpdateEnabledState(bool state)
        => colorContainer.Action = state ? this.ShowPopover : () => { };

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        Bindable.ValueChanged -= valueChanged;
    }

    private void valueChanged(ValueChangedEvent<Colour4> e)
    {
        updateColors(e.NewValue);
        OnValueChanged?.Invoke(e.NewValue);
    }

    private void updateColors(Colour4 color)
    {
        textBackground.Colour = color;
        text.Text = color.ToHex();
        text.Colour = Theme.IsBright(color) ? Theme.TextDark : Theme.Text;
    }

    public Popover GetPopover() => new FluXisPopover { Child = new FluXisColorPicker { Current = { BindTarget = Bindable } } };
}
