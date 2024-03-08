using System.Collections.Generic;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using fluXis.Game.Screens.Multiplayer.Gameplay.Results;
using fluXis.Shared.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Multiplayer.Gameplay;

public partial class MultiResults : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override float Zoom => 1.2f;
    public override float BackgroundDim => 0.5f;
    public override float BackgroundBlur => 0.2f;

    private RealmMap map { get; }
    private List<ScoreInfo> scores { get; }

    private CornerButton backButton;

    public MultiResults(RealmMap map, List<ScoreInfo> scores)
    {
        this.map = map;
        this.scores = scores;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(100),
                Child = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    CornerRadius = 20,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FluXisColors.Background2
                        },
                        new GridContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            RowDimensions = new[]
                            {
                                new Dimension(GridSizeMode.AutoSize),
                                new Dimension()
                            },
                            Content = new[]
                            {
                                new Drawable[]
                                {
                                    new MultiResultsHeader(map)
                                },
                                new Drawable[]
                                {
                                    new MultiResultsScores(scores)
                                }
                            }
                        }
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 60,
                Masking = true,
                EdgeEffect = FluXisStyles.ShadowMediumNoOffset,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
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
            }
        };
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.FadeInFromZero(400, Easing.OutQuint);
        backButton.Show();
        base.OnEntering(e);
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        backButton.Hide();
        this.FadeOut(400, Easing.OutQuint);
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
