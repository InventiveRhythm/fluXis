using System;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Shared.Points.Settings;

public partial class PointSettingsTextBox : PointSettingsBase, IHasTooltip
{
    public string Text { get; init; }
    public LocalisableString TooltipText { get; init; } = string.Empty;
    public string DefaultText { get; init; }
    public string ExtraText { get; init; }
    public int TextBoxWidth { get; init; } = 210;
    public Action<FluXisTextBox> OnTextChanged { get; set; }
    public Action<FluXisTextBox> OnCommit { get; set; }

    public FluXisTextBox TextBox { get; private set; }

    public Bindable<bool> Enabled { get; init; } = new(true);

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
            new FluXisSpriteText
            {
                Text = Text,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                WebFontSize = 16
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
                        Text = DefaultText,
                        SidePadding = 10,
                        TextContainerHeight = .7f,
                        CommitOnFocusLost = true,
                        BackgroundInactive = Theme.Background3,
                        BackgroundActive = Theme.Background4,
                        OnTextChanged = () => OnTextChanged?.Invoke(TextBox),
                        OnCommitAction = () =>
                        {
                            OnTextChanged?.Invoke(TextBox);
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

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Enabled.BindValueChanged(e => this.FadeTo(e.NewValue ? 1f : .4f, 200), true);
    }

    protected virtual Drawable CreateExtraButton() => Empty().With(d => d.Alpha = 0);
    protected virtual void UpdateLeftTextFlow(FillFlowContainer flow) { }
}
