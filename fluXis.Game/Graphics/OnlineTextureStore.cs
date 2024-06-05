using System;
using fluXis.Game.Online;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;

namespace fluXis.Game.Graphics;

public class OnlineTextureStore : TextureStore
{
    private APIEndpointConfig endpointConfig { get; }

    public OnlineTextureStore(GameHost host, APIEndpointConfig endpointConfig, UserCache users)
        : base(host.Renderer)
    {
        this.endpointConfig = endpointConfig;
        AddTextureSource(host.CreateTextureLoaderStore(new OnlineStore()));
        AddTextureSource(host.CreateTextureLoaderStore(new HttpOnlineStore()));

        users.OnAvatarUpdate += id => purge(AssetType.Avatar, id);
        users.OnBannerUpdate += id => purge(AssetType.Banner, id);
    }

    public Texture GetAchievement(string id) => get(AssetType.Achievement, id, true);
    public Texture GetAvatar(long id) => get(AssetType.Avatar, id);
    public Texture GetBanner(long id) => get(AssetType.Banner, id);
    public Texture GetBackground(long id) => get(AssetType.Background, id);
    public Texture GetCover(long id) => get(AssetType.Cover, id);
    public Texture GetClubIcon(long id) => get(AssetType.ClubIcon, id);
    public Texture GetClubBanner(long id) => get(AssetType.ClubBanner, id);

    private Texture get(AssetType type, long id, bool addExtension = false) => get(type, id.ToString(), addExtension);
    private Texture get(AssetType type, string id, bool addExtension = false) => Get(getUrl(type, id, addExtension));

    private string getUrl(AssetType type, string id, bool addExtension)
    {
        var typeStr = type switch
        {
            AssetType.Achievement => "achievement",
            AssetType.Avatar => "avatar",
            AssetType.Banner => "banner",
            AssetType.Background => "background",
            AssetType.Cover => "cover",
            AssetType.ClubIcon => "club-icon",
            AssetType.ClubBanner => "club-banner",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        return $"{endpointConfig.AssetUrl}/{typeStr}/{id}" + (addExtension ? ".png" : "");
    }

    private static string getLookup(string name, WrapMode wrapModeS = default, WrapMode wrapModeT = default)
        => $"{name}:wrap-{(int)wrapModeS}-{(int)wrapModeT}";

    private void purge(AssetType type, long id) => purge(type, id.ToString());

    private void purge(AssetType type, string id, bool addExtension = false)
    {
        var url = getUrl(type, id, addExtension);
        if (TryGetCached(getLookup(url), out var tex))
            Purge(tex);
    }

    public enum AssetType
    {
        Achievement,
        Avatar,
        Banner,
        Background,
        Cover,
        ClubIcon,
        ClubBanner
    }
}
