using System.Collections.Generic;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Menu;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osuTK;
using Box = osu.Framework.Graphics.Shapes.Box;

namespace fluXis.Game.Screens.Select.List;

public partial class MapDifficultyEntry : Container, IHasContextMenu
{
    public MenuItem[] ContextMenuItems
    {
        get
        {
            List<MenuItem> items = new()
            {
                new FluXisMenuItem("Play", FontAwesome.Solid.Play, MenuItemType.Highlighted, () =>
                {
                    var screen = mapListEntry.Screen;

                    screen.MapSet.Value = map.MapSet; // failsafe
                    screen.MapInfo.Value = map;
                    screen.Accept();
                }),
                new FluXisMenuItem("Edit", FontAwesome.Solid.Pen, MenuItemType.Normal, () => mapListEntry.Screen.EditMapSet(map))
            };

            if (FluXisGameBase.IsDebug)
                items.Add(new FluXisMenuItem("Copy ID", FontAwesome.Solid.Copy, MenuItemType.Normal, () => clipboard?.SetText(map.ID.ToString())));

            return items.ToArray();
        }
    }

    [Resolved]
    private Storage storage { get; set; }

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
                EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Shadow,
                    Colour = Colour4.Black.Opacity(0.25f),
                    Radius = 0,
                    Offset = new Vector2(0, 2)
                },
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
                                        new CircularContainer
                                        {
                                            Width = 50,
                                            Height = 14,
                                            Masking = true,
                                            Children = new Drawable[]
                                            {
                                                new Box
                                                {
                                                    RelativeSizeAxes = Axes.Both,
                                                    Colour = FluXisColors.GetDifficultyColor(map.Filters.NotesPerSecond)
                                                },
                                                new FluXisSpriteText
                                                {
                                                    Text = map.Filters.NotesPerSecond.ToStringInvariant("00.00"),
                                                    FontSize = 14,
                                                    Colour = Colour4.Black,
                                                    Alpha = .75f,
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre
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

        mapListEntry.Screen.MapInfo.BindValueChanged(e => updateSelected(e.NewValue));
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (Equals(mapListEntry.Screen.MapInfo.Value, map))
            mapListEntry.Screen.Accept();
        else
            mapListEntry.Screen.MapInfo.Value = map;

        return true;
    }

    private void updateSelected(RealmMap newMap)
    {
        if (Equals(newMap, map))
            outline.FadeIn(200);
        else
            outline.FadeOut(200);
    }
}
