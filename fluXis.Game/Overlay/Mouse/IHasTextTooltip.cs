using osu.Framework.Localisation;

namespace fluXis.Game.Overlay.Mouse;

public interface IHasTextTooltip
{
    LocalisableString Tooltip { get; }
}
