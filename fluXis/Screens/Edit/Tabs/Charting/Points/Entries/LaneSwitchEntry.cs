using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Utils;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Points.Entries;

public partial class LaneSwitchEntry : PointListEntry
{
    protected override string Text => "Lane Switch";
    protected override Colour4 Color => Theme.LaneSwitch;

    private LaneSwitchEvent laneSwitch => Object as LaneSwitchEvent;

    public LaneSwitchEntry(LaneSwitchEvent laneSwitch)
        : base(laneSwitch)
    {
    }

    public override ITimedObject CreateClone() => laneSwitch.JsonCopy();

    protected override Drawable[] CreateValueContent() => new FluXisSpriteText
    {
        Text = $"{laneSwitch.Count}K {(int)laneSwitch.Duration}ms",
        Colour = Color
    }.Yield().ToArray<Drawable>();

    protected override IEnumerable<Drawable> CreateSettings() => base.CreateSettings().Concat(new Drawable[]
    {
        new PointSettingsNumber<int>
        {
            Text = "Key Count",
            TooltipText = "The number of keys to switch to.",
            ExtraText = "K",
            TextBoxWidth = 50,
            DefaultValue = laneSwitch.Count,
            Min = 1,
            Max = Map.RealmMap.KeyCount,
            OnValueChanged = v =>
            {
                laneSwitch.Count = v;
                Map.Update(laneSwitch);
            }
        },
        new PointSettingsLength<LaneSwitchEvent>(Map, laneSwitch, BeatLength),
        new PointSettingsEasing<LaneSwitchEvent>(Map, laneSwitch)
    });
}
