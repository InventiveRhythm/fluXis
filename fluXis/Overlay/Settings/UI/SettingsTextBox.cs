using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Overlay.Settings.UI;

public partial class SettingsTextBox : SettingsItem
{
    public Bindable<string> Bindable { get; init; }

    protected override bool IsDefault => Bindable.IsDefault;

    private FluXisTextBox box;

    protected override Drawable CreateContent()
    {
        Content.AutoSizeAxes = Axes.X;
        Content.RelativeSizeAxes = Axes.Y;

        return box = new FluXisTextBox
        {
            Width = 400,
            RelativeSizeAxes = Axes.Y,
            SidePadding = 12,
            Text = Bindable.Value,
            CornerRadius = 8,
            BackgroundInactive = Theme.Background3,
            BackgroundActive = Theme.Background4,
            FontSize = FluXisSpriteText.GetWebFontSize(16),
            OnTextChanged = () => Bindable.Value = box.Text,
            OnCommitAction = () => Bindable.Value = box.Text
        };
    }

    protected override void Reset() => Bindable.SetDefault();
}
