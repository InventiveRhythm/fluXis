using System;
using fluXis.Graphics;
using fluXis.Graphics.Sprites.Animated;
using fluXis.Online.API.Models.Users;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Animations;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Logging;
using osuTK;

namespace fluXis.Online.Drawables.Images;

#nullable enable

[LongRunningLoad]
public partial class DrawableBanner : CompositeDrawable
{
    [Resolved]
    private OnlineTextureStore store { get; set; } = null!;

    [Resolved]
    private TextureStore textures { get; set; } = null!;

    [Resolved]
    private UserCache? users { get; set; }

    [Resolved]
    private AnimatedSpriteStore animations { get; set; } = null!;

    private APIUser? user;

    public DrawableBanner(APIUser user)
    {
        this.user = user;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        setTexture();
        registerCallback();
    }

    private void setTexture()
    {
        if (!setAnimated())
        {
            var sprite = new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                FillMode = FillMode.Fill
            };

            if (user is { ID: >= 0 }) // the texture from the online store could still be null
                sprite.Texture = store.GetBanner(user.BannerHash) ?? textures.Get("Online/default-banner");
            else
                sprite.Texture = textures.Get("Online/default-banner");

            InternalChild = sprite;
        }

        Schedule(() => this.FadeInFromZero(400));
    }

    private bool setAnimated()
    {
        if (user is null || !user.HasAnimatedBanner)
            return false;

        try
        {
            var frames = animations.Get(user.BannerHash, OnlineTextureStore.AssetType.Banner);
            if (frames is null || frames.Length <= 0) return false;

            var spr = new DrawableAnimation { RelativeSizeAxes = Axes.Both };

            foreach (var frame in frames)
            {
                spr.AddFrame(new Sprite
                {
                    Texture = frame.Texture,
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    FillMode = FillMode.Fill,
                    Size = new Vector2(1)
                }, frame.Duration);
            }

            InternalChild = spr;
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to load animated banner!");
            return false;
        }
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
