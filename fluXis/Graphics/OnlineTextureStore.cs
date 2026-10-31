using System;
using System.Text;
using fluXis.Online.Fluxel;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;

namespace fluXis.Graphics;

public class OnlineTextureStore : TextureStore
{
    private IAPIClient api { get; }

    public OnlineTextureStore(GameHost host, IAPIClient api)
        : base(host.Renderer)
    {
        this.api = api;
        AddTextureSource(host.CreateTextureLoaderStore(new OnlineStore()));
        AddTextureSource(host.CreateTextureLoaderStore(new HttpOnlineStore()));
    }

    public Texture GetAchievement(string id) => get(AssetType.Achievement, id);
    public Texture GetAvatar(string hash) => get(AssetType.Avatar, hash);
    public Texture GetBanner(string hash) => get(AssetType.Banner, hash);
    public Texture GetBackground(long id, AssetSize size = AssetSize.Normal) => get(AssetType.Background, id, size);
    public Texture GetCover(long id, AssetSize size = AssetSize.Normal) => get(AssetType.Cover, id, size);
    public Texture GetClubIcon(string id) => get(AssetType.ClubIcon, id);
    public Texture GetClubBanner(string id) => get(AssetType.ClubBanner, id);

    private Texture get(AssetType type, long id, AssetSize size = AssetSize.Normal) => get(type, id.ToString(), size);
    private Texture get(AssetType type, string id, AssetSize size = AssetSize.Normal) => string.IsNullOrEmpty(id) ? null : Get(GetUrl(api, type, id, size));

    public static string GetUrl(IAPIClient api, AssetType type, string id, AssetSize size = AssetSize.Normal)
    {
        var typeStr = type switch
        {
            AssetType.Achievement => "achievements",
            AssetType.Avatar or AssetType.Banner or AssetType.ClubIcon or AssetType.ClubBanner => "hashed",
            AssetType.Background => "backgrounds",
            AssetType.Cover => "covers",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        var builder = new StringBuilder();
        builder.Append($"{api.Endpoint.AssetUrl}/{typeStr}/{id}");

        switch (size)
        {
            case AssetSize.Small:
                builder.Append("-sm");
                break;

            case AssetSize.Large:
                builder.Append("-lg");
                break;
        }

        switch (type)
        {
            case AssetType.Achievement:
            case AssetType.Avatar:
            case AssetType.Banner:
            case AssetType.ClubIcon:
            case AssetType.ClubBanner:
                builder.Append(".png");
                break;

            case AssetType.Background:
            case AssetType.Cover:
                builder.Append(".jpg");
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

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
        Normal,
        Large
    }
}
