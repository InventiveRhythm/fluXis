using System;
using System.Collections.Generic;
using System.Linq;

namespace fluXis.Overlay.Settings.UI.Visual;

public partial class SettingsVisualEnum<T> : SettingsVisualSelect<T>
    where T : struct, Enum
{
    public string TexturePrefix { get; init; }

    protected override IEnumerable<ToggleImage> CreateImages()
        => Enum.GetValues<T>().Select(value => new ToggleImage($"{TexturePrefix}{value.ToString()}", Bindable, value) { TooltipText = value.ToString() });
}
