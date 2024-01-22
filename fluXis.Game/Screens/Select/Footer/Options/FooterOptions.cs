using System;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Database.Score;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Settings;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Select.Footer.Options;

public partial class FooterOptions : FocusedOverlayContainer
{
    protected override bool StartHidden => true;
    public SelectFooterButton Button { get; set; }
    public SelectFooter Footer { get; init; }
    public Action ScoresWiped { get; init; }

    [Resolved]
    private SettingsMenu settings { get; set; }

    [Resolved]
    private MapStore maps { get; set; }

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    private FooterOptionSection setSection;
    private FooterOptionSection mapSection;

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 300;
        AutoSizeAxes = Axes.Y;
        X = 450;
        Margin = new MarginPadding { Bottom = 100 };
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomCentre;

        InternalChildren = new Drawable[]
        {
            new Container
            {
                Size = new Vector2(40),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomRight,
                Rotation = 45,
                Y = 20,
                Masking = true,
                CornerRadius = 10,
                EdgeEffect = FluXisStyles.ShadowMedium,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Masking = true,
                CornerRadius = 20,
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
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(0, 5),
                        Padding = new MarginPadding(10),
                        Children = new Drawable[]
                        {
                            setSection = new FooterOptionSection
                            {
                                Title = "General"
                            },
                            new FooterOptionButton
                            {
                                Text = "Game Settings",
                                Icon = FontAwesome6.Solid.Gear,
                                Action = () =>
                                {
                                    settings.Show();
                                    State.Value = Visibility.Hidden;
                                }
                            },
                            setSection = new FooterOptionSection
                            {
                                Title = "For all difficulties"
                            },
                            new FooterOptionButton
                            {
                                Text = "Delete MapSet",
                                Icon = FontAwesome6.Solid.Trash,
                                Color = FluXisColors.Red,
                                Action = () =>
                                {
                                    Footer.Screen.OpenDeleteConfirm(maps.CurrentMapSet);
                                    State.Value = Visibility.Hidden;
                                }
                            },
                            mapSection = new FooterOptionSection
                            {
                                Title = "For this difficulty"
                            },
                            new FooterOptionButton
                            {
                                Text = "Edit Map",
                                Icon = FontAwesome6.Solid.Pen,
                                Action = () =>
                                {
                                    Footer.Screen.EditMapSet(maps.CurrentMap);
                                    State.Value = Visibility.Hidden;
                                }
                            },
                            new FooterOptionButton
                            {
                                Text = "Wipe local scores",
                                Icon = FontAwesome6.Solid.Eraser,
                                Color = FluXisColors.Red,
                                Action = () =>
                                {
                                    State.Value = Visibility.Hidden;

                                    game.Overlay = new ButtonPanel
                                    {
                                        Text = "Are you sure you want to wipe all local scores for this difficulty?",
                                        SubText = "This action cannot be undone.",
                                        ButtonWidth = 200,
                                        Buttons = new ButtonData[]
                                        {
                                            new()
                                            {
                                                Text = "Yes, do it.",
                                                Color = FluXisColors.ButtonRed,
                                                HoldToConfirm = true,
                                                Action = () =>
                                                {
                                                    realm.RunWrite(r =>
                                                    {
                                                        var scores = r.All<RealmScore>().Where(s => s.MapID == maps.CurrentMap.ID);
                                                        r.RemoveRange(scores);
                                                    });

                                                    ScoresWiped?.Invoke();
                                                }
                                            },
                                            new()
                                            {
                                                Text = "No, nevermind."
                                            }
                                        }
                                    };
                                }
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

        maps.MapBindable.BindValueChanged(mapChanged, true);
    }

    private void mapChanged(ValueChangedEvent<RealmMap> e)
    {
        var map = e.NewValue;

        setSection.SubTitle = $"{map.Metadata.Artist} - {map.Metadata.Title}";
        mapSection.SubTitle = map.Difficulty;
    }

    protected override void Update()
    {
        base.Update();

        var delta = Button.ScreenSpaceDrawQuad.Centre.X - ScreenSpaceDrawQuad.Centre.X;
        X += delta;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        maps.MapBindable.ValueChanged -= mapChanged;
    }

    protected override bool OnHover(HoverEvent e) => true;
    protected override bool OnClick(ClickEvent e) => true;
    protected override bool OnDragStart(DragStartEvent e) => true;
    protected override bool OnScroll(ScrollEvent e) => true;

    protected override void OnFocusLost(FocusLostEvent e) => Hide();

    protected override void PopIn()
    {
        this.FadeIn(200).MoveToY(0, 400, Easing.OutQuint);
    }

    protected override void PopOut()
    {
        this.FadeOut(200).MoveToY(40, 400, Easing.OutQuint);
    }
}
