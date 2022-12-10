using fluXis.Game.Online;
using fluXis.Game.Online.API;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Graphics
{
    public class DrawableAvatar : Sprite
    {
        private readonly APIUserShort user;

        public DrawableAvatar(APIUserShort user)
        {
            this.user = user;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            Texture = textures.Get($"{APIConstants.API_URL}/assets/avatar/{user.ID}");
        }
    }
}
