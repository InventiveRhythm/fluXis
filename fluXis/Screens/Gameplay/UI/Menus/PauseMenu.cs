using fluXis.Audio.Transforms;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Input;
using fluXis.Screens.Gameplay.Audio;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Screens.Gameplay.UI.Menus;

public partial class PauseMenu : CompositeDrawable, IKeyBindingHandler<FluXisGlobalKeybind>
{
    [Resolved]
    private GameplayClock clock { get; set; }

    [Resolved]
    private GameplayScreen screen { get; set; }

    private Container background;
    private Container content;
    private SelectionCycleContainer<GameplayMenuButton> buttons;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            background = new ClickableContainer
            {
                RelativeSizeAxes = Axes.Both,
                RelativePositionAxes = Axes.X,
                X = 1.2f,
                Width = 1.2f,
                Shear = new Vector2(-.2f, 0f),
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.Black,
                    Alpha = 0.4f
                }
            },
            content = new Container
            {
                Width = 400,
                AutoSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                CornerRadius = 20,
                Masking = true,
                Scale = new Vector2(.8f),
                Alpha = 0,
                EdgeEffect = Styling.ShadowMedium,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Theme.Background2
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Direction = FillDirection.Vertical,
                        Padding = new MarginPadding(10),
                        Spacing = new Vector2(5),
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                Text = "Paused",
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Margin = new MarginPadding { Bottom = -15 },
                                FontSize = 48
                            },
                            new FluXisSpriteText
                            {
                                Text = "What do you want to do?",
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                FontSize = 24,
                                Colour = Colour4.Gray,
                                Margin = new MarginPadding { Bottom = 10 }
                            },
                            buttons = new SelectionCycleContainer<GameplayMenuButton>
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Direction = FillDirection.Vertical,
                                Spacing = new Vector2(5),
                                Children = new GameplayMenuButton[]
                                {
                                    new()
                                    {
                                        Text = "Resume",
                                        SubText = "Continue playing",
                                        Icon = FontAwesome6.Solid.Play,
                                        Action = () => screen.IsPaused.Value = false
                                    },
                                    new()
                                    {
                                        Text = "Restart",
                                        SubText = "Try again?",
                                        Icon = FontAwesome6.Solid.RotateRight,
                                        Action = () => screen.RestartMap()
                                    },
                                    new()
                                    {
                                        Text = "Quit",
                                        Color = Theme.Red,
                                        SubText = "Bye bye",
                                        Icon = FontAwesome6.Solid.DoorOpen,
                                        Action = () => screen.Exit()
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        screen.IsPaused.BindValueChanged(e =>
        {
            if (e.NewValue)
                Show();
            else
                Hide();
        });
    }

    public override void Hide()
    {
        clock.RateTo(screen.Rate, 400, Easing.In);

        background.MoveToX(1.2f, 500, Easing.InQuint);
        content.ScaleTo(.8f, 400, Easing.InQuint).Delay(100).FadeOut(200);
    }

    public override void Show()
    {
        clock.RateTo(0, 400, Easing.Out);

        background.MoveToX(-1.2f).MoveToX(-.2f, 750, Easing.OutQuint);
        content.ScaleTo(1, 1000, Easing.OutElastic).FadeIn(200);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (!screen.IsPaused.Value)
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
