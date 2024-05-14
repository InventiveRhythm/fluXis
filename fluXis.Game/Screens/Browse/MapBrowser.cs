using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Audio.Preview;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Context;
using fluXis.Game.Input;
using fluXis.Game.Localization;
using fluXis.Game.Map.Drawables;
using fluXis.Game.Online.Activity;
using fluXis.Game.Online.API.Models.Maps;
using fluXis.Game.Online.API.Requests.MapSets;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Screens.Browse.Info;
using fluXis.Game.Screens.Browse.Search;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Browse;

public partial class MapBrowser : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override float BackgroundDim => 0.75f;
    public override float BackgroundBlur => 0.5f;
    public override bool AllowMusicControl => false;
    public override UserActivity InitialActivity => new UserActivity.BrowsingMaps();

    [Resolved]
    private FluxelClient fluxel { get; set; }

    [Resolved]
    private GlobalClock clock { get; set; }

    [Resolved]
    private PreviewManager previews { get; set; }

    private readonly Bindable<APIMapSet> selectedSet = new();

    public FillFlowContainer<MapCard> Maps { get; private set; }
    private CornerButton backButton;
    private LoadingIcon loadingIcon;
    private FluXisSpriteText text;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Spacing = new Vector2(10),
                Padding = new MarginPadding
                {
                    Horizontal = 120,
                    Vertical = 40
                },
                Children = new Drawable[]
                {
                    new SpriteIcon
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Icon = FontAwesome6.Solid.EarthAmericas,
                        Size = new Vector2(20)
                    },
                    new FluXisSpriteText
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        FontSize = 24,
                        Text = "Online Map Browser"
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(100),
                Child = new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    ColumnDimensions = new Dimension[]
                    {
                        new(),
                        new(GridSizeMode.Absolute, 40),
                        new(GridSizeMode.Absolute, 780)
                    },
                    Content = new[]
                    {
                        new[]
                        {
                            new GridContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                RowDimensions = new Dimension[]
                                {
                                    new(GridSizeMode.AutoSize),
                                    new(GridSizeMode.Absolute, 20),
                                    new()
                                },
                                Content = new[]
                                {
                                    new Drawable[] { new BrowserSearchBar { MapBrowser = this } },
                                    new[] { Empty() },
                                    new Drawable[]
                                    {
                                        new FluXisContextMenuContainer
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Child = new FluXisScrollContainer
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                ScrollbarAnchor = Anchor.TopLeft,
                                                Child = Maps = new FillFlowContainer<MapCard>
                                                {
                                                    RelativeSizeAxes = Axes.X,
                                                    AutoSizeAxes = Axes.Y,
                                                    Direction = FillDirection.Full,
                                                    Spacing = new Vector2(20)
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            Empty(),
                            new BrowseInfo
                            {
                                BindableSet = selectedSet
                            }
                        }
                    }
                }
            },
            backButton = new CornerButton
            {
                Icon = FontAwesome6.Solid.ChevronLeft,
                ButtonText = LocalizationStrings.General.Back,
                Action = this.Exit
            },
            text = new FluXisSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0,
                FontSize = 30
            },
            loadingIcon = new LoadingIcon
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(100)
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (fluxel.Status != ConnectionStatus.Online)
        {
            text.Text = "You are not connected to the server!";
            text.FadeInFromZero(20);
            loadingIcon.Hide();
            return;
        }

        loadMapsets();
    }

    private void loadMapsets()
    {
        var req = new MapSetsRequest();
        req.PerformAsync(fluxel);
        req.Success += response =>
        {
            if (response.Status != 200)
            {
                Logger.Log($"Failed to load mapsets: {response.Status} {response.Message}", LoggingTarget.Network);
                loadingIcon.Hide();
                return;
            }

            foreach (var mapSet in response.Data)
            {
                Maps.Add(new MapCard(mapSet)
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    EdgeEffect = FluXisStyles.ShadowSmall,
                    OnClickAction = set =>
                    {
                        previews.PlayPreview(set.ID);
                        selectedSet.Value = set;
                    }
                });
            }

            loadingIcon.Hide();
        };
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.FadeOut();

        using (BeginDelayedSequence(ENTER_DELAY))
        {
            this.FadeInFromZero(FADE_DURATION);
            backButton.Show();

            clock.FadeOut(FADE_DURATION);
            clock.Delay(FADE_DURATION).OnComplete(_ => clock.Stop());
        }
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        this.FadeOut(FADE_DURATION);

        backButton.Hide();
        previews.StopPreview();

        clock.Start();
        clock.FadeIn(FADE_DURATION);
        return false;
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

    public void Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            foreach (var map in Maps)
                map.Show();

            return;
        }

        foreach (var map in Maps)
        {
            var matches = false;
            matches |= map.MapSet.Title?.ToLower().Contains(query.ToLower()) ?? false;
            matches |= map.MapSet.Artist?.ToLower().Contains(query.ToLower()) ?? false;
            matches |= map.MapSet.Creator?.Username.ToLower().Contains(query.ToLower()) ?? false;
            matches |= map.MapSet.Creator?.DisplayName?.ToLower().Contains(query.ToLower()) ?? false;
            matches |= map.MapSet.Tags?.Any(tag => tag.ToLower().Contains(query.ToLower())) ?? false;
            matches |= map.MapSet.Source?.ToLower().Contains(query.ToLower()) ?? false;

            if (matches)
                map.Show();
            else
                map.Hide();
        }
    }
}
