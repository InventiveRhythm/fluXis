using fluXis.Game.Online.API;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Online.Overlay.Sidebar
{
    public class SidebarBanner : Sprite
    {
        private APIUserShort user;
        private TextureStore textures;

        public SidebarBanner(APIUserShort user)
        {
            this.user = user;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            this.textures = textures;
            FillMode = FillMode.Fill;
            Texture = textures.Get($"{APIConstants.API_URL}/assets/banner/{user?.ID ?? -1}");
        }

        public void UpdateUser(APIUserShort user)
        {
            this.user = user;
            Texture = textures.Get($"{APIConstants.API_URL}/assets/banner/{user?.ID ?? -1}");
        }
    }
}
