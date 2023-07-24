using System;
using System.Globalization;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Panel;
using fluXis.Game.Map;
using fluXis.Game.Map.Events;
using fluXis.Game.Overlay.Notification;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Compose.Effect.EffectEdit;

public partial class FlashEditorPanel : Panel
{
    public FlashEvent Event { get; set; }
    public MapEvents MapEvents { get; init; }
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
                            Text = "Flash Editor",
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
                    LabelText = "Duration",
                    Text = (Event.Duration / beatLength).ToStringInvariant(),
                    OnTextChanged = textBox =>
                    {
                        if (float.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                            Event.Duration = result * beatLength;
                        else
                            textBox.NotifyError();
                    }
                },
                new LabelledTextBox
                {
                    LabelText = "Start Color",
                    Text = Event.StartColor.ToHex(),
                    OnTextChanged = textBox =>
                    {
                        try
                        {
                            string text = textBox.Text;
                            if (text.StartsWith("#"))
                                text = text.Remove(0, 1);

                            if (text.Length == 3)
                                text = $"{text[0]}{text[0]}{text[1]}{text[1]}{text[2]}{text[2]}";

                            if (text.Length != 6)
                                throw new Exception();

                            var color = Colour4.FromHex(text);
                            Event.StartColor = color;
                        }
                        catch
                        {
                            textBox.NotifyError();
                        }
                    }
                },
                new LabelledTextBox
                {
                    LabelText = "Start Opacity",
                    Text = Event.StartOpacity.ToStringInvariant(),
                    OnTextChanged = textBox =>
                    {
                        if (float.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                        {
                            if (result is >= 0 and <= 1)
                                Event.StartOpacity = result;
                            else
                                textBox.NotifyError();
                        }
                        else
                            textBox.NotifyError();
                    }
                },
                new LabelledTextBox
                {
                    LabelText = "End Color",
                    Text = Event.EndColor.ToHex(),
                    OnTextChanged = textBox =>
                    {
                        try
                        {
                            string text = textBox.Text;
                            if (text.StartsWith("#"))
                                text = text.Remove(0, 1);

                            if (text.Length == 3)
                                text = $"{text[0]}{text[0]}{text[1]}{text[1]}{text[2]}{text[2]}";

                            if (text.Length != 6)
                                throw new Exception();

                            var color = Colour4.FromHex(text);
                            Event.EndColor = color;
                        }
                        catch
                        {
                            textBox.NotifyError();
                        }
                    }
                },
                new LabelledTextBox
                {
                    LabelText = "End Opacity",
                    Text = Event.EndOpacity.ToStringInvariant(),
                    OnTextChanged = textBox =>
                    {
                        if (float.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                        {
                            if (result is >= 0 and <= 1)
                                Event.EndOpacity = result;
                            else
                                textBox.NotifyError();
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
                                MapEvents.FlashEvents.Remove(Event);
                                Hide();
                            }
                        },
                        new()
                        {
                            Width = 100,
                            Height = 40,
                            Text = "Cancel",
                            Action = Hide
                        }
                    }
                }
            }
        };
    }
}
