using fluXis.Game.Online;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Online.Drawables;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Mouse;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;

namespace fluXis.Game.Graphics.Drawables;

public partial class DrawableAvatar : Sprite, IHasDrawableTooltip
{
    [Resolved]
    private Fluxel fluxel { get; set; }

    [Resolved]
    private OnlineTextureStore store { get; set; }

    public bool ShowTooltip { get; set; }

    private APIUserShort user;

    public DrawableAvatar(APIUserShort user)
    {
        this.user = user ?? APIUserShort.Dummy;
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

        UserCache.GetAvatarUpdateCallbacks(user.ID).Add(reload);
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

    public void UpdateUser(APIUserShort newUser)
    {
        UserCache.GetAvatarUpdateCallbacks(user.ID).Remove(reload);

        user = newUser ?? APIUserShort.Dummy;
        Texture = store.GetAvatar(user.ID);
        Schedule(() => this.FadeInFromZero(400));

        UserCache.GetAvatarUpdateCallbacks(user.ID).Add(reload);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        UserCache.GetAvatarUpdateCallbacks(user.ID).Remove(reload);
    }

    protected override bool OnHover(HoverEvent e) => user.ID >= 0 && ShowTooltip;
    public Drawable GetTooltip() => OnHover(null) ? new UserTooltip { UserID = user.ID } : null;
}
