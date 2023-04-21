using System.Threading;
using fluXis.Game.Online.API;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Graphics;

public partial class DrawableBanner : Sprite
{
    private APIUserShort user;
    private TextureStore textures;

    private CancellationTokenSource cancellationToken = new();

    public DrawableBanner(APIUserShort user)
    {
        this.user = user ?? APIUserShort.Dummy;
        Alpha = 0;
        FillMode = FillMode.Fill;
    }

    [BackgroundDependencyLoader]
    private async void load(TextureStore textures)
    {
        this.textures = textures;
        Texture = await textures.GetAsync(user.BannerUrl, cancellationToken.Token);
    }

    protected override void LoadAsyncComplete()
    {
        this.FadeInFromZero(200);
        base.LoadAsyncComplete();
    }

    public async void UpdateUser(APIUserShort newUser)
    {
        cancellationToken.Cancel();
        cancellationToken = new CancellationTokenSource();

        user = newUser ?? APIUserShort.Dummy;
        Texture = await textures.GetAsync(user.BannerUrl, cancellationToken.Token);
    }
}
