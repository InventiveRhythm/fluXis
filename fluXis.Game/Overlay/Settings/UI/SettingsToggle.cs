using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.Settings.UI;

public partial class SettingsToggle : SettingsItem
{
    public override bool IsDefault => Bindable.IsDefault;

    public Bindable<bool> Bindable { get; init; } = new();

    private ToggleIcon icon;
    private Sample toggleOn;
    private Sample toggleOff;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        toggleOn = samples.Get(@"UI/toggle-on");
        toggleOff = samples.Get(@"UI/toggle-off");

        AddRange(new Drawable[]
        {
            icon = new ToggleIcon()
        });

        Bindable.BindValueChanged(e => icon.UpdateValue(e.NewValue), true);
        icon.UpdateHover(false);
    }

    protected override void Reset() => Bindable.SetDefault();

    protected override bool OnClick(ClickEvent e)
    {
        Bindable.Value = !Bindable.Value;

        if (Bindable.Value)
            toggleOn?.Play();
        else
            toggleOff?.Play();

        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        icon.UpdateHover(true);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        icon.UpdateHover(false);
    }

    private partial class ToggleIcon : CircularContainer
    {
        public int BorderSize { get; init; } = size_off;

        private const int size_off = 4;
        private const int size_off_hover = 6;
        private const int size_on = 13;
        private const int size_on_hover = 9;

        private bool value;

        [BackgroundDependencyLoader]
        private void load()
        {
            Anchor = Anchor.CentreRight;
            Origin = Anchor.CentreRight;
            Size = new Vector2(25);
            Masking = true;
            BorderColour = Colour4.White;
            BorderThickness = BorderSize;
            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
                AlwaysPresent = true
            };
        }

        protected override void Update()
        {
            base.Update();
            BorderThickness = BorderSize;
        }

        public void UpdateHover(bool hovered)
        {
            if (value)
                borderSizeTo(hovered ? size_on_hover : size_on);
            else
                borderSizeTo(hovered ? size_off_hover : size_off);
        }

        public void UpdateValue(bool v)
        {
            value = v;
            borderSizeTo(value ? size_on_hover : size_off_hover);
        }

        private void borderSizeTo(int size) => this.TransformTo(nameof(BorderSize), size, 200, Easing.OutQuint);
    }
}
