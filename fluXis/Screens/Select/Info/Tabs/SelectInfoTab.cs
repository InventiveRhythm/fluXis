using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Screens.Select.Info.Tabs;

public abstract partial class SelectInfoTab : CompositeDrawable
{
    public abstract LocalisableString Title { get; }
    public abstract IconUsage Icon { get; }
    public virtual bool ShowBackdrop => false;

    public virtual Drawable CreateHeader() => Empty();
}
