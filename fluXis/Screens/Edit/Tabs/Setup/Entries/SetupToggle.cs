using System;
using fluXis.Graphics.Sprites.Text;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Edit.Tabs.Setup.Entries;

public partial class SetupToggle : SetupEntry
{
    protected override float ContentSpacing => -2;
    protected override bool ShowHoverFlash => true;

    public Action<bool> OnChange { get; init; } = _ => { };

    private ForcedHeightText text;
    private Bindable<bool> bindable { get; }

    public SetupToggle(string title, bool value)
        : this(title, new Bindable<bool>(value))
    {
    }

    public SetupToggle(string title, Bindable<bool> bind)
        : base(title)
    {
        bindable = bind;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        bindable.BindValueChanged(v =>
        {
            text.Text = v.NewValue ? "Enabled" : "Disabled";
            OnChange?.Invoke(v.NewValue);

            if (v.NewValue) StartHighlight();
            else StopHighlight();
        }, true);
    }

    protected override Drawable CreateContent() => text = new ForcedHeightText
    {
        Text = "Enabled",
        WebFontSize = 18,
        Height = 24
    };

    protected override bool OnClick(ClickEvent e)
    {
        bindable.Value = !bindable.Value;
        return base.OnClick(e);
    }
}
