using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Database.Score;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Gamepad;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using fluXis.Game.Localization;
using fluXis.Game.Map;
using fluXis.Game.Online.Activity;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Models.Scores;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Scores;
using fluXis.Game.Screens.Result.UI;
using fluXis.Shared.Components.Users;
using fluXis.Shared.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Result;

public partial class ResultsScreen : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override float Zoom => 1.2f;
    public override bool AllowMusicControl => false;
    public override UserActivity InitialActivity => new UserActivity.Results();

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private FluxelClient fluxel { get; set; }

    private readonly RealmMap map;
    private readonly MapInfo mapInfo;
    private readonly ScoreInfo score;
    private readonly APIUser player;
    private readonly RealmScore realmScore;

    private Container content;
    private ResultsRatingInfo ratingInfo;
    private GamepadTooltipBar gamepadTooltips;
    private Container buttons;
    private CornerButton backButton;

    private readonly bool showPlayData;

    public ResultsScreen(RealmMap map, MapInfo mapInfo, ScoreInfo score, APIUser player, bool showPlayData = true, bool saveScore = true)
    {
        this.map = map;
        this.mapInfo = mapInfo;
        this.score = score;
        this.showPlayData = showPlayData;
        this.player = player;

        if (saveScore)
        {
            realmScore = new RealmScore(map)
            {
                PlayerID = player?.ID ?? -1,
                Accuracy = score.Accuracy,
                Rank = score.Rank,
                Score = score.Score,
                MaxCombo = score.MaxCombo,
                Flawless = score.Flawless,
                Perfect = score.Perfect,
                Great = score.Great,
                Alright = score.Alright,
                Okay = score.Okay,
                Miss = score.Miss,
                Mods = string.Join(' ', score.Mods)
            };
        }
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        InternalChildren = new Drawable[]
        {
            content = new Container
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                CornerRadius = 10,
                MaskingSmoothness = 0,
                Masking = true,
                EdgeEffect = FluXisStyles.ShadowMedium,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2
                    },
                    new DrawableBanner(player)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = ColourInfo.GradientVertical(FluXisColors.Background2.Opacity(.6f), FluXisColors.Background2)
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Y,
                        Width = 740,
                        Direction = FillDirection.Vertical,
                        Margin = new MarginPadding(20),
                        Children = new[]
                        {
                            new ResultTitle(map) { User = player },
                            new ResultScore { Score = score, MapInfo = mapInfo },
                            showPlayData ? new ResultHitPoints { MapInfo = mapInfo, Score = score } : Empty(),
                            ratingInfo = new ResultsRatingInfo(showPlayData)
                        }
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Height = 60,
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
                    buttons = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            backButton = new CornerButton
                            {
                                ButtonText = LocalizationStrings.General.Back,
                                Icon = FontAwesome6.Solid.ChevronLeft,
                                Action = this.Exit
                            }
                        }
                    },
                    gamepadTooltips = new GamepadTooltipBar
                    {
                        ShowBackground = false,
                        TooltipsLeft = new GamepadTooltip[]
                        {
                            new()
                            {
                                Text = LocalizationStrings.General.Back,
                                Icon = "B"
                            }
                        }
                    }
                }
            }
        };

        GamepadHandler.OnGamepadStatusChanged += onGamepadStatusChanged;
        onGamepadStatusChanged(GamepadHandler.GamepadConnected);

        if (realmScore != null && !map.MapSet.Managed)
        {
            realm.RunWrite(r => r.Add(realmScore));
            OnlineScores.UploadScore(fluxel, score, res => ratingInfo.ScoreResponse = res);
        }
        else
            ratingInfo.ScoreResponse = new APIResponse<APIScoreResponse>(400, "Score not submittable.", null);
    }

    private void onGamepadStatusChanged(bool status)
    {
        gamepadTooltips.FadeTo(status ? 1 : 0);
        buttons.FadeTo(status ? 0 : 1);
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

    public override void OnEntering(ScreenTransitionEvent e)
    {
        content.ScaleTo(0.9f).ScaleTo(1f, 400, Easing.OutQuint);
        this.FadeInFromZero(200);
        backButton.Show();
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        content.ScaleTo(1.1f, 400, Easing.OutQuint);
        this.FadeOut(200);
        backButton.Hide();

        return base.OnExiting(e);
    }
}
