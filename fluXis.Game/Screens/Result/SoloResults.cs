using System;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using fluXis.Game.Online.API.Requests.Scores;
using fluXis.Game.Screens.Result.Extended;
using fluXis.Game.Screens.Result.Normal;
using fluXis.Game.UI;
using fluXis.Shared.Components.Users;
using fluXis.Shared.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Result;

public partial class SoloResults : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override float Zoom => 1.2f;
    public override float BackgroundDim => 0.5f;
    public override float BackgroundBlur => 0.2f;

    [Resolved]
    private GlobalBackground backgrounds { get; set; }

    public ScoreInfo Score { get; }
    public RealmMap Map { get; }
    public APIUserShort Player { get; }

    public ScoreInfo ComparisonScore { get; set; }
    public ScoreSubmitRequest SubmitRequest { get; set; }

    private DependencyContainer dependencies;

    public Action OnRestart;
    private bool extendedState;

    private NormalResults normal;
    private ExtendedResults extended;

    private CornerButton backButton;
    private CornerButton retryButton;

    public SoloResults(RealmMap map, ScoreInfo score, APIUserShort player)
    {
        Map = map;
        Score = score;
        Player = player;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        dependencies.CacheAs(this);
        dependencies.CacheAs(Score);
        dependencies.CacheAs(Map);
        dependencies.CacheAs(Player ?? APIUserShort.Default);

        InternalChildren = new Drawable[]
        {
            normal = new NormalResults(),
            extended = new ExtendedResults(),
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 60,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                        EdgeEffect = FluXisStyles.ShadowMediumNoOffset,
                        Child = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FluXisColors.Background2
                        }
                    },
                    backButton = new CornerButton
                    {
                        ButtonText = "Back",
                        Icon = FontAwesome6.Solid.ChevronLeft,
                        Action = this.Exit
                    },
                    retryButton = new CornerButton
                    {
                        ButtonText = "Retry",
                        Corner = Corner.BottomRight,
                        Icon = FontAwesome6.Solid.RotateRight,
                        ButtonColor = FluXisColors.Accent2,
                        Alpha = OnRestart is not null ? 1f : 0f,
                        Action = () => OnRestart?.Invoke()
                    }
                }
            }
        };
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    protected override void LoadComplete()
    {
        base.LoadComplete();

        extended.Alpha = 0f;
        extended.Y = 100f;
    }

    private void setState(bool state)
    {
        if (extendedState == state) return;

        extendedState = state;

        backgrounds.SetBlur(state ? .5f : .2f, 400);
        backgrounds.SetDim(state ? .8f : .5f, 400);

        normal.Delay(state ? 0 : 200).FadeTo(state ? 0f : 1f, 200).MoveToY(state ? -100 : 0, 400, Easing.Out);
        extended.Delay(state ? 200 : 0).FadeTo(state ? 1f : 0f, 200).MoveToY(state ? 0 : 100, 400, Easing.Out);
    }

    protected override bool OnScroll(ScrollEvent e)
    {
        setState(e.ScrollDelta.Y < 0);
        return true;
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.FadeInFromZero(400, Easing.OutQuint);
        normal.ScaleTo(.8f).ScaleTo(1f, 600, Easing.OutQuint);

        backButton.Show();
        retryButton.Show();
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        this.FadeOut(400, Easing.OutQuint);
        normal.ScaleTo(.8f, 600, Easing.OutQuint);
        extended.ScaleTo(.8f, 600, Easing.OutQuint);

        backButton.Hide();
        retryButton.Hide();
        return base.OnExiting(e);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back:
                this.Exit();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
