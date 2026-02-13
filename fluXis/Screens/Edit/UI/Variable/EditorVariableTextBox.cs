using System;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Edit.UI.Variable;

public partial class EditorVariableTextBox : EditorVariableBase
{
    public string CurrentValue { get; init; }
    public Action<FluXisTextBox> OnValueChanged { get; set; }
    public Action<FluXisTextBox> OnCommit { get; set; }

    public string ExtraText { get; init; }
    public int TextBoxWidth { get; init; } = 210;

    public FluXisTextBox TextBox { get; private set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 32;

        FillFlowContainer leftText;

        InternalChildren = new Drawable[]
        {
            leftText = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Direction = FillDirection.Vertical,
                Child = new ForcedHeightText
                {
                    Text = Text,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    WebFontSize = 16,
                    Height = 16
                }
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(5),
                Children = new[]
                {
                    TextBox = new FluXisTextBox
                    {
                        Width = TextBoxWidth,
                        RelativeSizeAxes = Axes.Y,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Text = CurrentValue,
                        SidePadding = 10,
                        TextContainerHeight = .7f,
                        CommitOnFocusLost = true,
                        BackgroundInactive = Theme.Background3,
                        BackgroundActive = Theme.Background4,
                        OnTextChanged = () => OnValueChanged?.Invoke(TextBox),
                        OnCommitAction = () =>
                        {
                            OnValueChanged?.Invoke(TextBox);
                            OnCommit?.Invoke(TextBox);
                        }
                    },
                    new FluXisSpriteText
                    {
                        Text = ExtraText,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        WebFontSize = 16,
                        Alpha = string.IsNullOrEmpty(ExtraText) ? 0 : 1
                    },
                    CreateExtraButton().With(d =>
                    {
                        d.Anchor = Anchor.CentreLeft;
                        d.Origin = Anchor.CentreLeft;
                    })
                }
            }
        };

        UpdateLeftTextFlow(leftText);
    }

    protected virtual Drawable CreateExtraButton() => Empty().With(d => d.Alpha = 0);
    protected virtual void UpdateLeftTextFlow(FillFlowContainer flow) { }
}
