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

namespace fluXis.Screens.Skinning.UI;

public partial class SkinEditorColor : Container, IHasPopover
{
    public string Text { get; set; } = string.Empty;
    public Colour4 Color { get; set; } = Colour4.White;
    public Action<Colour4> OnColorChanged { get; set; } = _ => { };

    private Bindable<Colour4> colorBindable;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 30;

        Box box;
        FluXisSpriteText text;

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Text = Text
            },
            new ClickableContainer
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
                    box = new Box { RelativeSizeAxes = Axes.Both },
                    text = new FluXisSpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        FontSize = 16,
                        FixedWidth = true
                    }
                }
            }
        };

        colorBindable = new Bindable<Colour4>(Color);
        colorBindable.BindValueChanged(e =>
        {
            OnColorChanged(e.NewValue);
            box.Colour = e.NewValue;

            text.Text = e.NewValue.ToHex();
            text.Colour = FluXisColors.IsBright(e.NewValue) ? FluXisColors.TextDark : FluXisColors.Text;
        }, true);
    }

    public Popover GetPopover() => new FluXisPopover { Child = new FluXisColorPicker { Current = { BindTarget = colorBindable } } };
}
