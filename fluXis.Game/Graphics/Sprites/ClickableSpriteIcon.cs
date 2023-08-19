using System;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;

namespace fluXis.Game.Graphics.Sprites;

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

    public readonly BindableBool Enabled = new();

    protected override bool OnClick(ClickEvent e)
    {
        if (Enabled.Value)
            Action?.Invoke();
        return true;
    }
}
