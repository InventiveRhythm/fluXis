using fluXis.Game.Online.API;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Online.Overlay.Sidebar
{
    public class SidebarBanner : Sprite
    {
        private readonly APIUserShort user;

        public SidebarBanner(APIUserShort user)
        {
            this.user = user;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            FillMode = FillMode.Fill;
            Texture = textures.Get($"{APIConstants.API_URL}/assets/banner/{user.ID}");
        }
    }
}
