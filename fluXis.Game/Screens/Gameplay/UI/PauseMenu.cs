using System;
using fluXis.Game.Audio;
using fluXis.Game.Graphics;
using fluXis.Game.Overlay.Mouse;
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
    private GlobalCursorOverlay cursorOverlay { get; set; }

    [Resolved]
    private AudioClock clock { get; set; }

    public GameplayScreen Screen;

    public PauseMenu(GameplayScreen screen)
    {
        Screen = screen;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = 0.4f
            },
            new SpriteText
            {
                Text = "Paused",
                Font = FluXisFont.Default(100),
                Margin = new MarginPadding { Left = 100, Top = 100 },
                Shadow = true
            },
            new SpriteText
            {
                Text = "What do you want to do?",
                Font = FluXisFont.Default(48),
                Margin = new MarginPadding { Left = 100, Top = 180 },
                Colour = Colour4.Gray,
                Shadow = true
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(10),
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
        clock.RateTo(Screen.Rate);
        this.FadeOut(200);
        cursorOverlay.ShowCursor = false;
    }

    public override void Show()
    {
        clock.RateTo(0);
        this.FadeIn(200);
        cursorOverlay.ShowCursor = true;
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
                new SpriteText
                {
                    Text = Text,
                    Font = FluXisFont.Default(48),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.BottomLeft,
                    Margin = new MarginPadding { Left = 90 },
                    Y = 10,
                    Shadow = true
                },
                new SpriteText
                {
                    Text = Description,
                    Font = FluXisFont.Default(32),
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
