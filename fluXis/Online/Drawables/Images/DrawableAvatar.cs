using System;
using fluXis.Graphics;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Drawables.Users;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;

namespace fluXis.Online.Drawables.Images;

#nullable enable

[LongRunningLoad]
public partial class DrawableAvatar : Sprite, IHasCustomTooltip<APIUser>
{
    [Resolved]
    private OnlineTextureStore store { get; set; } = null!;

    [Resolved]
    private TextureStore textures { get; set; } = null!;

    [Resolved]
    private UserCache? users { get; set; }

    public Action? ClickAction { get; init; }

    private APIUser? user;

    public DrawableAvatar(APIUser? user)
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
            Texture = store.GetAvatar(user.AvatarHash) ?? textures.Get("Online/default-avatar");
        else
            Texture = textures.Get("Online/default-avatar");

        Schedule(() => this.FadeInFromZero(400));
    }

    private void registerCallback()
    {
        if (user != null)
            users?.RegisterAvatarCallback(user.ID, reload);
    }

    private void unregisterCallback()
    {
        if (user != null)
            users?.UnregisterAvatarCallback(user.ID, reload);
    }

    private void reload(string hash)
    {
        user!.AvatarHash = hash;
        Scheduler.ScheduleOnceIfNeeded(setTexture);
    }

    public void UpdateUser(APIUser? newUser)
    {
        unregisterCallback();

        user = newUser;
        setTexture();

        registerCallback();
    }

    protected override bool OnClick(ClickEvent e)
    {
        ClickAction?.Invoke();
        return ClickAction != null;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        unregisterCallback();
    }

    #region Tooltip

    public bool ShowTooltip { get; init; }
    APIUser? IHasCustomTooltip<APIUser>.TooltipContent => ShowTooltip ? user : null;
    public ITooltip<APIUser> GetCustomTooltip() => new UserTooltip();

    #endregion
}
