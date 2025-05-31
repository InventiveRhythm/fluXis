using System;
using fluXis.Graphics.Sprites.Text;
using Markdig.Extensions.Tables;
using Markdig.Syntax;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers.Markdown;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Graphics.Containers.Markdown;

public partial class FluXisMarkdown : MarkdownContainer
{
    public Action<HeadingBlock> HeadingCreated;
    public Action<string> OnLinkClicked;

    public FluXisMarkdown()
    {
        LineSpacing = 0;
    }

    public override MarkdownTextFlowContainer CreateTextFlow() => new FluXisMarkdownTextFlow(this);

    protected override MarkdownHeading CreateHeading(HeadingBlock headingBlock)
    {
        var heading = base.CreateHeading(headingBlock);

        switch (headingBlock.Level)
        {
            case 1:
                heading.Margin = new MarginPadding { Bottom = 16 };
                break;

            case 2:
                heading.Margin = new MarginPadding { Vertical = 16 };
                break;
        }

        HeadingCreated?.Invoke(headingBlock);
        return heading;
    }

    protected override MarkdownParagraph CreateParagraph(ParagraphBlock paragraphBlock, int level)
    {
        var paragraph = base.CreateParagraph(paragraphBlock, level);
        paragraph.Margin = new MarginPadding { Bottom = 16 };
        return paragraph;
    }

    protected override MarkdownList CreateList(ListBlock listBlock) => new FluXisMarkdownList();
    protected override MarkdownTable CreateTable(Table table) => new FluXisMarkdownTable(table);
    protected override MarkdownCodeBlock CreateCodeBlock(CodeBlock codeBlock) => new FluXisMarkdownCodeBlock(this, codeBlock);

    public override SpriteText CreateSpriteText() => new FluXisSpriteText();
}
