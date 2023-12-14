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

        UserCache.OnAvatarUpdate = id => purge(AssetType.Avatar, id);
        UserCache.OnBannerUpdate = id => purge(AssetType.Banner, id);
    }

    public Texture GetAvatar(int id) => get(AssetType.Avatar, id);
    public Texture GetBanner(int id) => get(AssetType.Banner, id);

    private Texture get(AssetType type, int id) => Get(getUrl(type, id));

    private string getUrl(AssetType type, int id)
    {
        var typeStr = type.ToString().ToLower();
        return $"{endpointConfig.AssetUrl}/{typeStr}/{id}";
    }

    private static string getLookup(string name, WrapMode wrapModeS = default, WrapMode wrapModeT = default)
        => $"{name}:wrap-{(int)wrapModeS}-{(int)wrapModeT}";

    private void purge(AssetType type, int id)
    {
        var url = getUrl(type, id);
        if (TryGetCached(getLookup(url), out var tex))
            Purge(tex);
    }

    public enum AssetType
    {
        Avatar,
        Banner,
        Background,
        Cover
    }
}
