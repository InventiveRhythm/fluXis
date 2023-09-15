using fluXis.Game.Audio;
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

public partial class PauseMenu : CompositeDrawable
{
    [Resolved]
    private AudioClock clock { get; set; }

    public GameplayScreen Screen { get; set; }

    private Container background;
    private Container content;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

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
                            new GameplayMenuButton
                            {
                                Text = "Resume",
                                SubText = "Continue playing",
                                Icon = FontAwesome.Solid.Play,
                                Action = () => Screen.IsPaused.Value = false
                            },
                            new GameplayMenuButton
                            {
                                Text = "Restart",
                                SubText = "Try again?",
                                Icon = FontAwesome.Solid.Redo,
                                Action = () => Screen.RestartMap()
                            },
                            new GameplayMenuButton
                            {
                                Text = "Quit",
                                Color = FluXisColors.Red,
                                SubText = "Bye bye",
                                Icon = FontAwesome.Solid.DoorOpen,
                                Action = () => Screen.Exit()
                            }
                        }
                    }
                }
            }
        };

        Screen.IsPaused.BindValueChanged(e =>
        {
            if (e.NewValue)
                Show();
            else
                Hide();
        });
    }

    public override void Hide()
    {
        clock.RateTo(Screen.Rate, 400, Easing.InQuint);

        background.MoveToX(1.2f, 500, Easing.InQuint);
        content.ScaleTo(.8f, 400, Easing.InQuint).Delay(100).FadeOut(200);
    }

    public override void Show()
    {
        clock.RateTo(0);

        background.MoveToX(-1.2f).MoveToX(-.2f, 750, Easing.OutQuint);
        content.ScaleTo(1, 1000, Easing.OutElastic).FadeIn(200);
    }
}
