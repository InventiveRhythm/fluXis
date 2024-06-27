using fluXis.Game.Online;
using fluXis.Shared.Components.Users;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Graphics.Drawables;

public partial class DrawableBanner : Sprite
{
    [Resolved]
    private OnlineTextureStore store { get; set; }

    [Resolved]
    private UserCache users { get; set; }

    private APIUser user;

    public DrawableBanner(APIUser user)
    {
        this.user = user ?? APIUser.Dummy;
        Alpha = 0;
        FillMode = FillMode.Fill;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Texture = store.GetBanner(user.ID);
    }

    protected override void LoadComplete()
    {
        this.FadeInFromZero(400);

        users.RegisterBannerCallback(user.ID, reload);
    }

    private void reload()
    {
        // clear from texture store
        Texture = null;

        // wait 2 frames to allow texture store to clear
        Schedule(() => Schedule(() =>
        {
            Texture = store.GetBanner(user.ID);
            this.FadeInFromZero(400);
        }));
    }

    public void UpdateUser(APIUser newUser)
    {
        if (user.ID == newUser?.ID) return;

        users.UnregisterBannerCallback(user.ID, reload);

        user = newUser ?? APIUser.Dummy;
        Texture = store.GetBanner(user.ID);
        Schedule(() => this.FadeInFromZero(400));

        users.RegisterBannerCallback(user.ID, reload);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        users.UnregisterBannerCallback(user.ID, reload);
    }
}
