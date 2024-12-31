using System;
using fluXis.Audio;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;

namespace fluXis.Graphics.Sprites;

public partial class ClickableSpriteIcon : SpriteIcon
{
    private Action action;

    public Action Action
    {
        get => action;
        set
        {
            action = value;
            Enabled.Value = action != null;
        }
    }

    [Resolved]
    private UISamples samples { get; set; }

    public readonly BindableBool Enabled = new();

    protected override bool OnClick(ClickEvent e)
    {
        if (Enabled.Value)
        {
            Action?.Invoke();
            samples.Click();
        }

        return true;
    }
}
