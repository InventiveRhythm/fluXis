using System.Collections.Generic;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Menu;
using fluXis.Game.Screens.Edit;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Screens;
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
                new FluXisMenuItem("Play", MenuItemType.Highlighted, () =>
                {
                    var screen = mapListEntry.Screen;

                    screen.MapSet.Value = map.MapSet; // failsafe
                    screen.MapInfo.Value = map;
                    screen.Accept();
                }),
                new FluXisMenuItem("Edit", MenuItemType.Normal, () =>
                {
                    var screen = mapListEntry.Screen;

                    screen.MapSet.Value = map.MapSet;
                    screen.MapInfo.Value = map;

                    var path = storage.GetFullPath($"files/{PathUtils.HashToPath(map.Hash)}");
                    var loadedMap = MapUtils.LoadFromPath(path);

                    if (loadedMap == null)
                    {
                        Logger.Log($"Could not load map file for {map.Hash}", LoggingTarget.Runtime, LogLevel.Error);
                        return;
                    }

                    var editor = new Editor(map, loadedMap);
                    screen.Push(editor);
                })
            };

            return items.ToArray();
        }
    }

    [Resolved]
    private Storage storage { get; set; }

    private readonly MapListEntry mapListEntry;
    private readonly RealmMap map;

    private Box wedge;
    private Box wedge2;

    public MapDifficultyEntry(MapListEntry parentEntry, RealmMap map)
    {
        mapListEntry = parentEntry;
        this.map = map;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 40;
        CornerRadius = 5;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.GetKeyColor(map.KeyCount)
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(2),
                Child = new GridContainer
                {
                    ColumnDimensions = new Dimension[]
                    {
                        new(),
                        new(GridSizeMode.Absolute, 40)
                    },
                    RelativeSizeAxes = Axes.Both,
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Children = new Drawable[]
                                {
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        CornerRadius = 5,
                                        Masking = true,
                                        Child = new Box
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Colour = FluXisColors.Background
                                        },
                                    },
                                    wedge2 = new Box
                                    {
                                        RelativeSizeAxes = Axes.Y,
                                        Width = 30,
                                        Colour = FluXisColors.GetKeyColor(map.KeyCount).Darken(.2f),
                                        Anchor = Anchor.CentreRight,
                                        Origin = Anchor.CentreLeft,
                                        Shear = new Vector2(-.1f, 0)
                                    },
                                    wedge = new Box
                                    {
                                        RelativeSizeAxes = Axes.Y,
                                        Width = 45,
                                        Colour = FluXisColors.GetKeyColor(map.KeyCount),
                                        Anchor = Anchor.CentreRight,
                                        Origin = Anchor.CentreLeft,
                                        Shear = new Vector2(.15f, 0)
                                    },
                                    new FillFlowContainer
                                    {
                                        Direction = FillDirection.Horizontal,
                                        RelativeSizeAxes = Axes.Both,
                                        Spacing = new Vector2(3),
                                        Padding = new MarginPadding { Left = 10 },
                                        Children = new Drawable[]
                                        {
                                            new SpriteText
                                            {
                                                Text = map.Difficulty,
                                                Anchor = Anchor.CentreLeft,
                                                Origin = Anchor.CentreLeft,
                                                Font = FluXisFont.Default(22)
                                            },
                                            new SpriteText
                                            {
                                                Text = $"mapped by {map.Metadata.Mapper}",
                                                Anchor = Anchor.CentreLeft,
                                                Origin = Anchor.CentreLeft,
                                                Font = FluXisFont.Default(22),
                                                Colour = FluXisColors.Text2
                                            }
                                        }
                                    }
                                }
                            },
                            new SpriteText
                            {
                                Text = $"{map.KeyCount}K",
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Font = FluXisFont.Default(),
                                Colour = FluXisColors.GetKeyColor(map.KeyCount).ToHSL().Z > 0.5f ? Colour4.FromHex("#1a1a20") : Colour4.White
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
        {
            wedge.MoveToX(-40, 100, Easing.OutQuint);
            wedge2.MoveToX(-42, 170, Easing.OutQuint);
        }
        else
        {
            wedge.MoveToX(0, 100, Easing.InQuint);
            wedge2.MoveToX(0, 160, Easing.InQuint);
        }
    }
}
