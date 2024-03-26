using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Edit;

public partial class EditorDifficultyCreationPanel : Panel
{
    private FluXisTextBox textBox;

    public Action<string> OnCreateNewDifficulty { get; set; }
    public Action<string> OnCopyDifficulty { get; set; }

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
                    Text = "Create New Difficulty",
                    FontSize = 30,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre
                },
                textBox = new FluXisTextBox
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 40,
                    PlaceholderText = "New difficulty name...",
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
                            Text = "New",
                            TooltipText = "Create a new difficulty from scratch with only metadata and timing points.",
                            Action = () =>
                            {
                                if (string.IsNullOrEmpty(textBox.Text))
                                {
                                    textBox.NotifyError();
                                    return;
                                }

                                OnCreateNewDifficulty?.Invoke(textBox.Text);
                            },
                            Width = 100,
                            RelativeSizeAxes = Axes.Y,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre
                        },
                        new FluXisButton
                        {
                            Text = "Copy",
                            TooltipText = "Create a new difficulty by copying the map data from an existing difficulty.",
                            Action = () =>
                            {
                                if (string.IsNullOrEmpty(textBox.Text))
                                {
                                    textBox.NotifyError();
                                    return;
                                }

                                OnCopyDifficulty?.Invoke(textBox.Text);
                            },
                            Width = 100,
                            RelativeSizeAxes = Axes.Y,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre
                        },
                        new FluXisButton
                        {
                            Text = "Cancel",
                            Action = Hide,
                            Width = 100,
                            RelativeSizeAxes = Axes.Y,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre
                        }
                    }
                }
            }
        };
    }
}
