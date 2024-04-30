using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Localisation;

namespace fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings;

public partial class PointSettingsToggle : Container, IHasTooltip
{
    public string Text { get; init; }
    public LocalisableString TooltipText { get; init; } = string.Empty;
    public bool CurrentValue { get; init; }
    public Action<bool> OnStateChanged { get; init; }
    public Bindable<bool> Bindable { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 32;

        Bindable ??= new Bindable<bool>(CurrentValue);

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = Text,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                WebFontSize = 16
            },
            new FluXisToggleSwitch
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                State = Bindable
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Bindable.BindValueChanged(valueChanged);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        Bindable.ValueChanged -= valueChanged;
    }

    private void valueChanged(ValueChangedEvent<bool> e) => OnStateChanged?.Invoke(e.NewValue);
}
