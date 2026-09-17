using System;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Online.Chat;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Overlay.Network.Tabs.Chat;

#nullable enable

public partial class ChatInputBar : CompositeDrawable
{
    [Resolved]
    private ChatDecoManager decorations { get; set; } = null!;

    public Func<string, bool>? OnConfirm { get; init; }

    private readonly Container completionsWrapper;
    private readonly FluXisMenu completions;
    private readonly FluXisTextBox textBox;
    private int lastCursorPosition = -1;

    public ChatInputBar()
    {
        RelativeSizeAxes = Axes.X;
        Height = 48;

        InternalChildren =
        [
            completionsWrapper = new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                CornerRadius = 8,
                Masking = true,
                Children =
                [
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Theme.Background4
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Padding = new MarginPadding { Bottom = 48 },
                        Child = completions = new FluXisMenu(Direction.Vertical, true)
                        {
                            RelativeSizeAxes = Axes.X,
                            MaskingRadius = 8,
                            MaxHeight = 440
                        }
                    }
                ]
            },
            textBox = new FluXisTextBox
            {
                BackgroundActive = Theme.Background3,
                BackgroundInactive = Theme.Background3,
                PlaceholderText = "Type your message here...",
                RelativeSizeAxes = Axes.X,
                Height = 48,
                SidePadding = 14,
                CornerRadius = 8,
                FontSize = FluXisSpriteText.GetWebFontSize(16),
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft
            }
        ];
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        textBox.OnCommitAction += () =>
        {
            if (OnConfirm is null)
                return;

            var result = OnConfirm.Invoke(textBox.Text);
            if (result) textBox.Text = "";
            GetContainingFocusManager()?.ChangeFocus(textBox);
        };
    }

    protected override void Update()
    {
        base.Update();

        if (lastCursorPosition != textBox.SelectionStart)
            updateCompletions();

        lastCursorPosition = textBox.SelectionStart;
    }

    private void updateCompletions()
    {
        var text = textBox.Text[..textBox.SelectionStart];
        var completion = decorations.Autocomplete(text);

        if (!completion.Searching)
        {
            completions.Items = [];
            return;
        }

        var results = completion.Matcher!.AutocompleteSearch(decorations, completion.Query).ToArray();
        results.ForEach(x => x.Action.Value = () =>
        {
            var name = x.Text.Value.ToString();
            var rest = name[completion.Query.Length..];
            textBox.AddAfterCursor($"{rest}: ");
        });
        completions.Items = [.. results];
    }
}
