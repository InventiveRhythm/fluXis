using fluXis.Graphics.Sprites.Text;
using fluXis.Utils.Attributes;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Screens.Gameplay.HUD.Components;

public partial class AttributeText : GameplayHUDComponent
{
    [BindableSetting("Type", "The data to replace '{value}' with.", "type")]
    public Bindable<AttributeType> Type { get; } = new();

    [BindableSetting("Font Size", "The size of the font.", "size")]
    public BindableFloat FontSize { get; } = new(24) { MinValue = 8, MaxValue = 96, Precision = 1 };

    [BindableSetting("Text", "The text to display.", "text")]
    public Bindable<string> TextTemplate { get; } = new("{value}");

    [BindableSetting("Max Width", "The maximum width of the text. Applies truncation using dots when reached.", "max-width")]
    public BindableFloat MaxWidth { get; } = new() { MinValue = 0, MaxValue = 2000, Precision = 1 };

    private TruncatingText text;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;

        InternalChild = text = new TruncatingText
        {
            Shadow = true,
            Text = TextTemplate.Value.Replace("{value}", getValue(Type.Value))
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Type.BindValueChanged(_ => updateText(), true);
        FontSize.BindValueChanged(_ => text.FontSize = FontSize.Value, true);
        TextTemplate.BindValueChanged(_ => updateText(), true);
        MaxWidth.BindValueChanged(_ => text.MaxWidth = MaxWidth.Value > 0 ? MaxWidth.Value : float.PositiveInfinity, true);
    }

    private void updateText() => text.Text = TextTemplate.Value.Replace("{value}", getValue(Type.Value));

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
