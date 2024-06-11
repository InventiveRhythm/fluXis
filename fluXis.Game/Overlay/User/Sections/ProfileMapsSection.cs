using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.User.Sections.Maps;
using fluXis.Shared.Components.Users;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Overlay.User.Sections;

public partial class ProfileMapsSection : FillFlowContainer
{
    private APIUserMaps maps { get; }

    public ProfileMapsSection(APIUserMaps maps)
    {
        this.maps = maps;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(20);

        InternalChildren = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding { Horizontal = 10 },
                Child = new FluXisSpriteText
                {
                    Text = "Maps",
                    WebFontSize = 24
                }
            },
            new ProfileMapsSubsection("Pure", maps.Pure),
            new ProfileMapsSubsection("Impure/Unsubmitted", maps.Impure),
            new ProfileMapsSubsection("Guest Difficulties", maps.Guest)
        };
    }
}
