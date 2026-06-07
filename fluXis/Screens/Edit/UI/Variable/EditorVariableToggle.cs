using System;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.UI.Variable;

public partial class EditorVariableToggle : EditorVariableBase
{
    public bool CurrentValue { get; init; }
    public Action<bool> OnValueChanged { get; init; }

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

    private void valueChanged(ValueChangedEvent<bool> e) => OnValueChanged?.Invoke(e.NewValue);
}
