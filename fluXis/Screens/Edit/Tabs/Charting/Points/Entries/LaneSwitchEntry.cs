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
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Points.Entries;

public partial class LaneSwitchEntry : PointListEntry
{
    protected override string Text => "Lane Switch";
    protected override Colour4 Color => FluXisColors.LaneSwitch;

    private LaneSwitchEvent laneSwitch => Object as LaneSwitchEvent;

    public LaneSwitchEntry(LaneSwitchEvent laneSwitch)
        : base(laneSwitch)
    {
    }

    public override ITimedObject CreateClone() => laneSwitch.JsonCopy();

    protected override Drawable[] CreateValueContent()
    {
        return new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = $"{laneSwitch.Count}K {(int)laneSwitch.Duration}ms",
                Colour = Color
            }
        };
    }

    protected override IEnumerable<Drawable> CreateSettings()
    {
        return base.CreateSettings().Concat(new Drawable[]
        {
            new PointSettingsTextBox
            {
                Text = "Key Count",
                TooltipText = "The number of keys to switch to.",
                ExtraText = "K",
                TextBoxWidth = 50,
                DefaultText = laneSwitch.Count.ToString(),
                OnTextChanged = box =>
                {
                    if (int.TryParse(box.Text, out var result))
                    {
                        if (result > Map.RealmMap.KeyCount || result < 1)
                            box.NotifyError();
                        else
                            laneSwitch.Count = result;
                    }
                    else
                        box.NotifyError();

                    Map.Update(laneSwitch);
                }
            },
            new PointSettingsLength<LaneSwitchEvent>(Map, laneSwitch, BeatLength),
            new PointSettingsEasing<LaneSwitchEvent>(Map, laneSwitch)
        });
    }
}
