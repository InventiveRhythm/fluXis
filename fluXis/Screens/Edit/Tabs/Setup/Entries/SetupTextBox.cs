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

    protected override float ContentSpacing => -3;

    public string Default { get; init; } = string.Empty;
    public string Placeholder { get; init; } = string.Empty;
    public Action<string> OnChange { get; init; } = _ => { };
    public int MaxLength { get; init; } = 256;

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
        LengthLimit = MaxLength
    };

    protected override void OnFocus(FocusEvent e) => redirect();

    protected override bool OnClick(ClickEvent e)
    {
        redirect();
        return true;
    }

    private void redirect() => GetContainingFocusManager()?.ChangeFocus(textBox);
}
