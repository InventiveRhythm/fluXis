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
        switch (AttributeType)
        {
            case AttributeType.Title:
                return Screen.RealmMap.Metadata.Title;

            case AttributeType.Artist:
                return Screen.RealmMap.Metadata.Artist;

            case AttributeType.Difficulty:
                return Screen.RealmMap.Difficulty;

            case AttributeType.Mapper:
                return Screen.RealmMap.Metadata.Mapper;

            default:
                return "";
        }
    }
}

public enum AttributeType
{
    Title,
    Artist,
    Difficulty,
    Mapper,
}
