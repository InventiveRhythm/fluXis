using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Text;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Edit.Tabs.Setup.Entries;

public partial class SetupTextBox : SetupEntry, ITabbableContainer
{
    public bool CanBeTabbedTo => true;
    public override bool AcceptsFocus => true;

    protected override float ContentSpacing => -3;

    public string Default { get; init; } = string.Empty;
    public string Placeholder { get; init; } = string.Empty;
    public Action<string> OnChange { get; init; } = _ => { };

    public CompositeDrawable TabbableContentContainer
    {
        set => textBox.TabbableContentContainer = value;
    }

    private FluXisTextBox textBox;

    public SetupTextBox(string title)
        : base(title)
    {
    }

    protected override Drawable CreateContent()
    {
        return textBox = new FluXisTextBox
        {
            RelativeSizeAxes = Axes.X,
            Height = 24,
            Text = Default,
            FontSize = FluXisSpriteText.GetWebFontSize(18),
            SidePadding = 0,
            PlaceholderText = Placeholder,
            BackgroundActive = FluXisColors.Background3,
            BackgroundInactive = FluXisColors.Background3,
            OnTextChanged = () => OnChange.Invoke(textBox.Text),
            OnCommitAction = () => OnChange.Invoke(textBox.Text),
            CommitOnFocusLost = true,
        };
    }

    protected override void OnFocus(FocusEvent e)
    {
        // redirect focus to the textbox
        GetContainingFocusManager()?.ChangeFocus(textBox);
    }
}
