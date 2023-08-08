using System;
using fluXis.Game.Audio;
using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.UI;

public partial class PauseMenu : CompositeDrawable
{
    [Resolved]
    private AudioClock clock { get; set; }

    public GameplayScreen Screen { get; set; }

    private Container background;
    private FluXisSpriteText title;
    private FluXisSpriteText subtitle;
    private FillFlowContainer buttons;

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
            title = new FluXisSpriteText
            {
                Text = "Paused",
                FontSize = 100,
                Alpha = 0,
                Margin = new MarginPadding { Left = 100, Top = 100 },
                Shadow = true
            },
            subtitle = new FluXisSpriteText
            {
                Text = "What do you want to do?",
                FontSize = 48,
                Alpha = 0,
                Margin = new MarginPadding { Left = 100, Top = 180 },
                Colour = Colour4.Gray,
                Shadow = true
            },
            buttons = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(10),
                Alpha = 0,
                Margin = new MarginPadding { Left = 100 },
                Children = new Drawable[]
                {
                    new PauseButton
                    {
                        Text = "Resume",
                        Description = "Continue playing",
                        Icon = FontAwesome.Solid.Play,
                        Action = () => Screen.IsPaused.Value = false
                    },
                    new PauseButton
                    {
                        Text = "Restart",
                        Description = "Try again?",
                        Icon = FontAwesome.Solid.Redo,
                        Action = () => Screen.RestartMap()
                    },
                    new PauseButton
                    {
                        Text = "Quit",
                        Description = "Bye bye",
                        Icon = FontAwesome.Solid.DoorOpen,
                        Action = () => Screen.Exit()
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
        clock.RateTo(Screen.Rate, 600, Easing.InQuint);

        background.MoveToX(1.2f, 500, Easing.InQuint);
        title.MoveToX(100, 400, Easing.InQuint).Delay(100).FadeOut(200);
        subtitle.Delay(40).MoveToX(100, 400, Easing.InQuint).Delay(100).FadeOut(200);
        buttons.Delay(80).MoveToX(100, 400, Easing.InQuint).Delay(100).FadeOut(200);
    }

    public override void Show()
    {
        clock.RateTo(0);

        background.MoveToX(-1.2f).MoveToX(-.2f, 750, Easing.OutQuint);
        title.MoveToX(-100).FadeInFromZero(200).MoveToX(0, 400, Easing.OutQuint);
        subtitle.MoveToX(-100).FadeOut().Delay(40).FadeIn(200).MoveToX(0, 400, Easing.OutQuint);
        buttons.MoveToX(-100).FadeOut().Delay(80).FadeIn(200).MoveToX(0, 400, Easing.OutQuint);
    }

    private partial class PauseButton : Container
    {
        public string Text;
        public string Description;
        public IconUsage Icon;
        public Action Action;

        public PauseButton()
        {
            Height = 100;
            Width = 400;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.White,
                    Alpha = 0
                },
                new SpriteIcon
                {
                    Icon = Icon,
                    Size = new Vector2(80),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Shadow = true
                },
                new FluXisSpriteText
                {
                    Text = Text,
                    FontSize = 48,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.BottomLeft,
                    Margin = new MarginPadding { Left = 90 },
                    Y = 10,
                    Shadow = true
                },
                new FluXisSpriteText
                {
                    Text = Description,
                    FontSize = 32,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.TopLeft,
                    Margin = new MarginPadding { Left = 90 },
                    Colour = Colour4.Gray,
                    Shadow = true
                }
            };
        }

        protected override bool OnClick(ClickEvent e)
        {
            Action?.Invoke();
            return true;
        }
    }
}
