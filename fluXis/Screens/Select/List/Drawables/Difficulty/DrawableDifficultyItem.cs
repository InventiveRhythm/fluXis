using System;
using System.Collections.Generic;
using fluXis.Database.Maps;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Localization;
using fluXis.Map;
using fluXis.Map.Drawables;
using fluXis.Screens.Select.List.Items;
using fluXis.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Screens.Select.List.Drawables.Difficulty;

public partial class DrawableDifficultyItem : CompositeDrawable, IHasContextMenu
{
    public MenuItem[] ContextMenuItems
    {
        get
        {
            List<MenuItem> items = new()
            {
                !Equals(maps.CurrentMap, map)
                    ? new FluXisMenuItem(LocalizationStrings.General.Select, FontAwesome6.Solid.ArrowRight, MenuItemType.Highlighted, () => maps.Select(map, true))
                    : new FluXisMenuItem(LocalizationStrings.General.Play, FontAwesome6.Solid.Play, MenuItemType.Highlighted, () => SelectAction?.Invoke()),
                new FluXisMenuItem(LocalizationStrings.General.Edit, FontAwesome6.Solid.Pen, MenuItemType.Normal, () => EditAction?.Invoke(map)),
                new FluXisMenuItem(LocalizationStrings.General.Export, FontAwesome6.Solid.BoxOpen, MenuItemType.Normal, () => ExportAction?.Invoke(map.MapSet)) { Enabled = () => !map.MapSet.AutoImported },
                new FluXisMenuItem(LocalizationStrings.General.Delete, FontAwesome6.Solid.Trash, MenuItemType.Dangerous, () => DeleteAction?.Invoke(map.MapSet)) { Enabled = () => !map.MapSet.AutoImported }
            };

            if (FluXisGameBase.IsDebug)
            {
                if (map.OnlineID > 0)
                    items.Add(new FluXisMenuItem("Copy Online ID", FontAwesome6.Solid.Copy, MenuItemType.Normal, () => clipboard?.SetText(map.OnlineID.ToString())));

                items.Add(new FluXisMenuItem("Copy ID", FontAwesome6.Solid.Copy, MenuItemType.Normal, () => clipboard?.SetText(map.ID.ToString())));
            }

            return items.ToArray();
        }
    }

    [Resolved]
    private MapStore maps { get; set; }

    [Resolved]
    private Clipboard clipboard { get; set; }

    public Action SelectAction;
    public Action<RealmMap> EditAction;
    public Action<RealmMapSet> ExportAction;
    public Action<RealmMapSet> DeleteAction;

    private MapDifficultyItem item { get; }
    private RealmMap map { get; }

    private Box colorBrighten;

    private DelayedLoadWrapper backgroundWrapper;
    private SectionedGradient gradientBlack;
    private SectionedGradient gradientColor;

    private Container content;
    private DelayedLoadWrapper contentWrapper;
    private RoundedOutline outline;
    private Container arrow;

    private float contentPadding
    {
        get => content.Padding.Right;
        set => content.Padding = new MarginPadding { Right = arrow.Width = value };
    }

    public DrawableDifficultyItem(MapDifficultyItem item, RealmMap map)
    {
        this.item = item;
        this.map = map;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 80;
        CornerRadius = 10;
        Masking = true;

        var color = map.Metadata.Color;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = color
            },
            colorBrighten = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.White.Opacity(.5f),
                Alpha = 0
            },
            content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Right = 10 },
                Child = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    CornerRadius = 10,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        backgroundWrapper = new DelayedLoadUnloadWrapper(() => new MapBackground(map, true)
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        }, 100, 200)
                        {
                            RelativeSizeAxes = Axes.Both
                        },
                        gradientBlack = new SectionedGradient
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Colour4.Black,
                            Alpha = .4f
                        },
                        gradientColor = new SectionedGradient
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = color,
                            Alpha = .4f
                        },
                        contentWrapper = new DelayedLoadUnloadWrapper(createContent, 100, 200)
                        {
                            RelativeSizeAxes = Axes.Both
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Width = .5f,
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            Padding = new MarginPadding { Right = -1 }, // to fix the 1px seam
                            Children = new Drawable[]
                            {
                                new RoundedOutline
                                {
                                    BorderColour = ColourInfo.GradientHorizontal(color.Opacity(0), color),
                                },
                                outline = new RoundedOutline
                                {
                                    BorderColour = ColourInfo.GradientHorizontal(Colour4.White.Opacity(0), Colour4.White.Opacity(.5f)),
                                    Alpha = 0
                                }
                            }
                        }
                    }
                }
            },
            arrow = new Container
            {
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Child = new FluXisSpriteIcon
                {
                    X = -2,
                    Icon = FontAwesome6.Solid.AngleLeft,
                    Size = new Vector2(16),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = color
                }
            }
        };

        backgroundWrapper.DelayedLoadComplete += d => d.FadeInFromZero(300);
        contentWrapper.DelayedLoadComplete += d => d.FadeInFromZero(300);
    }

    private Drawable createContent()
    {
        var modeColor = FluXisColors.GetKeyColor(map.KeyCount);

        return new Container
        {
            RelativeSizeAxes = Axes.Both,
            Padding = new MarginPadding { Right = 12 },
            Child = new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                ColumnDimensions = new[]
                {
                    new Dimension(GridSizeMode.Absolute, 80),
                    new Dimension(GridSizeMode.Absolute, 10),
                    new Dimension(),
                    new Dimension(GridSizeMode.Absolute, 10),
                    new Dimension(GridSizeMode.AutoSize)
                },
                Content = new[]
                {
                    new[]
                    {
                        new LoadWrapper<MapCover>
                        {
                            RelativeSizeAxes = Axes.Both,
                            CornerRadius = 10,
                            Masking = true,
                            LoadContent = () => new MapCover(map.MapSet)
                            {
                                RelativeSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre
                            },
                            OnComplete = cover => cover.FadeInFromZero(300)
                        },
                        Empty(),
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Spacing = new Vector2(0, -5),
                            Children = new Drawable[]
                            {
                                new TruncatingText
                                {
                                    RelativeSizeAxes = Axes.X,
                                    WebFontSize = 20,
                                    Text = map.Metadata.Title,
                                    Shadow = true
                                },
                                new TruncatingText
                                {
                                    RelativeSizeAxes = Axes.X,
                                    WebFontSize = 14,
                                    Text = map.Metadata.Artist,
                                    Shadow = true
                                },
                                new FillFlowContainer
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Direction = FillDirection.Horizontal,
                                    Margin = new MarginPadding { Top = 8 },
                                    Spacing = new Vector2(6),
                                    Children = new Drawable[]
                                    {
                                        new RoundedChip
                                        {
                                            Height = 16,
                                            Text = $"{map.KeyCount}K",
                                            BackgroundColour = modeColor,
                                            TextColour = (FluXisColors.IsBright(modeColor) ? FluXisColors.Background2 : FluXisColors.Text).Opacity(.75f),
                                            EdgeEffect = FluXisStyles.ShadowSmall,
                                            WebFontSize = 10,
                                            SidePadding = 8
                                        },
                                        new TruncatingText
                                        {
                                            WebFontSize = 12,
                                            Text = map.Difficulty,
                                            Shadow = true
                                        }
                                    }
                                }
                            }
                        },
                        Empty(),
                        new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Vertical,
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            Spacing = new Vector2(6),
                            Children = new Drawable[]
                            {
                                new StatusTag(map)
                                {
                                    Size = new Vector2(0, 20),
                                    WebFontSize = 12,
                                    WidthStep = 20,
                                    EdgeEffect = FluXisStyles.ShadowSmall,
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight
                                },
                                new DifficultyChip
                                {
                                    Size = new Vector2(64, 20),
                                    WebFontSize = 12,
                                    Rating = map.Filters.NotesPerSecond,
                                    EdgeEffect = FluXisStyles.ShadowSmall,
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight
                                },
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        item.State.BindValueChanged(stateChanged, true);
    }

    protected override void Update()
    {
        base.Update();

        if (Precision.AlmostEquals(item.Position, Y))
            Y = item.Position;
        else
            Y = (float)Interpolation.Lerp(item.Position, Y, Math.Exp(-0.01 * Time.Elapsed));
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        item.State.ValueChanged -= stateChanged;
    }

    private void stateChanged(ValueChangedEvent<SelectedState> e)
    {
        if (e.NewValue == SelectedState.Selected)
        {
            this.TransformTo(nameof(contentPadding), 27f, 400, Easing.OutQuint);
            colorBrighten.FadeIn(200);
            outline.FadeIn(200);
        }
        else
        {
            this.TransformTo(nameof(contentPadding), 10f, 400, Easing.OutQuint);
            colorBrighten.FadeOut(200);
            outline.FadeOut(200);
        }
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (item.State.Value == SelectedState.Selected)
        {
            SelectAction?.Invoke();
            return true;
        }

        maps.Select(map, true);
        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        gradientBlack.FadeTo(.2f, 50);
        gradientColor.FadeTo(.2f, 50);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        gradientBlack.FadeTo(.4f, 200);
        gradientColor.FadeTo(.4f, 200);
    }
}
