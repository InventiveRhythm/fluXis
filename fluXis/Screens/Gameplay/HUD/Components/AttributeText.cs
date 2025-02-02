using fluXis.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Screens.Gameplay.HUD.Components;

public partial class AttributeText : GameplayHUDComponent
{
    [BackgroundDependencyLoader]
    private void load()
    {
        var type = Settings.GetSetting("type", AttributeType.Title);
        var size = (float)Settings.GetSetting("size", 24d); // the json gets parsed as a double
        var text = Settings.GetSetting("text", "{value}");
        var maxWidth = Settings.GetSetting("max-width", 0d);

        AutoSizeAxes = Axes.Both;

        var drawable = new TruncatingText
        {
            FontSize = size,
            Shadow = true,
            Text = text.Replace("{value}", getValue(type))
        };

        if (maxWidth > 0)
            drawable.MaxWidth = (float)maxWidth;

        Add(drawable);
    }

    private string getValue(AttributeType type)
    {
        return type switch
        {
            AttributeType.Title => Deps.RealmMap.Metadata.Title,
            AttributeType.Artist => Deps.RealmMap.Metadata.Artist,
            AttributeType.Difficulty => Deps.RealmMap.Difficulty,
            AttributeType.Mapper => Deps.RealmMap.Metadata.Mapper,
            _ => ""
        };
    }
}

public enum AttributeType : long
{
    Title = 0,
    Artist = 1,
    Difficulty = 2,
    Mapper = 3
}
