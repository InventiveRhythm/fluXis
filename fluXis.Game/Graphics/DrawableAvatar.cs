using fluXis.Game.Online;
using fluXis.Game.Online.API;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Graphics
{
    public partial class DrawableAvatar : Sprite
    {
        private APIUserShort user;
        private TextureStore textures;

        public DrawableAvatar(APIUserShort user)
        {
            this.user = user;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            this.textures = textures;
            Texture = textures.Get($"{APIConstants.API_URL}/assets/avatar/{user?.ID ?? -1}");
        }

        public void UpdateUser(APIUserShort user)
        {
            this.user = user;
            Texture = textures.Get($"{APIConstants.API_URL}/assets/avatar/{user?.ID ?? -1}");
        }
    }
}
