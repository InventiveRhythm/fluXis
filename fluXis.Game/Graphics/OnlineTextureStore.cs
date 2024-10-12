using System;
using System.Text;
using fluXis.Game.Online;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;

namespace fluXis.Game.Graphics;

public class OnlineTextureStore : TextureStore
{
    private APIEndpointConfig endpointConfig { get; }

    public OnlineTextureStore(GameHost host, APIEndpointConfig endpointConfig)
        : base(host.Renderer)
    {
        this.endpointConfig = endpointConfig;
        AddTextureSource(host.CreateTextureLoaderStore(new OnlineStore()));
        AddTextureSource(host.CreateTextureLoaderStore(new HttpOnlineStore()));
    }

    public Texture GetAchievement(string id) => get(AssetType.Achievement, id, AssetSize.Small, true);
    public Texture GetAvatar(string hash) => get(AssetType.Avatar, hash);
    public Texture GetBanner(string hash) => get(AssetType.Banner, hash);
    public Texture GetBackground(long id, AssetSize size = AssetSize.Small) => get(AssetType.Background, id, size);
    public Texture GetCover(long id, AssetSize size = AssetSize.Small) => get(AssetType.Cover, id, size);
    public Texture GetClubIcon(string id) => get(AssetType.ClubIcon, id);
    public Texture GetClubBanner(string id) => get(AssetType.ClubBanner, id);

    private Texture get(AssetType type, long id, AssetSize size = AssetSize.Small, bool addExtension = false) => get(type, id.ToString(), size, addExtension);
    private Texture get(AssetType type, string id, AssetSize size = AssetSize.Small, bool addExtension = false) => string.IsNullOrEmpty(id) ? null : Get(getUrl(type, id, size, addExtension));

    private string getUrl(AssetType type, string id, AssetSize size, bool addExtension)
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

        var builder = new StringBuilder();
        builder.Append($"{endpointConfig.AssetUrl}/{typeStr}/{id}");

        if (size == AssetSize.Large)
            builder.Append("-lg");
        if (addExtension)
            builder.Append(".png");

        return builder.ToString();
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

    public enum AssetSize
    {
        Small,
        Large
    }
}
