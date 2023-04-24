using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Background;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Select.List;

public partial class MapListEntry : Container
{
    public readonly SelectScreen Screen;
    public readonly RealmMapSet MapSet;

    public bool Selected => Equals(Screen.MapSet.Value, MapSet);

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
                Y = 75,
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

        foreach (var map in MapSet.Maps)
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
        header.SetDim(.2f);
        difficultyContainer.FadeIn()
                           .ResizeHeightTo(difficultyFlow.Height + 10, 300, Easing.OutQuint);
    }

    private void deselect()
    {
        header.SetDim(.4f);
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

    private partial class MapListEntryHeader : Container
    {
        private readonly MapListEntry parent;
        private readonly RealmMapSet mapset;
        private Box dim;
        private DelayedLoadWrapper backgroundWrapper;

        public MapListEntryHeader(MapListEntry parent, RealmMapSet mapset)
        {
            this.parent = parent;
            this.mapset = mapset;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            Height = 75;
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
                        new SpriteText
                        {
                            Font = FluXisFont.Default(32),
                            Text = mapset.Metadata.Title,
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            Width = 800,
                            Truncate = true
                        },
                        new SpriteText
                        {
                            Font = FluXisFont.Default(28),
                            Text = mapset.Metadata.Artist,
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            Y = 24,
                            Width = 800,
                            Truncate = true
                        },
                        new StatusTag(mapset)
                    }
                }
            };

            backgroundWrapper.DelayedLoadComplete += drawable => drawable.FadeInFromZero(200);
        }

        protected override bool OnHover(HoverEvent e)
        {
            dim.FadeTo(parent.Selected ? .1f : .3f, 100);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            dim.FadeTo(parent.Selected ? .2f : .4f, 300);
            base.OnHoverLost(e);
        }

        public void SetDim(float alpha)
        {
            dim.FadeTo(alpha, 100);
        }
    }
}
