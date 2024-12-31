using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Setup;

public partial class SetupHeader : CompositeDrawable
{
    [Resolved]
    private EditorMap map { get; set; }

    private MapBackground background;
    private MapCover cover;
    private FluXisSpriteText title;
    private FluXisSpriteText artist;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 260;
        CornerRadius = 40;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background1
            },
            background = new MapBackground(map.RealmMap)
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2,
                Alpha = .5f
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Horizontal,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Padding = new MarginPadding(40),
                Spacing = new Vector2(20),
                Children = new Drawable[]
                {
                    new Container
                    {
                        Size = new Vector2(180),
                        CornerRadius = 20,
                        Masking = true,
                        Child = cover = new MapCover(map.MapSet)
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        }
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Spacing = new Vector2(-5),
                        Children = new Drawable[]
                        {
                            title = new FluXisSpriteText
                            {
                                Text = map.RealmMap.Metadata.Title,
                                WebFontSize = 32,
                                Shadow = true
                            },
                            artist = new FluXisSpriteText
                            {
                                Text = map.RealmMap.Metadata.Artist,
                                WebFontSize = 24,
                                Shadow = true
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

        map.BackgroundChanged += onBackgroundChanged;
        map.CoverChanged += onCoverChanged;
    }

    protected override void Update()
    {
        base.Update();

        title.Text = map.RealmMap.Metadata.Title;
        artist.Text = map.RealmMap.Metadata.Artist;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        map.BackgroundChanged -= onBackgroundChanged;
        map.CoverChanged -= onCoverChanged;
    }

    private void onBackgroundChanged() => background.Map = map.RealmMap;
    private void onCoverChanged() => cover.MapSet = map.MapSet;
}
