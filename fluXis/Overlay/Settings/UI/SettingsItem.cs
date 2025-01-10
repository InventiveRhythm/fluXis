using System;
using fluXis.Audio;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Overlay.Settings.UI;

public abstract partial class SettingsItem : Container
{
    public LocalisableString Label { get; init; } = string.Empty;
    public LocalisableString Description { get; init; } = string.Empty;

    protected virtual bool IsDefault => true;

    public bool Padded { get; init; }

    public bool Enabled
    {
        get => EnabledBindable.Value;
        set => EnabledBindable.Value = value;
    }

    public BindableBool EnabledBindable { get; init; } = new(true);

    protected FillFlowContainer TextFlow { get; private set; }

    private bool isDefault;
    private ResetButton resetButton;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 40;

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
                Padding = new MarginPadding { Left = Padded ? 20 : 0 },
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

        EnabledBindable.BindValueChanged(e => this.FadeTo(e.NewValue ? 1 : .5f, 200), true);
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
        private HoverLayer hover;
        private FlashLayer flash;
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
                    hover = new HoverLayer(),
                    flash = new FlashLayer(),
                    icon = new FluXisSpriteIcon
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Icon = FontAwesome6.Solid.RotateLeft,
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
            flash.Show();
            samples.Click();
            ClickAction?.Invoke();
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
