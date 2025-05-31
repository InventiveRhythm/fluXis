using System;
using fluXis.Audio;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API.Requests.Auth.Multifactor;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Auth.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Overlay.Auth;

public partial class MultifactorOverlay : CompositeDrawable
{
    [Resolved]
    private IAPIClient api { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    private Container content;
    private FluXisSpriteText errorText;
    private AuthOverlayTextBox code;
    private Container loadingLayer;

    private Action onComplete;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        InternalChildren = new Drawable[]
        {
            new FullInputBlockingContainer
            {
                RelativeSizeAxes = Axes.Both,
                OnClickAction = Hide,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black,
                    Alpha = .5f
                }
            },
            content = new ClickableContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                CornerRadius = 20,
                Masking = true,
                EdgeEffect = FluXisStyles.ShadowMedium,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2
                    },
                    new FillFlowContainer
                    {
                        Width = 540,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Padding = new MarginPadding(40),
                        Spacing = new Vector2(10),
                        Children = new[]
                        {
                            new FluXisSpriteText
                            {
                                Text = "Multi-factor Authentification",
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                WebFontSize = 32
                            },
                            errorText = new TruncatingText
                            {
                                Text = "error message",
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                MaxWidth = 300,
                                Colour = FluXisColors.Red,
                                WebFontSize = 14,
                                Alpha = 0
                            },
                            Empty().With(d => d.Anchor = d.Origin = Anchor.TopCentre),
                            code = new AuthOverlayTextBox
                            {
                                TabbableContentContainer = this,
                                PlaceholderText = "Code"
                            },
                            Empty().With(d => d.Anchor = d.Origin = Anchor.TopCentre),
                            new AuthOverlayButton("Continue") { Action = confirmCode }
                        }
                    },
                    loadingLayer = new FullInputBlockingContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Colour4.Black,
                                Alpha = .5f
                            },
                            new LoadingIcon
                            {
                                Size = new Vector2(48),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre
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

        code.OnCommit += (_, _) => confirmCode();
    }

    private void confirmCode()
    {
        setError("");

        if (string.IsNullOrEmpty(code.Text))
        {
            setError("Code cannot be empty.");
            return;
        }

        var req = new TOTPVerifyRequest(code.Text);

        req.Failure += ex =>
        {
            setError(ex.Message);
            loadingLayer.FadeOut(300);
        };

        req.Success += res =>
        {
            api.MultifactorToken = res.Data.Token;
            loadingLayer.FadeOut(300);
            Hide();
            onComplete?.Invoke();
        };

        loadingLayer.FadeIn(300);
        api.PerformRequestAsync(req);
    }

    private void setError(string msg)
    {
        if (string.IsNullOrEmpty(msg))
        {
            errorText.Alpha = 0;
            return;
        }

        errorText.Text = msg;
        errorText.Alpha = 1;
    }

    public override void Show() => Show(() => { });

    public void Show(Action onComplete)
    {
        this.onComplete = onComplete;

        this.FadeInFromZero(400, Easing.OutQuint);
        content.ScaleTo(.75f).ScaleTo(1f, 800, Easing.OutElasticHalf);
        samples.Overlay(false);

        Schedule(() => GetContainingFocusManager()?.ChangeFocus(code));
        loadingLayer.FadeOut();
    }

    public override void Hide()
    {
        this.FadeOut(400, Easing.OutQuint);
        content.ScaleTo(.9f, 400, Easing.OutQuint);
        samples.Overlay(true);
    }
}

