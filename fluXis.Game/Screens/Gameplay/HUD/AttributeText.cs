using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Gameplay.HUD;

public partial class AttributeText : GameplayHUDElement
{
    public string Text { get; init; } = "{value}";
    public float FontSize { get; init; } = 32;
    public AttributeType AttributeType { get; init; } = AttributeType.Title;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;

        Add(new FluXisSpriteText
        {
            FontSize = FontSize,
            Text = Text.Replace("{value}", getValue())
        });
    }

    private string getValue()
    {
        return AttributeType switch
        {
            AttributeType.Title => Screen.RealmMap.Metadata.Title,
            AttributeType.Artist => Screen.RealmMap.Metadata.Artist,
            AttributeType.Difficulty => Screen.RealmMap.Difficulty,
            AttributeType.Mapper => Screen.RealmMap.Metadata.Mapper,
            _ => ""
        };
    }
}

public enum AttributeType
{
    Title,
    Artist,
    Difficulty,
    Mapper
}
