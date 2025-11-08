using System;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Text;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Edit.Tabs.Setup.Entries;

public partial class SetupTextBox : SetupEntry, ITabbableContainer
{
    public bool CanBeTabbedTo => true;
    public override bool AcceptsFocus => true;

    protected override float ContentSpacing => -2;

    public string Value
    {
        get => textBox.Text;
        set => textBox.Text = value;
    }

    public string Default { get; init; } = string.Empty;
    public string Placeholder { get; init; } = string.Empty;
    public Action<string> OnChange { get; init; } = _ => { };
    public int MaxLength { get; init; } = 256;
    public bool ReadOnly { get; init; }

    public CompositeDrawable TabbableContentContainer
    {
        set => textBox.TabbableContentContainer = value;
    }

    private FluXisTextBox textBox;

    public SetupTextBox(string title)
        : base(title)
    {
    }

    protected override Drawable CreateContent() => textBox = new FluXisTextBox
    {
        RelativeSizeAxes = Axes.X,
        Height = 24,
        Text = Default,
        FontSize = FluXisSpriteText.GetWebFontSize(18),
        SidePadding = 0,
        PlaceholderText = Placeholder,
        BackgroundActive = BackgroundColor,
        BackgroundInactive = BackgroundColor,
        OnTextChanged = () => OnChange.Invoke(textBox.Text),
        OnCommitAction = () => OnChange.Invoke(textBox.Text),
        OnFocusAction = StartHighlight,
        OnFocusLostAction = StopHighlight,
        CommitOnFocusLost = true,
        LengthLimit = MaxLength,
        ReadOnly = ReadOnly,
        Alpha = ReadOnly ? 0.6f : 1f
    };

    protected override void OnFocus(FocusEvent e)
    {
        if (ReadOnly) return;

        redirect();
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (ReadOnly) return false;

        redirect();
        return true;
    }

    private void redirect() => GetContainingFocusManager()?.ChangeFocus(textBox);
}
