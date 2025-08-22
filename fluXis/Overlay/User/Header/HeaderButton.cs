using System;
using fluXis.Audio;
using fluXis.Graphics;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Overlay.User.Header;

public partial class HeaderButton : CircularContainer
{
    public IconUsage Icon { get; set; }
    public Vector2 IconSize { get; init; } = new Vector2(20);
    public string Text { get; set; }
    public Action Action { get; set; }
    public Colour4 BackgroundColour { get; init; } = Theme.Background2;
    public bool UseAutoSize { get; init; } = true;
    public bool Enabled { get; set; } = false;

    [Resolved]
    private UISamples samples { get; set; }

    private HoverLayer hover;
    private FlashLayer flash;
    private FillFlowContainer buttonFlow;

    [BackgroundDependencyLoader]
    private void load()
    {
        if (UseAutoSize)
        {
            AutoSizeAxes = Axes.X;
            Height = 48;
        }
        
        Masking = true;
        EdgeEffect = Styling.ShadowSmall;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = BackgroundColour
            },
            hover = new HoverLayer(),
            flash = new FlashLayer(),
            buttonFlow = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Horizontal,
                Alpha = Action != null ? 1 : .5f,
                Padding = new MarginPadding
                {
                    Horizontal = Text != null ? 20 : 14
                },
                Spacing = new Vector2(10),
                Children = new Drawable[]
                {
                    new FluXisSpriteIcon
                    {
                        Icon = Icon,
                        Size = IconSize
                    },
                    new FluXisSpriteText
                    {
                        Text = Text,
                        Alpha = Text != null ? 1 : 0,
                        WebFontSize = 16
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        if (Action != null) Enabled = true;
    }

    protected override void Update()
    {
        base.Update();
        buttonFlow.Alpha = Enabled ? 1 : .5f;
    }

    protected override bool OnHover(HoverEvent e)
    {
        samples.Hover();
        if (Action == null || !Enabled) return false;

        hover.Show();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
    }

    protected override bool OnClick(ClickEvent e)
    {
        samples.Click(Action == null || !Enabled);

        if (Action == null || !Enabled) return false;

        flash.Show();
        Action?.Invoke();
        return true;
    }
}