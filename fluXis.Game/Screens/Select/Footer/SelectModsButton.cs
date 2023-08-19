using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Mods.Drawables;
using fluXis.Game.Screens.Select.Mods;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Select.Footer;

public partial class SelectModsButton : Container
{
    public ModSelector ModSelector { get; init; }

    private Box hoverBox;
    private ModList modList;
    private int modCount = 0;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;

        InternalChildren = new Drawable[]
        {
            hoverBox = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Direction = FillDirection.Horizontal,
                Margin = new MarginPadding { Horizontal = 20 },
                Spacing = new Vector2(10),
                Children = new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        FontSize = 24,
                        Text = "Mods"
                    },
                    modList = new ModList
                    {
                        ModSpacing = -50,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Alpha = 0,
                        Scale = new Vector2(.6f)
                    }
                }
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        if (ModSelector == null) return;

        var mods = ModSelector.SelectedMods;
        modList.Alpha = mods.Count > 0 ? 1 : 0;

        if (mods.Count == 0)
        {
            modList.Mods.Clear();
            modCount = 0;
            return;
        }

        if (mods.Count == modCount) return;

        modList.Mods = mods;
        modCount = mods.Count;
    }

    protected override bool OnClick(ClickEvent e)
    {
        hoverBox.FadeTo(.4f).FadeTo(.2f, 200);
        ModSelector.IsOpen.Value = true;
        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        hoverBox.FadeTo(.2f, 50);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hoverBox.FadeOut(200);
    }
}
