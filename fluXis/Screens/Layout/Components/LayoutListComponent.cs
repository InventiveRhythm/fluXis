using System;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens.Gameplay.HUD;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Layout.Components;

public partial class LayoutListComponent : CompositeDrawable
{
    [Resolved]
    private LayoutEditor editor { get; set; }

    [Resolved]
    private LayoutManager layouts { get; set; }

    private string key { get; }
    private Type type { get; }

    private HoverLayer hover;
    private FlashLayer flash;

    public LayoutListComponent(string key, Type type)
    {
        this.key = key;
        this.type = type;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        CornerRadius = 12;
        Masking = true;

        var settings = new HUDComponentSettings();
        var comp = layouts.CreateComponent(key, settings, editor);

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background3
            },
            hover = new HoverLayer(),
            flash = new FlashLayer(),
            new FillFlowContainer()
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding(12),
                Spacing = new Vector2(8),
                Children = new Drawable[]
                {
                    comp.With(d => d.Anchor = d.Origin = Anchor.TopCentre),
                    new FluXisSpriteText
                    {
                        Text = type.Name,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        WebFontSize = 14
                    }
                }
            }
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.Show();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
        base.OnHoverLost(e);
    }

    protected override bool OnClick(ClickEvent e)
    {
        flash.Show();
        return true;
    }
}
