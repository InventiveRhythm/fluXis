using fluXis.Game.Online.API.Users;
using fluXis.Game.Online.Drawables;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Mouse;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;

namespace fluXis.Game.Graphics.Drawables;

public partial class DrawableAvatar : Sprite, IHasDrawableTooltip
{
    [Resolved]
    private Fluxel fluxel { get; set; }

    public bool ShowTooltip { get; set; } = false;

    private APIUserShort user;
    private TextureStore textures;

    public DrawableAvatar(APIUserShort user)
    {
        this.user = user ?? APIUserShort.Dummy;
        Alpha = 0;
        FillMode = FillMode.Fill;
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        this.textures = textures;
        Texture = textures.Get(user.GetAvatarUrl(fluxel.Endpoint));
    }

    protected override void LoadComplete()
    {
        this.FadeInFromZero(200);
    }

    public void UpdateUser(APIUserShort newUser)
    {
        user = newUser ?? APIUserShort.Dummy;
        Texture = textures.Get(user.GetAvatarUrl(fluxel.Endpoint));
    }

    protected override bool OnHover(HoverEvent e) => user.ID >= 0 && ShowTooltip;
    public Drawable GetTooltip() => new UserTooltip { UserID = user.ID };
}
