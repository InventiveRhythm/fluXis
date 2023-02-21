using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;

namespace fluXis.Game.Overlay.Settings.UI;

public partial class SettingsToggle : SettingsItem
{
    public Bindable<bool> Bindable { get; }

    public SettingsToggle(Bindable<bool> bindable, string label)
    {
        Bindable = bindable;

        Add(new SpriteText
        {
            Text = label,
            Font = new FontUsage("Quicksand", 24, "Bold"),
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,
        });

        Add(new Checkbox(bindable));
    }

    protected override bool OnClick(ClickEvent e)
    {
        Bindable.Value = !Bindable.Value;
        return true;
    }

    public partial class Checkbox : CircularContainer
    {
        public Bindable<bool> Bindable { get; }

        private readonly Box box;

        public Checkbox(Bindable<bool> bindable)
        {
            Bindable = bindable;

            RelativeSizeAxes = Axes.Y;
            Width = 20;
            Anchor = Anchor.CentreRight;
            Origin = Anchor.CentreRight;
            Masking = true;
            BorderColour = Colour4.White;
            BorderThickness = 2;

            AddRange(new Drawable[]
            {
                box = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.White
                }
            });

            Bindable.BindValueChanged(_ => updateState(), true);
        }

        private void updateState()
        {
            box.FadeTo(Bindable.Value ? 1 : 0);
        }
    }
}
