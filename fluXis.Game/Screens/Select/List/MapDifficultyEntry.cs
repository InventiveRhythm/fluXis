using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Menus;
using fluXis.Game.Localization;
using fluXis.Game.Map;
using fluXis.Game.Map.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.EnumExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Framework.Platform;
using osuTK;

namespace fluXis.Game.Screens.Select.List;

public partial class MapDifficultyEntry : Container, IHasContextMenu
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
                    mapListEntry.SelectAction?.Invoke();
                }),
                new FluXisMenuItem(LocalizationStrings.General.Edit, FontAwesome6.Solid.Pen, MenuItemType.Normal, () => mapListEntry.EditAction?.Invoke(map))
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

    private readonly MapListEntry mapListEntry;
    private readonly RealmMap map;
    private Container outline;

    public MapDifficultyEntry(MapListEntry parentEntry, RealmMap map)
    {
        mapListEntry = parentEntry;
        this.map = map;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 48;

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
                            Colour = ColourInfo.GradientVertical(FluXisColors.GetKeyColor(map.KeyCount).Lighten(1), FluXisColors.GetKeyColor(map.KeyCount))
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
                        Colour = FluXisColors.GetKeyColor(map.KeyCount)
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
                                    AutoSizeAxes = Axes.Both,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Direction = FillDirection.Vertical,
                                    Padding = new MarginPadding { Left = 10 },
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
                                                new DifficultyChip
                                                {
                                                    Width = 50,
                                                    Height = 14,
                                                    FontSize = 14,
                                                    Rating = map.Filters.NotesPerSecond,
                                                    Margin = new MarginPadding { Right = 5 }
                                                }
                                            }.Concat(getIcons()).ToArray()
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

    protected override void Dispose(bool isDisposing)
    {
        maps.MapBindable.ValueChanged -= updateSelected;

        base.Dispose(isDisposing);
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (Equals(maps.CurrentMap, map))
            mapListEntry.SelectAction?.Invoke();
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

        if (effects.HasFlagFast(EffectType.LaneSwitch))
            icons.Add(new GimmickIcon(FluXisIconType.LaneSwitch, "Contains lane switches"));

        if (effects.HasFlagFast(EffectType.Flash))
            icons.Add(new GimmickIcon(FluXisIconType.Flash, "Contains flashes"));

        if (effects.HasFlagFast(EffectType.Pulse))
            icons.Add(new GimmickIcon(FluXisIconType.Pulse, "Contains pulses"));

        if (effects.HasFlagFast(EffectType.PlayfieldMove))
            icons.Add(new GimmickIcon(FluXisIconType.PlayfieldMove, "Contains playfield moves"));

        if (effects.HasFlagFast(EffectType.PlayfieldScale))
            icons.Add(new GimmickIcon(FluXisIconType.PlayfieldScale, "Contains playfield scales"));

        if (effects.HasFlagFast(EffectType.PlayfieldFade))
            icons.Add(new GimmickIcon(FluXisIconType.PlayfieldFade, "Contains playfield fades"));

        if (effects.HasFlagFast(EffectType.Shake))
            icons.Add(new GimmickIcon(FluXisIconType.Shake, "Contains shakes"));

        if (effects.HasFlagFast(EffectType.Shader))
            icons.Add(new GimmickIcon(FluXisIconType.Shader, "Contains shaders"));

        if (effects.HasFlagFast(EffectType.BeatPulse))
            icons.Add(new GimmickIcon(FluXisIconType.BeatPulse, "Contains beat pulses"));

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
}
