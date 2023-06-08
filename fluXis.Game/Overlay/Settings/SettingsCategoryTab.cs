using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.Settings;

public partial class SettingsCategoryTab : Container
{
    private readonly SettingsMenu menu;

    public readonly FillFlowContainer TabContent;
    public int Index { get; set; }

    public SettingsSection Section { get; }

    public SettingsCategoryTab(SettingsMenu menu, SettingsSection section)
    {
        this.menu = menu;
        Section = section;

        RelativeSizeAxes = Axes.Y;
        Width = 50;
        Padding = new MarginPadding(5) { Right = 10 };
        Masking = true;

        Add(TabContent = new FillFlowContainer
        {
            Height = 40,
            AutoSizeAxes = Axes.X,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(10, 0),
            Children = new Drawable[]
            {
                new SpriteIcon
                {
                    Size = new Vector2(30),
                    Icon = section.Icon,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Margin = new MarginPadding { Horizontal = 5 }
                },
                new FluXisSpriteText
                {
                    Text = section.Title,
                    FontSize = 24,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft
                }
            }
        });
    }

    protected override bool OnClick(ClickEvent e)
    {
        menu.Selector.SelectTab(this);
        return true;
    }

    public void Select() => this.ResizeWidthTo(TabContent.DrawWidth + 10, 400, Easing.OutQuint);

    public void Deselect() => this.ResizeWidthTo(50, 400, Easing.OutQuint);
}
