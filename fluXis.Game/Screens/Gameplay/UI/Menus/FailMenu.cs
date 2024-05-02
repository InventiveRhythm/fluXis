using fluXis.Game.Configuration;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.UI.Menus;

public partial class FailMenu : Container, IKeyBindingHandler<FluXisGlobalKeybind>
{
    [Resolved]
    private GameplayScreen screen { get; set; }

    private GameplaySamples samples => screen.Samples;

    private Box dim;
    private CircularContainer circle;
    private FluXisSpriteText text;
    private Container buttonsContainer;
    private SelectionCycleContainer<GameplayMenuButton> buttons;

    private Bindable<bool> dimOnLowHealth;
    private bool failed;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        dimOnLowHealth = config.GetBindable<bool>(FluXisSetting.DimAndFade);

        RelativeSizeAxes = Axes.Both;
        Alpha = 0.001f; // making sure the flow is centered

        InternalChildren = new Drawable[]
        {
            dim = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black.Opacity(.5f),
                Alpha = dimOnLowHealth.Value ? 0 : 1
            },
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
                    buttonsContainer = new Container
                    {
                        Width = 300,
                        AutoSizeAxes = Axes.Y,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        CornerRadius = 20,
                        Masking = true,
                        Alpha = 0,
                        EdgeEffect = FluXisStyles.ShadowMedium,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = FluXisColors.Background2
                            },
                            buttons = new SelectionCycleContainer<GameplayMenuButton>
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Padding = new MarginPadding(10),
                                Spacing = new Vector2(5),
                                Children = new GameplayMenuButton[]
                                {
                                    new()
                                    {
                                        Text = "Restart",
                                        SubText = "Try again?",
                                        Icon = FontAwesome6.Solid.RotateRight,
                                        Action = () => screen?.RestartMap()
                                    },
                                    new()
                                    {
                                        Text = "Quit",
                                        Color = FluXisColors.Red,
                                        SubText = "Bye bye",
                                        Icon = FontAwesome6.Solid.DoorOpen,
                                        Action = () => screen?.Exit()
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        dimOnLowHealth.BindValueChanged(_ => dim.FadeTo(dimOnLowHealth.Value ? 0 : 1, 300), true);
    }

    public override void Show()
    {
        this.FadeIn(200);

        failed = true;

        this.Delay(800).FadeIn().OnComplete(_ => samples.Fail());
        text.ScaleTo(1, 800, Easing.InQuint).Delay(2500).FadeIn().OnComplete(_ => buttonsContainer.FadeIn(200));
        circle.Delay(800).ResizeTo(400, 400, Easing.OutQuint).TransformTo(nameof(circle.BorderThickness), 0f, 1200, Easing.OutQuint);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (!failed)
            return false;

        switch (e.Action)
        {
            case FluXisGlobalKeybind.Next:
                buttons.Next();
                return true;

            case FluXisGlobalKeybind.Previous:
                buttons.Previous();
                return true;

            case FluXisGlobalKeybind.Select:
                buttons.Current?.TriggerClick();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
