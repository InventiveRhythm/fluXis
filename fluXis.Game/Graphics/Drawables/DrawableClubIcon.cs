using fluXis.Game.Online.API.Models.Clubs;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Graphics.Drawables;

public partial class DrawableClubIcon : Sprite
{
    [Resolved]
    private OnlineTextureStore store { get; set; }

    private APIClub club { get; }

    public DrawableClubIcon(APIClub club)
    {
        this.club = club;
        FillMode = FillMode.Fill;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Texture = store.GetClubIcon(club.IconHash);
    }
}
