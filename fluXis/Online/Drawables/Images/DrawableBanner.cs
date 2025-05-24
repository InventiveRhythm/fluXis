using System;
using System.IO;
using fluXis.Graphics;
using fluXis.Graphics.Sprites;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Fluxel;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Animations;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Network;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osuTK;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;

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
    private IAPIClient api { get; set; } = null!;

    [Resolved]
    private GameHost host { get; set; } = null!;

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
            var path = host.CacheStorage.GetFullPath($"animations/{user.BannerHash}.gif", true);

            if (!host.CacheStorage.Exists(path))
            {
                var req = new WebRequest($"{api.Endpoint.AssetUrl}/banner/{user.BannerHash}_a.gif");
                req.AllowInsecureRequests = true;
                req.Perform();

                var data = req.GetResponseData();
                if (data is null) return false;

                File.WriteAllBytes(path, data);
            }

            using var stream = File.OpenRead(path);
            using var image = Image.Load<Rgba32>(stream);

            var spr = new DrawableAnimation { RelativeSizeAxes = Axes.Both };

            for (var i = 0; i < image.Frames.Count; i++)
            {
                var frame = image.Frames[i];
                var meta = frame.Metadata.GetGifMetadata();

                var clone = image.Frames.CloneFrame(i);
                var upload = new TextureUpload(clone);
                spr.AddFrame(new CustomTextureSprite(upload)
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    FillMode = FillMode.Fill,
                    Size = new Vector2(1)
                }, meta.FrameDelay * 10);
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
