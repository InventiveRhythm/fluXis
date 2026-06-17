using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Overlay.Settings;

public partial class SettingsSection : FillFlowContainer, IFilterable
{
    public virtual IconUsage Icon => Phosphor.Bold.GearSix;
    public virtual LocalisableString Title => "Section";

    public IEnumerable<SettingsSubSection> SubSections => InternalChildren.OfType<SettingsSubSection>().ToList();

    IEnumerable<LocalisableString> IHasFilterTerms.FilterTerms => [Title];
    bool IFilterable.FilteringActive { set { } }

    public event Action<bool> OnMatchingChanged;

    public bool MatchingFilter
    {
        set
        {
            OnMatchingChanged?.Invoke(value);
            this.FadeTo(value && CurrentSection.Value == this ? 1 : 0);
        }
    }

    public Bindable<SettingsSection> CurrentSection { get; set; } = new();

    protected SettingsSection()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(12);
        Alpha = 0;
    }
}
