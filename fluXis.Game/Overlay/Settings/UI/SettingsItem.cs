using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Overlay.Settings.UI;

public abstract partial class SettingsItem : Container
{
    public string Label { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool Enabled { get; init; } = true;

    public virtual bool IsDefault => true;

    protected FillFlowContainer TextFlow { get; private set; }

    private bool isDefault;
    private CircularContainer resetButton;

    private const float reset_button_height = 30;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 40;
        Alpha = Enabled ? 1 : 0.5f;

        InternalChildren = new Drawable[]
        {
            resetButton = new CircularContainer
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreRight,
                X = -5,
                Size = new Vector2(10, IsDefault ? 0 : reset_button_height),
                Masking = true,
                Child = new ClickableContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Action = Reset,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Accent
                    }
                }
            },
            TextFlow = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Text = Label,
                        FontSize = 24,
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopLeft
                    },
                    new FluXisSpriteText
                    {
                        Text = Description,
                        Colour = FluXisColors.Text2,
                        FontSize = 16,
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopLeft
                    }
                }
            }
        };
    }

    protected override void Update()
    {
        if (isDefault != IsDefault)
        {
            isDefault = IsDefault;
            resetButton.ResizeHeightTo(IsDefault ? 0 : reset_button_height, 200, Easing.OutQuint);
        }
    }

    public abstract void Reset();
}
