using System.Globalization;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Panel;
using fluXis.Game.Map;
using fluXis.Game.Map.Events;
using fluXis.Game.Overlay.Notification;
using fluXis.Game.Screens.Edit.Tabs.Charting.Effect.UI;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Effect.EffectEdit;

public partial class LaneSwtichEditorPanel : Panel
{
    public LaneSwitchEvent Event { get; set; }
    public MapInfo MapInfo { get; init; }
    public EditorMapEvents MapEvents { get; init; }
    public EditorClock EditorClock { get; set; }

    [Resolved]
    private NotificationOverlay notifications { get; set; }

    private float beatLength => EditorClock.MapInfo.GetTimingPoint(Event.Time).MsPerBeat;

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 400;
        AutoSizeAxes = Axes.Y;
        Content.RelativeSizeAxes = Axes.X;
        Content.AutoSizeAxes = Axes.Y;

        Content.Child = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Spacing = new Vector2(10),
            Direction = FillDirection.Vertical,
            Children = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Children = new Drawable[]
                    {
                        new FluXisSpriteText
                        {
                            Text = "Lane Switch Editor",
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            FontSize = 30
                        },
                        new ClickableSpriteIcon
                        {
                            Anchor = Anchor.TopRight,
                            Origin = Anchor.TopRight,
                            Icon = FontAwesome.Solid.Question,
                            Size = new Vector2(20),
                            Margin = new MarginPadding(5),
                            Action = () => notifications.Post("Not implemented yet!")
                        }
                    }
                },
                new LabelledTextBox
                {
                    LabelText = "Time",
                    Text = Event.Time.ToStringInvariant(),
                    OnTextChanged = textBox =>
                    {
                        if (float.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                            Event.Time = result;
                        else
                            textBox.NotifyError();
                    }
                },
                new BeatsTextBox
                {
                    LabelText = "Speed",
                    Text = (Event.Speed / beatLength).ToStringInvariant(),
                    OnTextChanged = textBox =>
                    {
                        if (float.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                            Event.Speed = result * beatLength;
                        else
                            textBox.NotifyError();
                    }
                },
                new LabelledTextBox
                {
                    LabelText = "Lanes",
                    Text = Event.Count.ToString(),
                    OnTextChanged = textBox =>
                    {
                        if (int.TryParse(textBox.Text, out var result))
                        {
                            if (result > MapInfo.KeyCount || result < 1)
                                textBox.NotifyError();
                            else
                                Event.Count = result;
                        }
                        else
                            textBox.NotifyError();
                    }
                },
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(10),
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Children = new FluXisButton[]
                    {
                        new()
                        {
                            Width = 100,
                            Height = 40,
                            Text = "Delete",
                            Color = FluXisColors.ButtonRed,
                            Action = () =>
                            {
                                MapEvents.Remove(Event);
                                Hide();
                            }
                        },
                        new()
                        {
                            Width = 100,
                            Height = 40,
                            Text = "Close",
                            Action = Hide
                        }
                    }
                }
            }
        };
    }
}
