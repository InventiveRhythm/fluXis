using fluXis.Database.Maps;
using fluXis.Graphics;
using fluXis.Graphics.Sprites.Text;
using fluXis.Map;
using fluXis.Map.Drawables;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Screens.Menu.UI.NowPlaying;

public partial class MenuNowPlaying : Container
{
    [Resolved]
    private MapStore mapStore { get; set; }

    private Container coverContainer;
    private MapCover cover;
    private FluXisSpriteText title;
    private FluXisSpriteText artist;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Anchor = Anchor.TopRight;
        Origin = Anchor.CentreRight;
        Y = 60;
        Alpha = 0;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                Width = 600,
                Height = 80,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.Centre,
                Colour = ColourInfo.GradientHorizontal(Color4.Black.Opacity(0), Color4.Black),
            },
            coverContainer = new Container
            {
                Size = new Vector2(120),
                X = -75,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.Centre,
                CornerRadius = 10,
                Masking = true,
                EdgeEffect = Styling.ShadowSmall,
                Child = cover = new MapCover(null)
                {
                    RelativeSizeAxes = Axes.Both,
                    FillMode = FillMode.Fill,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Direction = FillDirection.Vertical,
                Margin = new MarginPadding { Right = 160 },
                Children = new Drawable[]
                {
                    title = new FluXisSpriteText
                    {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        Margin = new MarginPadding { Right = -10 },
                        Shadow = true,
                        FontSize = 36
                    },
                    artist = new FluXisSpriteText
                    {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        Shadow = true
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        mapStore.MapSetBindable.BindValueChanged(onSongChanged, true);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        mapStore.MapSetBindable.ValueChanged -= onSongChanged;
    }

    private void onSongChanged(ValueChangedEvent<RealmMapSet> e)
    {
        cover.MapSet = mapStore.CurrentMapSet;
        title.Text = mapStore.CurrentMapSet?.Metadata.LocalizedTitle;
        artist.Text = mapStore.CurrentMapSet?.Metadata.LocalizedArtist;

        const int delay = 4000;

        Scheduler.ScheduleOnceIfNeeded(() =>
        {
            this.MoveToX(50).MoveToX(0, 600, Easing.OutQuint).FadeInFromZero(400)
                .Delay(delay).MoveToX(50, 800, Easing.OutQuint).FadeOut(400);

            coverContainer.RotateTo(-4).RotateTo(4, 600, Easing.OutQuint);
        });
    }
}
