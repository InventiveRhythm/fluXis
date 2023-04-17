using System.Threading;
using fluXis.Game.Online;
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
        this.user = user;
        Alpha = 0;
        FillMode = FillMode.Fill;
    }

    [BackgroundDependencyLoader]
    private async void load(TextureStore textures)
    {
        this.textures = textures;
        Texture = await textures.GetAsync($"{APIConstants.APIUrl}/assets/banner/{user?.ID ?? -1}", cancellationToken.Token);
    }

    protected override void LoadAsyncComplete()
    {
        this.FadeInFromZero(200);
        base.LoadAsyncComplete();
    }

    public async void UpdateUser(APIUserShort user)
    {
        cancellationToken.Cancel();
        cancellationToken = new CancellationTokenSource();

        this.user = user;
        Texture = await textures.GetAsync($"{APIConstants.APIUrl}/assets/banner/{user?.ID ?? -1}", cancellationToken.Token);
    }
}