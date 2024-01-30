using System;
using System.Globalization;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs;

public partial class EditorOffsetPanel : Panel
{
    private FluXisTextBox textBox;

    public Action<float> OnApplyOffset { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 600;
        AutoSizeAxes = Axes.Y;
        Content.RelativeSizeAxes = Axes.X;
        Content.AutoSizeAxes = Axes.Y;

        Content.Child = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Spacing = new Vector2(5),
            Children = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = "Apply Offset to map",
                    FontSize = 30,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre
                },
                textBox = new FluXisTextBox
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 40,
                    PlaceholderText = "Offset in milliseconds...",
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre
                },
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.X,
                    Height = 40,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(10),
                    Margin = new MarginPadding { Top = 10 },
                    Children = new Drawable[]
                    {
                        new FluXisButton
                        {
                            Text = "Apply",
                            Width = 100,
                            RelativeSizeAxes = Axes.Y,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Action = () =>
                            {
                                if (!float.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var offset))
                                {
                                    textBox.NotifyError();
                                    return;
                                }

                                OnApplyOffset?.Invoke(offset);
                                Hide();
                            }
                        },
                        new FluXisButton
                        {
                            Text = "Cancel",
                            Width = 100,
                            RelativeSizeAxes = Axes.Y,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Action = Hide
                        }
                    }
                }
            }
        };
    }
}
