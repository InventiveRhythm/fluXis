using System;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Screens.Edit.Tabs.Charting.Effect.UI;

public partial class ColorTextBox : Container, IHasPopover
{
    public string LabelText { get; init; }
    public Colour4 Color { get; init; }
    public Action<Colour4> OnColorChanged { get; init; }

    private Bindable<Colour4> colorBindable;
    private FluXisSpriteText text;
    private Box box;

    [BackgroundDependencyLoader]
    private void load()
    {
        Height = 30;
        RelativeSizeAxes = Axes.X;

        Children = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = LabelText,
                FontSize = 30,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft
            },
            new GridContainer
            {
                Width = 200,
                Height = 30,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                ColumnDimensions = new[]
                {
                    new Dimension(),
                    new Dimension(GridSizeMode.Absolute, 5),
                    new Dimension(GridSizeMode.Absolute, 30)
                },
                Content = new[]
                {
                    new[]
                    {
                        text = new FluXisSpriteText
                        {
                            Text = Color.ToHex(),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            FontSize = 18
                        },
                        Empty(),
                        new ClickableContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Action = this.ShowPopover,
                            Masking = true,
                            CornerRadius = 5,
                            Children = new Drawable[]
                            {
                                box = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Color
                                }
                            }
                        }
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
        }, true);
    }

    public Popover GetPopover() => new FluXisPopover { Child = new FluXisColorPicker { Current = { BindTarget = colorBindable } } };
}
