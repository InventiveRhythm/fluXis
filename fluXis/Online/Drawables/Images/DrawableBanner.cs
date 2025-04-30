using fluXis.Graphics;
using fluXis.Online.API.Models.Users;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Online.Drawables.Images;

#nullable enable

[LongRunningLoad]
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
        registerCallback();
    }

    private void setTexture()
    {
        if (user is { ID: >= 0 }) // the texture from the online store could still be null
            Texture = store.GetBanner(user.BannerHash) ?? textures.Get("Online/default-banner");
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

    private void reload(string hash)
    {
        user!.BannerHash = hash;
        Scheduler.ScheduleOnceIfNeeded(setTexture);
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
