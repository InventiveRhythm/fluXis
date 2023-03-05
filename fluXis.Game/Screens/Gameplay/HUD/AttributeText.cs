using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Gameplay.HUD;

public partial class AttributeText : GameplayHUDElement
{
    private readonly SpriteText sprText;

    private string text = string.Empty;

    public string Text
    {
        get => text;
        set
        {
            sprText.Text = value.Replace("{value}", getValue());
            text = value;
        }
    }

    public float FontSize
    {
        get => sprText.Size.Y;
        set => sprText.Font = new FontUsage("Quicksand", value, "Bold");
    }

    private AttributeType attributeType;

    public AttributeType AttributeType
    {
        get => attributeType;
        set
        {
            attributeType = value;
            Text = text;
        }
    }

    public AttributeText(GameplayScreen screen)
        : base(screen)
    {
        AutoSizeAxes = Axes.Both;

        Add(sprText = new SpriteText());

        //default values
        FontSize = 32;
        AttributeType = AttributeType.Title;
        Text = "{value}";
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
