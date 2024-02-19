using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Game.Overlay.Settings.UI;

public partial class SettingsTextBox : SettingsItem
{
    public Bindable<string> Bindable { get; init; }

    protected override bool IsDefault => Bindable.IsDefault;

    private FluXisTextBox box;

    [BackgroundDependencyLoader]
    private void load()
    {
        Add(box = new FluXisTextBox
        {
            Width = 400,
            RelativeSizeAxes = Axes.Y,
            SidePadding = 10,
            Text = Bindable.Value,
            Anchor = Anchor.CentreRight,
            Origin = Anchor.CentreRight,
            CornerRadius = 10,
            BackgroundInactive = FluXisColors.Background3,
            BackgroundActive = FluXisColors.Background4,
            FontSize = FluXisSpriteText.GetWebFontSize(16),
            OnTextChanged = () => Bindable.Value = box.Text,
            OnCommitAction = () => Bindable.Value = box.Text
        });
    }

    protected override void Reset() => Bindable.SetDefault();
}
