using System;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Localization;
using fluXis.Mods;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Select.Mods;

public partial class ModReset : CompositeDrawable, IMod
{
    public new string Name => "Reset";
    public string Acronym => "RM";
    public string Description => "Clear all selected mods.";
    public IconUsage Icon => FontAwesome6.Solid.RotateLeft;
    public ModType Type => ModType.Misc;
    public float ScoreMultiplier => 1f;
    public bool Rankable => true;
    public Type[] IncompatibleMods => Array.Empty<Type>();

    [Resolved]
    private UISamples samples { get; set; }

    private Container content { get; }
    private Box background { get; }
    private HoverLayer hover { get; }
    private FlashLayer flash { get; }

    private ModsOverlay overlay { get; set; }

    private Box line { get; }
    private FillFlowContainer flow { get; }
    private SpriteIcon icon { get; }
    private ForcedHeightText name { get; }
    private ForcedHeightText description { get; }

    public ModReset(ModsOverlay overlay)
    {
        this.overlay = overlay;

        RelativeSizeAxes = Axes.X;
        Width = 0.75f;
        Height = 74;
        AlwaysPresent = true;
        Alpha = 0;

        InternalChild = content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Shear = new Vector2(.2f, 0),
            CornerRadius = 4,
            Masking = true,
            Children = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background2
                },
                hover = new HoverLayer { Colour = Colour4.White },
                line = new Box
                {
                    Width = 6,
                    RelativeSizeAxes = Axes.Y,
                    Colour = Colour4.White
                },
                flow = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Horizontal,
                    Padding = new MarginPadding { Horizontal = 20 },
                    Spacing = new Vector2(12),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Shear = new Vector2(-.2f, 0),
                    Children = new Drawable[]
                    {
                        icon = new FluXisSpriteIcon
                        {
                            Icon = FontAwesome6.Solid.RotateLeft,
                            Size = new Vector2(20),
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                        },
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Spacing = new Vector2(2),
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Children = new Drawable[]
                            {
                                name = new ForcedHeightText
                                {
                                    Text = LocalizationStrings.Mods.GetName(this),
                                    Height = 12,
                                    TextColor = Colour4.White,
                                    WebFontSize = 16,
                                },
                                description = new ForcedHeightText
                                {
                                    Text = LocalizationStrings.Mods.GetDescription(this),
                                    Height = 8,
                                    TextColor = Colour4.White,
                                    WebFontSize = 12,
                                    Alpha = .8f
                                }
                            }
                        }
                    }
                },
                flash = new FlashLayer { Colour = Colour4.White }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        FinishTransforms(true);
    }

    protected override bool OnClick(ClickEvent e)
    {
        samples.Click();
        overlay.DeselectAll();

        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.Show();
        samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
    }

    public override void Show()
    {
        this.FadeIn(400);
        content.MoveToX(50).MoveToX(0, 400, Easing.OutQuint);
    }

    public override void Hide()
    {
        this.FadeOut(200);
        content.MoveToX(-50, 400, Easing.OutQuint);
    }

    public void Animate()
    {
        flash.Show();

        icon.Colour = FluXisColors.Background2;
        name.Colour = FluXisColors.Background2;
        description.Colour = FluXisColors.Background2;

        icon.Delay(100).FadeColour(Colour4.White, 250);
        name.Delay(100).FadeColour(Colour4.White, 250);
        description.Delay(100).FadeColour(Colour4.White, 250);
    }
}