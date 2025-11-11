using System;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Interaction;
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
    public bool HideWhenDisabled { get; init; }

    public bool Padded { get; init; }
    public bool SmallReset { get; init; }

    public bool Enabled
    {
        get => EnabledBindable.Value;
        set => EnabledBindable.Value = value;
    }

    public BindableBool EnabledBindable { get; init; } = new(true);

    protected FillFlowContainer TextFlow { get; private set; }

    protected new Container Content { get; } = new()
    {
        AutoSizeAxes = Axes.Both,
        Anchor = Anchor.CentreRight,
        Origin = Anchor.CentreRight
    };

    private bool isDefault;
    private ResetButton resetButton;
    private FluXisSpriteIcon rightIndicator;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 40;

        InternalChildren = new Drawable[]
        {
            resetButton = new ResetButton
            {
                Small = SmallReset,
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
                        WebFontSize = 16
                    },
                    new FluXisSpriteText
                    {
                        Text = Description,
                        WebFontSize = 12,
                        Alpha = .8f
                    }
                }
            },
            rightIndicator = new FluXisSpriteIcon
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Icon = FontAwesome6.Solid.AngleLeft,
                Size = new Vector2(16),
                Alpha = 0
            },
            Content.WithChild(CreateContent()),
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        UpdateResetButton();

        Content.FinishTransforms(true);

        EnabledBindable.BindValueChanged(e =>
        {
            var hideAlpha = HideWhenDisabled ? 0 : .5f;
            this.FadeTo(e.NewValue ? 1 : hideAlpha, HideWhenDisabled ? 0 : 200);
        }, true);
    }

    protected abstract Drawable CreateContent();

    protected override void Update()
    {
        if (isDefault == IsDefault) return;

        isDefault = IsDefault;
        UpdateResetButton();
    }

    protected void UpdateResetButton()
    {
        if (IsDefault)
            resetButton.Hide();
        else
            resetButton.Show();
    }

    protected abstract void Reset();

    protected override bool OnHover(HoverEvent e)
    {
        rightIndicator.MoveToX(32).FadeIn(50).MoveToX(24, 100, Easing.OutQuint);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        rightIndicator.FadeOut(200).MoveToX(32, 400, Easing.OutQuint);
    }

    private partial class ResetButton : Container
    {
        [Resolved]
        private UISamples samples { get; set; }

        public Action ClickAction { get; init; }

        public bool Small { get; init; }

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
            X = Small ? -16 : -26;
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
            this.ScaleTo(Small ? .8f : 1, 400, Easing.OutQuint);
            icon.RotateTo(40).RotateTo(0, 400, Easing.OutQuint);
        }

        public override void Hide()
        {
            this.FadeOut(200);
            this.ScaleTo(Small ? .7f : .9f, 400, Easing.OutQuint);
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
