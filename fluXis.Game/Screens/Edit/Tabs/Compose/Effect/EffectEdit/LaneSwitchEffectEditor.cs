using System.Globalization;
using fluXis.Game.Graphics;
using fluXis.Game.Map;
using fluXis.Game.Map.Events;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Compose.Effect.EffectEdit;

public partial class LaneSwitchEffectEditor : FluXisPopover
{
    public LaneSwitchEvent LaneSwitchEvent { get; init; }
    public MapInfo MapInfo { get; init; }

    [Resolved]
    private EditorClock clock { get; set; }

    private float beatLength => clock.MapInfo.GetTimingPoint(LaneSwitchEvent.Time).MsPerBeat;

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Y,
            Width = 250,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 5),
            Children = new Drawable[]
            {
                new LabelledTextBox
                {
                    LabelText = "Time",
                    Text = LaneSwitchEvent.Time.ToStringInvariant(),
                    OnTextChanged = textBox =>
                    {
                        if (float.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                            LaneSwitchEvent.Time = result;
                        else
                            textBox.NotifyError();
                    }
                },
                new BeatsTextBox
                {
                    LabelText = "Speed",
                    Text = (LaneSwitchEvent.Speed / beatLength).ToStringInvariant(),
                    OnTextChanged = textBox =>
                    {
                        if (float.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                            LaneSwitchEvent.Speed = result * beatLength;
                        else
                            textBox.NotifyError();
                    }
                },
                new LabelledTextBox
                {
                    LabelText = "Lanes",
                    Text = LaneSwitchEvent.Count.ToString(),
                    OnTextChanged = textBox =>
                    {
                        if (int.TryParse(textBox.Text, out var result))
                        {
                            if (result > MapInfo.KeyCount || result < 1)
                                textBox.NotifyError();
                            else
                                LaneSwitchEvent.Count = result;
                        }
                        else
                            textBox.NotifyError();
                    }
                }
            }
        };
    }
}
