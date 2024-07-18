using System;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Screens.Edit.BottomBar.Snap;
using fluXis.Game.Skinning;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;

namespace fluXis.Game.Screens.Edit.BottomBar;

public partial class SnapControl : CompositeDrawable, IHasPopover, IHasTooltip
{
    public static int[] DefaultSnaps { get; } = { 1, 2, 3, 4, 6, 8, 12, 16 };

    public LocalisableString TooltipText => "The current beat snap.\nCan also be changed by shift-scrolling.";

    private FluXisSpriteText text;

    [Resolved]
    private EditorSettings settings { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    [Resolved]
    private SkinManager skinManager { get; set; }

    private Box hover;
    private Box flash;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            hover = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            flash = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            text = new FluXisSpriteText
            {
                WebFontSize = 16,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        settings.SnapDivisorBindable.BindValueChanged(e =>
        {
            var snap = e.NewValue;
            text.Text = $"1/{snap.ToOrdinalShort(true)}";

            var idx = Array.IndexOf(DefaultSnaps, snap);
            Colour = skinManager.SkinJson.SnapColors.GetColor(idx);
        }, true);
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.FadeTo(.2f, 50);
        samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeOut(200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        samples.Click();
        flash.FadeOutFromOne(1000, Easing.OutQuint);
        this.ShowPopover();
        return true;
    }

    public Popover GetPopover() => new FluXisPopover
    {
        ContentPadding = 0,
        BodyRadius = 12,
        Child = new SnapControlPopover()
    };
}
