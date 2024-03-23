using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Shared.Toolbox.Snap;

public partial class ToolboxSnapButton : ToolboxButton
{
    protected override string Text => $"1/{snap}{getSuffix()}";
    public override LocalisableString Tooltip => $"Snap to 1/{snap}{getSuffix()} of a beat";
    protected override bool IsSelected => settings.SnapDivisor == snap;

    [Resolved]
    private EditorSettings settings { get; set; }

    private int snap { get; }

    public ToolboxSnapButton(int value)
    {
        snap = value;
    }

    protected override void LoadComplete()
    {
        settings.SnapDivisorBindable.BindValueChanged(_ => UpdateSelectionState(), true);
    }

    protected override Drawable CreateIcon()
    {
        return new Container
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Children = new Drawable[]
            {
                new FluXisSpriteText
                {
                    RelativePositionAxes = Axes.Both,
                    Position = new Vector2(-.25f),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    FontSize = 24,
                    Text = "1"
                },
                new CircularContainer
                {
                    Width = 3,
                    Height = 32,
                    Rotation = 45,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both
                        }
                    }
                },
                new FluXisSpriteText
                {
                    RelativePositionAxes = Axes.Both,
                    Position = new Vector2(.25f),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    FontSize = 24,
                    Text = $"{snap}"
                }
            }
        };
    }

    public override void Select()
    {
        base.Select();
        settings.SnapDivisor = snap;
    }

    private string getSuffix()
    {
        switch (snap)
        {
            case 1:
                return "st";

            case 2:
                return "nd";

            case 3:
                return "rd";

            default:
                return "th";
        }
    }
}
