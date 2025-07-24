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

    public FluXisMarkdownCodeBlock(FluXisMarkdown markdown, [NotNull] CodeBlock codeBlock)
        : base(codeBlock)
    {
        this.markdown = markdown;
        Margin = new MarginPadding { Bottom = 16 };
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
