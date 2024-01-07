using fluXis.Game.Online.API.Models.Clubs;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Graphics.Drawables;

public partial class DrawableClubBanner : Sprite
{
    [Resolved]
    private OnlineTextureStore store { get; set; }

    private APIClubShort club { get; }

    public DrawableClubBanner(APIClubShort club)
    {
        this.club = club;
        FillMode = FillMode.Fill;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Texture = store.GetClubBanner(club.ID);
    }
}
