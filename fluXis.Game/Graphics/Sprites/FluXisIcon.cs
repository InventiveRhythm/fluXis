using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Graphics.Sprites;

public partial class FluXisIcon : Sprite
{
    [Resolved]
    private TextureStore textures { get; set; }

    private FluXisIconType type = FluXisIconType.LaneSwitch;

    public FluXisIconType Type
    {
        get => type;
        set
        {
            type = value;

            if (IsLoaded)
                Texture = textures.Get($"Map/Icons/{Type}.png");
        }
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        FillMode = FillMode.Fit;
        FillAspectRatio = 1;
        Texture = textures.Get($"Icons/{Type}");
    }
}

public enum FluXisIconType
{
    LaneSwitch,
    Flash,
    Pulse,
    PlayfieldMove,
    PlayfieldScale,
    Shake
}
