using System;
using fluXis.Database.Maps;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Buttons.Presets;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Footer;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Types;
using fluXis.Localization;
using fluXis.Map;
using fluXis.Overlay.Settings;
using fluXis.Scoring;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Screens.Select.Footer.Options;

public partial class FooterOptions : FocusedOverlayContainer
{
    protected override bool StartHidden => true;
    public FooterButton Button { get; set; }

    public Action<RealmMapSet> ExportAction { get; init; }
    public Action<RealmMap> EditAction { get; init; }
    public Action<RealmMapSet> DeleteAction { get; init; }
    public Action ScoresWiped { get; init; }

    [Resolved]
    private SettingsMenu settings { get; set; }

    [Resolved]
    private MapStore maps { get; set; }

    [Resolved]
    private ScoreManager scores { get; set; }

    [Resolved]
    private PanelContainer panels { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private FluXisGame game { get; set; }

    private FooterOptionSection setSection;
    private FooterOptionButton viewOnlineButton;
    private FooterOptionButton exportButton;

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
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Theme.Background2
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Masking = true,
                CornerRadius = 20,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Theme.Background2
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
                                Title = LocalizationStrings.General.General
                            },
                            new FooterOptionButton
                            {
                                Text = LocalizationStrings.SongSelect.OptionsSettings,
                                Icon = FontAwesome6.Solid.Gear,
                                Hotkey = Key.S,
                                Action = () =>
                                {
                                    settings.Show();
                                    State.Value = Visibility.Hidden;
                                }
                            },
                            setSection = new FooterOptionSection
                            {
                                Title = LocalizationStrings.SongSelect.OptionsForAll
                            },
                            viewOnlineButton = new FooterOptionButton
                            {
                                Text = LocalizationStrings.General.ViewOnline,
                                Icon = FontAwesome6.Solid.EarthAmericas,
                                Hotkey = Key.O,
                                Action = () =>
                                {
                                    game?.PresentMapSet(maps.CurrentMapSet.OnlineID);
                                    State.Value = Visibility.Hidden;
                                }
                            },
                            exportButton = new FooterOptionButton
                            {
                                Text = LocalizationStrings.General.Export,
                                Icon = FontAwesome6.Solid.BoxOpen,
                                Hotkey = Key.X,
                                Action = () =>
                                {
                                    ExportAction?.Invoke(maps.CurrentMapSet);
                                    State.Value = Visibility.Hidden;
                                }
                            },
                            new FooterOptionButton
                            {
                                Text = LocalizationStrings.SongSelect.OptionsDeleteSet,
                                Icon = FontAwesome6.Solid.Trash,
                                Color = Theme.Red,
                                Hotkey = Key.D,
                                Action = () =>
                                {
                                    DeleteAction?.Invoke(maps.CurrentMapSet);
                                    State.Value = Visibility.Hidden;
                                }
                            },
                            mapSection = new FooterOptionSection
                            {
                                Title = LocalizationStrings.SongSelect.OptionsForCurrent
                            },
                            new FooterOptionButton
                            {
                                Text = LocalizationStrings.General.Edit,
                                Icon = FontAwesome6.Solid.Pen,
                                Hotkey = Key.E,
                                Action = () =>
                                {
                                    EditAction?.Invoke(maps.CurrentMap);
                                    State.Value = Visibility.Hidden;
                                }
                            },
                            new FooterOptionButton
                            {
                                Text = LocalizationStrings.SongSelect.OptionsWipeScores,
                                Icon = FontAwesome6.Solid.Eraser,
                                Color = Theme.Red,
                                Hotkey = Key.W,
                                Action = () =>
                                {
                                    State.Value = Visibility.Hidden;

                                    panels.Content = new ButtonPanel
                                    {
                                        Icon = FontAwesome6.Solid.Eraser,
                                        Text = LocalizationStrings.SongSelect.WipeScoresConfirmation,
                                        SubText = LocalizationStrings.General.CanNotBeUndone,
                                        Buttons = new ButtonData[]
                                        {
                                            new DangerButtonData(LocalizationStrings.General.PanelGenericConfirm, () =>
                                            {
                                                scores.WipeFromMap(maps.CurrentMap.ID);
                                                ScoresWiped?.Invoke();
                                            }, true),
                                            new CancelButtonData()
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

        viewOnlineButton.Alpha = map.MapSet.OnlineID > 0 ? 1f : 0f;
        exportButton.Alpha = !map.MapSet.AutoImported ? 1f : 0f;

        setSection.SubTitle = $"{map.Metadata.LocalizedArtist} - {map.Metadata.LocalizedTitle}";
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

    protected override void PopIn() => this.FadeIn(200).MoveToY(0, 400, Easing.OutQuint);
    protected override void PopOut() => this.FadeOut(200).MoveToY(40, 400, Easing.OutQuint);
}
