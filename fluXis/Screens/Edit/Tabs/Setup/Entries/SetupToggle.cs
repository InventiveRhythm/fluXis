using System;
using fluXis.Graphics.Sprites;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Edit.Tabs.Setup.Entries;

public partial class SetupToggle : SetupEntry
{
    protected override float ContentSpacing => -3;
    protected override bool ShowHoverFlash => true;

    public Action<bool> OnChange { get; init; } = _ => { };

    private TruncatingText text;
    private BindableBool bindable { get; }

    public SetupToggle(string title, bool value)
        : base(title)
    {
        bindable = new BindableBool(value);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        bindable.BindValueChanged(v =>
        {
            text.Text = v.NewValue ? "Enabled" : "Disabled";
            OnChange?.Invoke(v.NewValue);
        }, true);
    }

    protected override Drawable CreateContent()
    {
        return text = new TruncatingText
        {
            RelativeSizeAxes = Axes.X,
            Text = "Enabled",
            WebFontSize = 18
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        bindable.Toggle();
        return base.OnClick(e);
    }
}
