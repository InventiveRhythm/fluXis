using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Utils.Extensions;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Graphics.UserInterface.Form;

public partial class FormInput : BaseFormComponent<string, FormInput>, ITabbableContainer, IHasTooltip
{
    LocalisableString IHasTooltip.TooltipText => Description ?? "";

    public bool CanBeTabbedTo => !ReadOnly;
    public override bool AcceptsFocus => !ReadOnly;

    public CompositeDrawable TabbableContentContainer { set => textbox.TabbableContentContainer = value; }

    public LocalisableString Placeholder { get; init; }
    public int MaxLength { get; init; } = 256;
    public bool ReadOnly { get; init; }
    public bool Password { get; init; }

    private FluXisTextBox textbox;

    public FormInput(LocalisableString label, string value)
        : this(label, new Bindable<string> { Value = value })
    {
    }

    public FormInput(LocalisableString label, Bindable<string> bind)
        : base(label, bind)
    {
    }

    protected override Drawable CreateContent() => new FillFlowContainer
    {
        RelativeSizeAxes = Axes.Both,
        Direction = FillDirection.Vertical,
        Spacing = new Vector2(INNER_GAP),
        Padding = new MarginPadding(PADDING),
        Children =
        [
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 16,
                Child = LabelSprite = CreateDefaultLabel().WithAnchor(Anchor.CentreLeft)
            },
            textbox = new FluXisTextBox
            {
                RelativeSizeAxes = Axes.X,
                Height = 22,
                Text = Bindable.Value,
                PlaceholderText = Placeholder,
                LengthLimit = MaxLength,
                FontSize = FluXisSpriteText.GetWebFontSize(18),
                BackgroundActive = Theme.Background3,
                BackgroundInactive = Theme.Background3,
                SidePadding = 0,
                OnFocusAction = StartHighlight,
                OnFocusLostAction = StopHighlight,
                OnTextChanged = () => Bindable.Value = textbox.Text,
                OnCommitAction = () => Bindable.Value = textbox.Text,
                CommitOnFocusLost = true,
                IsPassword = Password,
                ReadOnly = ReadOnly,
                Alpha = ReadOnly ? .6f : 1f
            }
        ]
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

    private void redirect() => GetContainingFocusManager()?.ChangeFocus(textbox);
}
