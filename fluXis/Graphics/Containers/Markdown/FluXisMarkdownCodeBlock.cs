using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using JetBrains.Annotations;
using Markdig.Syntax;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Containers.Markdown;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Graphics.Containers.Markdown;

public partial class FluXisMarkdownCodeBlock : MarkdownCodeBlock
{
    private FluXisMarkdown markdown { get; }
    private CodeBlock codeBlock;
    private FluXisMarkdownCopyButton copyButton;

    public FluXisMarkdownCodeBlock(FluXisMarkdown markdown, [NotNull] CodeBlock codeBlock)
        : base(codeBlock)
    {
        this.markdown = markdown;
        this.codeBlock = codeBlock;
        Margin = new MarginPadding { Bottom = 16 };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        AddInternal
        (
            copyButton = new FluXisMarkdownCopyButton(codeBlock)
            {
                Alpha = 0,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
            }
        );

        Scheduler.AddDelayed(() =>
        {
            copyButton.Anchor = Anchor.TopRight;
            copyButton.Origin = Anchor.TopRight;
            copyButton.FadeTo(1f, 300, Easing.In);
        }, 100);
    }

    protected override Drawable CreateBackground()
    {
        var container = new Container
        {
            RelativeSizeAxes = Axes.Both,
            CornerRadius = 8,
            Masking = true,
            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background1
            }
        };

        return container;
    }

    public override MarkdownTextFlowContainer CreateTextFlow()
    {
        var flow = new FluXisMarkdownTextFlow(markdown)
        {
            Font = FluXisFont.JetBrainsMono,
            Margin = new MarginPadding(16)
        };

        return flow;
    }
}
