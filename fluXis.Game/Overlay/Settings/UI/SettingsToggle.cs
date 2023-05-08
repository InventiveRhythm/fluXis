using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;

namespace fluXis.Game.Overlay.Settings.UI;

public partial class SettingsToggle : SettingsItem
{
    public Bindable<bool> Bindable { get; init; } = new();

    private SpriteIcon icon;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SpriteText
            {
                Text = Label,
                Font = FluXisFont.Default(24),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft
            },
            icon = new SpriteIcon
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Size = new(25),
                Icon = FontAwesome.Solid.Check
            }
        });

        Bindable.BindValueChanged(e => icon.FadeTo(e.NewValue ? 1 : 0.4f), true);
    }

    protected override bool OnClick(ClickEvent e)
    {
        Bindable.Value = !Bindable.Value;
        return true;
    }
}
