using fluXis.Configuration;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Shaders;
using fluXis.Graphics.Shaders.Steps;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Input;
using fluXis.Skinning;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Screens.Gameplay.UI.Menus;

public partial class FailMenu : Container, IKeyBindingHandler<FluXisGlobalKeybind>
{
    [Resolved]
    private GameplayScreen screen { get; set; }

    private GameplaySamples samples => screen.Samples;

    private Box dim;
    private FillFlowContainer flow;
    private Container buttonsContainer;
    private SelectionCycleContainer<GameplayMenuButton> buttons;
    private Drawable flash;

    private Bindable<bool> dimOnLowHealth;
    private bool failed;

    private Container textContainer;
    private FluXisSpriteText text;
    private ShaderTransformHandler glitchShader;

    [BackgroundDependencyLoader]
    private void load(ISkin skin, FluXisConfig config)
    {
        dimOnLowHealth = config.GetBindable<bool>(FluXisSetting.DimAndFade);

        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        InternalChildren = new[]
        {
            dim = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black.Opacity(.5f),
                Alpha = dimOnLowHealth.Value ? 0 : 1
            },
            flow = new FillFlowContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, 40),
                AutoSizeEasing = Easing.OutQuint,
                Children = new Drawable[]
                {
                    textContainer = new Container
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(300, 150),
                        Child = createGlitchyText().With(x => x.Anchor = x.Origin = Anchor.Centre)
                    },
                    buttonsContainer = new Container
                    {
                        Width = 300,
                        AutoSizeAxes = Axes.Y,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        CornerRadius = 20,
                        Masking = true,
                        Alpha = 0,
                        EdgeEffect = Styling.ShadowMedium,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Theme.Background2
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
                                        Icon = Phosphor.Bold.ArrowClockwise,
                                        Action = () => screen?.RestartMap()
                                    },
                                    new()
                                    {
                                        Text = "Quit",
                                        Color = Theme.Red,
                                        SubText = "Bye bye",
                                        Icon = Phosphor.Bold.DoorOpen,
                                        Action = () => screen?.Exit()
                                    }
                                }
                            }
                        }
                    }
                }
            },
            flash = skin.GetFailFlash().With(d =>
            {
                d.RelativeSizeAxes = Axes.Both;
                d.Size = Vector2.One;
            })
        };
    }

    private Drawable createGlitchyText()
    {
        var stack = new ShaderStackContainer
        {
            RelativeSizeAxes = Axes.None,
            Size = new Vector2(300),
            BackgroundColour = Theme.Text.Opacity(0)
        };

        glitchShader = stack.AddShader(new Glitch2ShaderStep());
        stack.AddContent(text = new FluXisSpriteText
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Text = "FAILED",
            FontSize = 100
        });
        return stack;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        dimOnLowHealth.BindValueChanged(_ => dim.FadeTo(dimOnLowHealth.Value ? 0 : 1, 300), true);
    }

    public override void Show()
    {
        this.FadeIn();
        flash.FadeOutFromOne(2400);

        glitchShader.StrengthTo(1f).Strength2To(0.8f)
                    .StrengthTo(0.2f, 1200, Easing.OutQuint).Strength2To(0.2f, 1200, Easing.OutQuint);

        failed = true;
        samples.Fail();

        textContainer.ScaleTo(8f).ScaleTo(1, 1200, Easing.OutQuint);

        text.RotateTo(280)
            .RotateTo(-6, 1200, Easing.OutQuint)
            .Then(400).FadeIn().OnComplete(_ =>
            {
                flow.AutoSizeDuration = 600;
                buttonsContainer.FadeIn(300);
            });
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
