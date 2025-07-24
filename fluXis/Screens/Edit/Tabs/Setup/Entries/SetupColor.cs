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
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Setup.Entries;

public partial class SetupColor : SetupEntry, IHasPopover
{
    protected override float ContentSpacing => -3;

    public Colour4 Color { get; set; }
    public Action<Colour4> OnColorChanged { get; set; }

    private Bindable<Colour4> bindableColor;
    private FluXisSpriteText text;
    private Box box;

    public SetupColor(string title)
        : base(title)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        bindableColor = new Bindable<Colour4>(Color);

        AddInternal(new Container
        {
            Size = new Vector2(40),
            Anchor = Anchor.CentreRight,
            Origin = Anchor.CentreRight,
            CornerRadius = 5,
            Masking = true,
            Margin = new MarginPadding { Right = 10 },
            Children = new Drawable[]
            {
                box = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color
                }
            }
        });
    }

    protected override Drawable CreateContent() => text = new FluXisSpriteText
    {
        RelativeSizeAxes = Axes.X,
        Text = Color.ToHex(),
        WebFontSize = 18
    };

    protected override void LoadComplete()
    {
        base.LoadComplete();

        bindableColor.BindValueChanged(color =>
        {
            Color = color.NewValue;
            box.Colour = Color;
            text.Text = Color.ToHex();
            OnColorChanged?.Invoke(Color);
        });
    }

    protected override bool OnClick(ClickEvent e)
    {
        StartHighlight();
        this.ShowPopover();
        return true;
    }

    public Popover GetPopover() => new FluXisPopover
    {
        ContentPadding = 0,
        OnClose = StopHighlight,
        Child = new FluXisColorPicker { Current = { BindTarget = bindableColor } }
    };
}
