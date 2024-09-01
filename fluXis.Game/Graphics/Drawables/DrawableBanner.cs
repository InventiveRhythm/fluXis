using fluXis.Game.Online;
using fluXis.Shared.Components.Users;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Graphics.Drawables;

#nullable enable

public partial class DrawableBanner : Sprite
{
    [Resolved]
    private OnlineTextureStore store { get; set; } = null!;

    [Resolved]
    private TextureStore textures { get; set; } = null!;

    [Resolved]
    private UserCache? users { get; set; }

    private APIUser? user;

    public DrawableBanner(APIUser user)
    {
        this.user = user;
        FillMode = FillMode.Fill;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        setTexture();
    }

    private void setTexture()
    {
        if (user is { ID: >= 0 }) // the texture from the online store could still be null
            Texture = store.GetBanner(user.ID) ?? textures.Get("Online/default-banner");
        else
            Texture = textures.Get("Online/default-banner");

        Schedule(() => this.FadeInFromZero(400));
    }

    private void registerCallback()
    {
        if (user != null)
            users?.RegisterBannerCallback(user.ID, reload);
    }

    private void unregisterCallback()
    {
        if (user != null)
            users?.UnregisterBannerCallback(user.ID, reload);
    }

    private void reload()
    {
        // clear from texture store
        Texture = null;

        // wait 2 frames to allow texture store to clear
        Schedule(() => Schedule(setTexture));
    }

    public void UpdateUser(APIUser? newUser)
    {
        unregisterCallback();

        user = newUser;
        setTexture();

        registerCallback();
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        unregisterCallback();
    }
}
