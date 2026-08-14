using System;
using fluXis.Graphics.Sprites.Text;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Edit.Tabs.Setup.Entries;

public partial class SetupButton : SetupEntry
{
    private readonly string subtitle;
    private readonly Action action;

    public SetupButton(string title, string subtitle, Action action)
        : base(title)
    {
        this.subtitle = subtitle;
        this.action = action;
    }

    protected override Drawable CreateContent() => new ForcedHeightText
    {
        Text = subtitle,
        WebFontSize = 18,
        Height = 24
    };

    protected override bool OnClick(ClickEvent e)
    {
        action?.Invoke();
        return true;
    }
}
