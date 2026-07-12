using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Overlay.Navigator;

#nullable enable

public partial class NavigatorBar : CompositeDrawable
{
    private readonly OnlineNavigator nav;
    private readonly Bindable<NavigatorPage?> page;

    private readonly Button back;
    private readonly Button refresh;
    private readonly FluXisSpriteText path;

    public NavigatorBar(OnlineNavigator nav, Bindable<NavigatorPage?> page)
    {
        this.nav = nav;
        this.page = page;

        RelativeSizeAxes = Axes.X;
        Height = OnlineNavigator.HEADER_HEIGHT;
        InternalChildren =
        [
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background2
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Padding = new MarginPadding { Horizontal = 16 },
                Spacing = new Vector2(16),
                Children =
                [
                    back = new Button(Phosphor.Bold.ArrowLeft) { Action = nav.Pop },
                    refresh = new Button(Phosphor.Bold.ArrowsClockwise) { Action = nav.Refresh },
                    path = new FluXisSpriteText
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        WebFontSize = 12,
                        Alpha = .8f
                    }
                ]
            }
        ];
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        refresh.Sample = samples.Get("UI/click-reload");
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        page.BindValueChanged(v =>
        {
            if (v.NewValue is null)
                return;

            path.Text = v.NewValue.Path;
            back.Enabled.Value = nav.PageCount > 1;
        }, true);
    }

    private partial class Button : ClickableContainer
    {
        [Resolved]
        private UISamples samples { get; set; } = null!;

        public Sample? Sample { get; set; }

        private readonly HoverLayer hover;
        private readonly FlashLayer flash;
        private readonly FluXisSpriteIcon icon;

        public Button(IconUsage icon)
        {
            Anchor = Origin = Anchor.CentreLeft;
            Size = new Vector2(24);
            CornerRadius = 4;
            Masking = true;

            InternalChildren =
            [
                hover = new HoverLayer(),
                flash = new FlashLayer(),
                this.icon = new FluXisSpriteIcon
                {
                    Icon = icon,
                    Size = new Vector2(16),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            ];
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            Enabled.BindValueChanged(v => icon.FadeTo(v.NewValue ? 1f : .5f), true);
        }

        protected override bool OnHover(HoverEvent e)
        {
            samples.Hover();
            hover.Show();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.Hide();
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (Sample != null) Sample.Play();
            else samples.Click();

            flash.Show();
            return base.OnClick(e);
        }
    }
}
