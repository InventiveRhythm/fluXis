using fluXis.Graphics;
using fluXis.Online.API.Models.Clubs;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Online.Drawables.Images;

[LongRunningLoad]
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
    private void load(TextureStore textures)
    {
        Texture = store.GetClubIcon(club.IconHash) ?? textures.Get("Online/default-club");
    }
}
