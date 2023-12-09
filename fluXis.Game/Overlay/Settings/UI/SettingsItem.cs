using System;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
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
    private ResetButton resetButton;

    private const float reset_button_height = 30;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 40;
        Alpha = Enabled ? 1 : 0.5f;

        InternalChildren = new Drawable[]
        {
            resetButton = new ResetButton
            {
                ClickAction = Reset
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

    protected override void LoadComplete()
    {
        base.LoadComplete();
        updateResetButton();
    }

    protected override void Update()
    {
        if (isDefault == IsDefault) return;

        isDefault = IsDefault;
        updateResetButton();
    }

    private void updateResetButton()
    {
        if (IsDefault)
            resetButton.Hide();
        else
            resetButton.Show();
    }

    protected abstract void Reset();

    private partial class ResetButton : Container
    {
        [Resolved]
        private UISamples samples { get; set; }

        public Action ClickAction { get; init; }

        private Container content;
        private Box hover;
        private Box flash;
        private SpriteIcon icon;

        [BackgroundDependencyLoader]
        private void load()
        {
            Size = new Vector2(32);
            Anchor = Anchor.CentreLeft;
            Origin = Anchor.Centre;
            X = -10 - 16;
            Alpha = 0;
            AlwaysPresent = true;

            InternalChild = content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 10,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0
                    },
                    hover = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0
                    },
                    flash = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0
                    },
                    icon = new SpriteIcon
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Icon = FontAwesome.Solid.Undo,
                        Size = new Vector2(16)
                    }
                }
            };
        }

        public override void Show()
        {
            this.FadeIn(200);
            this.ScaleTo(1, 400, Easing.OutQuint);
            icon.RotateTo(40).RotateTo(0, 400, Easing.OutQuint);
        }

        public override void Hide()
        {
            this.FadeOut(200);
            this.ScaleTo(.9f, 400, Easing.OutQuint);
            icon.RotateTo(-40, 400, Easing.OutQuint);
        }

        protected override bool OnClick(ClickEvent e)
        {
            flash.FadeOutFromOne(1000, Easing.OutQuint);
            samples.Click();
            ClickAction?.Invoke();
            return true;
        }

        protected override bool OnHover(HoverEvent e)
        {
            hover.FadeTo(0.2f, 50);
            samples.Hover();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.FadeTo(0, 200);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            content.ScaleTo(0.9f, 1000, Easing.OutQuint);
            return true;
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            content.ScaleTo(1, 1000, Easing.OutElastic);
        }
    }
}
