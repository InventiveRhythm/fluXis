using System.Collections.Generic;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.UI.Visual;

public partial class SettingsVisualToggle : SettingsVisualSelect<bool>
{
    public string TextureOff { get; init; }
    public LocalisableString TooltipOff { get; init; }

    public string TextureOn { get; init; }
    public LocalisableString TooltipOn { get; init; }

    protected override IEnumerable<ToggleImage> CreateImages()
    {
        yield return new ToggleImage(TextureOff, Bindable, false) { TooltipText = TooltipOff };
        yield return new ToggleImage(TextureOn, Bindable, true) { TooltipText = TooltipOn };
    }
}
