using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Cover;
using fluXis.Game.Graphics.Menu;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Select.List;

public partial class MapListEntry : Container
{
    public readonly SelectScreen Screen;
    public readonly RealmMapSet MapSet;

    public bool Selected => Equals(Screen.MapSet.Value, MapSet);

    public List<RealmMap> Maps
    {
        get
        {
            var diffs = MapSet.Maps.ToList();

            // sorting by nps until we have a proper difficulty system
            diffs.Sort((a, b) => a.Filters.NotesPerSecond.CompareTo(b.Filters.NotesPerSecond));
            return diffs;
        }
    }

    private MapListEntryHeader header;
    private Container difficultyContainer;
    private FillFlowContainer<MapDifficultyEntry> difficultyFlow;

    public MapListEntry(SelectScreen screen, RealmMapSet mapSet)
    {
        Screen = screen;
        MapSet = mapSet;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        InternalChildren = new Drawable[]
        {
            header = new MapListEntryHeader(this, MapSet),
            difficultyContainer = new Container
            {
                Masking = true,
                RelativeSizeAxes = Axes.X,
                Y = 80,
                Height = 10,
                Padding = new MarginPadding(5),
                Child = difficultyFlow = new FillFlowContainer<MapDifficultyEntry>
                {
                    Direction = FillDirection.Vertical,
                    AutoSizeAxes = Axes.Y,
                    RelativeSizeAxes = Axes.X,
                    Spacing = new Vector2(0, 5)
                }
            }
        };

        foreach (var map in Maps)
        {
            difficultyFlow.Add(new MapDifficultyEntry(this, map));
        }
    }

    private void updateSelected()
    {
        if (Selected)
            select();
        else
            deselect();
    }

    private void select()
    {
        difficultyContainer.FadeIn()
                           .ResizeHeightTo(difficultyFlow.Height + 10, 300, Easing.OutQuint);
    }

    private void deselect()
    {
        difficultyContainer.ResizeHeightTo(0, 300, Easing.OutQuint)
                           .Then().FadeOut();
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (Selected)
            return false; // dont handle clicks when we already selected this mapset

        if (Screen != null)
        {
            Screen.MapSet.Value = MapSet;
            return true;
        }

        return false;
    }

    protected override void LoadComplete()
    {
        // hacky thing because it doesnt get the height properly
        if (Selected)
            difficultyContainer.ResizeHeightTo(difficultyFlow.Children.Count * 45 + 5);
        else
            difficultyContainer.ResizeHeightTo(0);

        Screen?.MapSet.BindValueChanged(_ => updateSelected());

        base.LoadComplete();
    }

    private partial class MapListEntryHeader : Container, IHasContextMenu
    {
        public MenuItem[] ContextMenuItems
        {
            get
            {
                List<MenuItem> items = new();

                if (!Equals(parent.Screen.MapSet.Value, mapset))
                    items.Add(new FluXisMenuItem("Select", MenuItemType.Highlighted, () => parent.Screen.MapSet.Value = mapset));

                items.Add(new FluXisMenuItem("Export", MenuItemType.Normal, () => parent.Screen.ExportMapSet(mapset)));
                items.Add(new FluXisMenuItem("Delete", MenuItemType.Dangerous, () => parent.Screen.DeleteMapSet(mapset)));

                return items.ToArray();
            }
        }

        private readonly MapListEntry parent;
        private readonly RealmMapSet mapset;
        private Box dim;
        private DelayedLoadWrapper backgroundWrapper;
        private DelayedLoadWrapper coverWrapper;

        public MapListEntryHeader(MapListEntry parent, RealmMapSet mapset)
        {
            this.parent = parent;
            this.mapset = mapset;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            Height = 80;
            CornerRadius = 10;
            Margin = new MarginPadding { Bottom = 5 };
            Masking = true;
            Children = new Drawable[]
            {
                backgroundWrapper = new DelayedLoadWrapper(() => new MapBackground(mapset.Maps[0])
                {
                    RelativeSizeAxes = Axes.Both,
                    FillMode = FillMode.Fill,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }, 100),
                dim = new Box
                {
                    Name = "Background Dim",
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.Black,
                    Alpha = .4f
                },
                new Container
                {
                    Padding = new MarginPadding(10),
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        coverWrapper = new DelayedLoadWrapper(() => new Container
                        {
                            Size = new Vector2(60),
                            Masking = true,
                            CornerRadius = 10,
                            Child = new DrawableCover(mapset)
                            {
                                RelativeSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre
                            },
                            EdgeEffect = new EdgeEffectParameters
                            {
                                Type = EdgeEffectType.Shadow,
                                Colour = Colour4.Black.Opacity(.2f),
                                Radius = 5,
                                Offset = new Vector2(0, 1)
                            }
                        }, 100),
                        new SpriteText
                        {
                            Font = FluXisFont.Default(32),
                            Text = mapset.Metadata.Title,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.BottomLeft,
                            Y = 5,
                            Width = 740,
                            Truncate = true,
                            Margin = new MarginPadding { Left = 70 },
                            Shadow = true
                        },
                        new SpriteText
                        {
                            Font = FluXisFont.Default(24),
                            Text = mapset.Metadata.Artist,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.TopLeft,
                            Width = 740,
                            Truncate = true,
                            Margin = new MarginPadding { Left = 70 },
                            Shadow = true
                        },
                        new StatusTag(mapset)
                    }
                }
            };

            backgroundWrapper.DelayedLoadComplete += drawable => drawable.FadeInFromZero(200);
            coverWrapper.DelayedLoadComplete += drawable => drawable.FadeInFromZero(200);
        }

        protected override bool OnHover(HoverEvent e)
        {
            dim.FadeTo(.2f, 50);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            dim.FadeTo(.4f, 200);
        }
    }
}
