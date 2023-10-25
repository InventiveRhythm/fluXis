using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Gameplay.HUD.Components;

public partial class AttributeText : GameplayHUDComponent
{
    [BackgroundDependencyLoader]
    private void load()
    {
        var type = Settings.GetSetting("type", AttributeType.Title);
        var size = (float)Settings.GetSetting("size", 32d); // the json gets parsed as a double
        var text = Settings.GetSetting("text", "{value}");

        AutoSizeAxes = Axes.Both;

        Add(new FluXisSpriteText
        {
            FontSize = size,
            Shadow = true,
            Text = text.Replace("{value}", getValue(type))
        });
    }

    private string getValue(AttributeType type)
    {
        return type switch
        {
            AttributeType.Title => Screen.RealmMap.Metadata.Title,
            AttributeType.Artist => Screen.RealmMap.Metadata.Artist,
            AttributeType.Difficulty => Screen.RealmMap.Difficulty,
            AttributeType.Mapper => Screen.RealmMap.Metadata.Mapper,
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
