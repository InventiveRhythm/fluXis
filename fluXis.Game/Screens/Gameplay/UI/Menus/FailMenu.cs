using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Gameplay.UI.Menus;

public partial class FailMenu : Container
{
    public GameplayScreen Screen { get; init; }

    private CircularContainer circle;
    private FluXisSpriteText text;
    private Container buttons;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0.001f; // making sure the fillflow is centered

        InternalChildren = new Drawable[]
        {
            circle = new CircularContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Masking = true,
                BorderThickness = 20,
                Size = new Vector2(0),
                BorderColour = Colour4.White,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    AlwaysPresent = true,
                    Alpha = 0
                }
            },
            new FillFlowContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, 20),
                AutoSizeDuration = 400,
                AutoSizeEasing = Easing.OutQuint,
                Children = new Drawable[]
                {
                    text = new FluXisSpriteText
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Text = "FAILED",
                        FontSize = 100,
                        Scale = new Vector2(1.2f)
                    },
                    buttons = new Container
                    {
                        Width = 300,
                        AutoSizeAxes = Axes.Y,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        CornerRadius = 20,
                        Masking = true,
                        Alpha = 0,
                        EdgeEffect = new EdgeEffectParameters
                        {
                            Type = EdgeEffectType.Shadow,
                            Colour = Color4.Black.Opacity(.25f),
                            Radius = 10
                        },
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = FluXisColors.Background2
                            },
                            new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Padding = new MarginPadding(10),
                                Spacing = new Vector2(5),
                                Children = new Drawable[]
                                {
                                    new GameplayMenuButton
                                    {
                                        Text = "Restart",
                                        SubText = "Try again?",
                                        Icon = FontAwesome.Solid.Redo,
                                        Action = () => Screen?.RestartMap()
                                    },
                                    new GameplayMenuButton
                                    {
                                        Text = "Quit",
                                        Color = FluXisColors.Red,
                                        SubText = "Bye bye",
                                        Icon = FontAwesome.Solid.DoorOpen,
                                        Action = () => Screen?.Exit()
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    public override void Show()
    {
        this.FadeIn(200);

        text.ScaleTo(1, 800, Easing.InQuint).Delay(2500).FadeIn().OnComplete(_ => buttons.FadeIn(200));
        circle.Delay(800).ResizeTo(400, 400, Easing.OutQuint).TransformTo(nameof(circle.BorderThickness), 0f, 1200, Easing.OutQuint);
    }
}
