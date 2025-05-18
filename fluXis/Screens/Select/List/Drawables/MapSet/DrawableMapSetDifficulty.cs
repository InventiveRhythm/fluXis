using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Database.Score;
using fluXis.Graphics.Drawables;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Localization;
using fluXis.Map;
using fluXis.Map.Drawables;
using fluXis.Online.API.Models.Maps;
using fluXis.Scoring;
using fluXis.Scoring.Enums;
using fluXis.Utils.Extensions;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Framework.Platform;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Screens.Select.List.Drawables.MapSet;

public partial class DrawableMapSetDifficulty : Container, IHasContextMenu, IComparable<DrawableMapSetDifficulty>
{
    public MenuItem[] ContextMenuItems
    {
        get
        {
            List<MenuItem> items = new()
            {
                new FluXisMenuItem(LocalizationStrings.General.Play, FontAwesome6.Solid.Play, MenuItemType.Highlighted, () =>
                {
                    maps.Select(map, true);
                    item.SelectAction?.Invoke();
                }),
                new FluXisMenuItem(LocalizationStrings.General.Edit, FontAwesome6.Solid.Pen, MenuItemType.Normal, () => item.EditAction?.Invoke(map))
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
    private ScoreManager scores { get; set; }

    [Resolved]
    private Clipboard clipboard { get; set; }

    private readonly DrawableMapSetItem item;
    private readonly RealmMap map;
    private Container outline;
    private DrawableScoreRank rank;
    private DifficultyChip ratingChip;

    public Action RequestedResort { get; set; }

    public float TargetY = 0;

    public DrawableMapSetDifficulty(DrawableMapSetItem parentEntry, RealmMap map)
    {
        item = parentEntry;
        this.map = map;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 48;

        var color = FluXisColors.GetKeyColor(map.KeyCount);

        InternalChildren = new Drawable[]
        {
            outline = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(-2),
                Child = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    CornerRadius = 12,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = ColourInfo.GradientVertical(color.Lighten(1), color)
                        }
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                CornerRadius = 10,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = color
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Right = 35 },
                        Child = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            CornerRadius = 10,
                            Masking = true,
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = FluXisColors.Background2
                                },
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Direction = FillDirection.Horizontal,
                                    Padding = new MarginPadding { Left = 12 },
                                    Spacing = new Vector2(8),
                                    Children = new Drawable[]
                                    {
                                        rank = new DrawableScoreRank
                                        {
                                            Size = new Vector2(24),
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            FontSize = FluXisSpriteText.GetWebFontSize(20),
                                            Shadow = false
                                        },
                                        new FillFlowContainer
                                        {
                                            AutoSizeAxes = Axes.Both,
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            Direction = FillDirection.Vertical,
                                            Children = new Drawable[]
                                            {
                                                new FillFlowContainer
                                                {
                                                    AutoSizeAxes = Axes.Both,
                                                    Direction = FillDirection.Horizontal,
                                                    Spacing = new Vector2(10),
                                                    Children = new Drawable[]
                                                    {
                                                        new FluXisSpriteText
                                                        {
                                                            Text = map.Difficulty,
                                                            FontSize = 20,
                                                            Anchor = Anchor.CentreLeft,
                                                            Origin = Anchor.CentreLeft
                                                        },
                                                        new FluXisSpriteText
                                                        {
                                                            Text = $"mapped by {map.Metadata.Mapper}",
                                                            FontSize = 16,
                                                            Anchor = Anchor.CentreLeft,
                                                            Origin = Anchor.CentreLeft,
                                                            Alpha = .8f
                                                        }
                                                    }
                                                },
                                                new FillFlowContainer
                                                {
                                                    AutoSizeAxes = Axes.Both,
                                                    Direction = FillDirection.Horizontal,
                                                    Children = new Drawable[]
                                                    {
                                                        ratingChip = new DifficultyChip
                                                        {
                                                            Width = 50,
                                                            Height = 14,
                                                            FontSize = 14,
                                                            Rating = map.Rating,
                                                            Margin = new MarginPadding { Right = 5 }
                                                        }
                                                    }.Concat(getIcons()).ToArray()
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new Container
                    {
                        Width = 35,
                        RelativeSizeAxes = Axes.Y,
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                        Colour = Colour4.Black,
                        Alpha = .75f,
                        Child = new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Direction = FillDirection.Horizontal,
                            Children = new Drawable[]
                            {
                                new FluXisSpriteText
                                {
                                    Text = map.KeyCount.ToString(),
                                    FontSize = 20,
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.BottomLeft,
                                    Margin = new MarginPadding { Bottom = -1 }
                                },
                                new FluXisSpriteText
                                {
                                    Text = "K",
                                    FontSize = 14,
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.BottomLeft
                                }
                            }
                        }
                    }
                }
            }
        };

        maps.MapBindable.BindValueChanged(updateSelected, true);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        map.RatingChanged += updateRating;
        scores.TopScoreUpdated += updateTopScore;
        updateTopScore(map.ID, scores.GetCurrentTop(map.ID));
    }

    public void UpdatePosition(double elapsed)
    {
        if (Precision.AlmostEquals(TargetY, Y))
            Y = TargetY;
        else
            Y = (float)Interpolation.Lerp(TargetY, Y, Math.Exp(-0.01 * elapsed));
    }

    protected override void Dispose(bool isDisposing)
    {
        maps.MapBindable.ValueChanged -= updateSelected;
        scores.TopScoreUpdated -= updateTopScore;
        map.RatingChanged -= updateRating;

        base.Dispose(isDisposing);
    }

    private void updateRating() => Scheduler.ScheduleIfNeeded(() =>
    {
        ratingChip.Rating = map.Rating;
        RequestedResort?.Invoke();
    });

    private void updateTopScore(Guid id, [CanBeNull] RealmScore score)
    {
        if (id != map.ID)
            return;

        rank.Alpha = score is null ? 0 : 1;
        rank.Rank = score?.Rank ?? ScoreRank.D;
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (Equals(maps.CurrentMap, map))
            item.SelectAction?.Invoke();
        else
            maps.Select(map, true);

        return true;
    }

    private void updateSelected(ValueChangedEvent<RealmMap> e)
    {
        if (Equals(e.NewValue, map))
            outline.FadeIn(200);
        else
            outline.FadeOut(200);
    }

    private GimmickIcon[] getIcons()
    {
        var icons = new List<GimmickIcon>();

        var effects = map.Filters.Effects;

        if (effects.HasFlag(MapEffectType.LaneSwitch))
            icons.Add(new GimmickIcon(FluXisIconType.LaneSwitch, "Contains lane switches"));

        if (effects.HasFlag(MapEffectType.Flash))
            icons.Add(new GimmickIcon(FluXisIconType.Flash, "Contains flashes"));

        if (effects.HasFlag(MapEffectType.Pulse))
            icons.Add(new GimmickIcon(FluXisIconType.Pulse, "Contains pulses"));

        if (effects.HasFlag(MapEffectType.PlayfieldMove))
            icons.Add(new GimmickIcon(FluXisIconType.PlayfieldMove, "Contains playfield moves"));

        if (effects.HasFlag(MapEffectType.PlayfieldScale))
            icons.Add(new GimmickIcon(FluXisIconType.PlayfieldScale, "Contains playfield scales"));

        if (effects.HasFlag(MapEffectType.PlayfieldRotate))
            icons.Add(new GimmickIcon(FluXisIconType.PlayfieldRotate, "Contains playfield rotates"));

        if (effects.HasFlag(MapEffectType.Shake))
            icons.Add(new GimmickIcon(FluXisIconType.Shake, "Contains shakes"));

        if (effects.HasFlag(MapEffectType.Shader))
            icons.Add(new GimmickIcon(FluXisIconType.Shader, "Contains shaders"));

        if (effects.HasFlag(MapEffectType.BeatPulse))
            icons.Add(new GimmickIcon(FluXisIconType.BeatPulse, "Contains beat pulses"));

        if (effects.HasFlag(MapEffectType.HitObjectEase))
            icons.Add(new GimmickIcon(FluXisIconType.HitObjectEase, "Contains hitobject eases"));

        if (effects.HasFlag(MapEffectType.LayerFade))
            icons.Add(new GimmickIcon(FluXisIconType.LayerFade, "Contains layer fades"));

        if (effects.HasFlag(MapEffectType.ScrollMultiply))
            icons.Add(new GimmickIcon(FluXisIconType.ScrollMultiply, "Contains scroll multipliers"));

        if (effects.HasFlag(MapEffectType.TimeOffset))
            icons.Add(new GimmickIcon(FluXisIconType.TimeOffset, "Contains time offsets"));

        return icons.ToArray();
    }

    private partial class GimmickIcon : FluXisIcon, IHasTooltip
    {
        public LocalisableString TooltipText { get; }

        public GimmickIcon(FluXisIconType type, string tooltip)
        {
            Size = new Vector2(14);
            Type = type;
            TooltipText = tooltip;
        }

        protected override bool OnHover(HoverEvent e) => true;
    }

    public int CompareTo(DrawableMapSetDifficulty other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;

        var result = map.Rating.CompareTo(other.map.Rating);
        return result == 0 ? (map.Filters?.NotesPerSecond ?? 0).CompareTo(other.map.Filters?.NotesPerSecond ?? 0) : result;
    }
}
