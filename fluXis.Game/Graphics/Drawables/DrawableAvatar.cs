using fluXis.Game.Online;
using fluXis.Game.Online.Drawables;
using fluXis.Shared.Components.Users;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;

namespace fluXis.Game.Graphics.Drawables;

public partial class DrawableAvatar : Sprite
{
    [Resolved]
    private OnlineTextureStore store { get; set; }

    [Resolved]
    private UserCache users { get; set; }

    public bool ShowTooltip { get; set; }

    private APIUser user;

    public DrawableAvatar(APIUser user)
    {
        this.user = user ?? APIUser.Dummy;
        Alpha = 0;
        FillMode = FillMode.Fill;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Texture = store.GetAvatar(user.ID);
    }

    protected override void LoadComplete()
    {
        this.FadeInFromZero(400);

        users.RegisterAvatarCallback(user.ID, reload);
    }

    private void reload()
    {
        // clear from texture store
        Texture = null;

        // wait 2 frames to allow texture store to clear
        Schedule(() => Schedule(() =>
        {
            Texture = store.GetAvatar(user.ID);
            this.FadeInFromZero(400);
        }));
    }

    public void UpdateUser(APIUser newUser)
    {
        users.UnregisterAvatarCallback(user.ID, reload);

        user = newUser ?? APIUser.Dummy;
        Texture = store.GetAvatar(user.ID);
        Schedule(() => this.FadeInFromZero(400));

        users.RegisterAvatarCallback(user.ID, reload);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        users?.UnregisterAvatarCallback(user.ID, reload);
    }

    protected override bool OnHover(HoverEvent e) => user.ID >= 0 && ShowTooltip;
    public Drawable GetTooltip() => OnHover(null) ? new UserTooltip { UserID = user.ID } : null;
}
