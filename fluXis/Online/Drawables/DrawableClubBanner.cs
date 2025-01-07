using fluXis.Graphics;
using fluXis.Online.API.Models.Clubs;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Online.Drawables;

[LongRunningLoad]
public partial class DrawableClubBanner : Sprite
{
    [Resolved]
    private OnlineTextureStore store { get; set; }

    private APIClub club { get; }

    public DrawableClubBanner(APIClub club)
    {
        this.club = club;
        FillMode = FillMode.Fill;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Texture = store.GetClubBanner(club.BannerHash);
    }
}
