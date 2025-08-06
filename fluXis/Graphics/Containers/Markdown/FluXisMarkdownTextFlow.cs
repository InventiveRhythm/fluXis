using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using Markdig.Syntax.Inlines;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Containers.Markdown;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Graphics.Containers.Markdown;

public partial class FluXisMarkdownTextFlow : MarkdownTextFlowContainer
{
    public FluXisFont Font { get; init; } = FluXisFont.RenogareSoft;
    private FluXisMarkdown markdown { get; }

    public FluXisMarkdownTextFlow(FluXisMarkdown markdown)
    {
        this.markdown = markdown;
    }

    protected override SpriteText CreateSpriteText()
    {
        var text = base.CreateSpriteText();

        if (text is FluXisSpriteText flx)
            flx.Font = Font;

        return text;
    }

    protected override void AddLinkText(string text, LinkInline link)
        => AddDrawable(new FluXisMarkdownLink(text, link) { OnLinkClicked = markdown.OnLinkClicked });

    protected override void AddAutoLink(AutolinkInline link)
        => AddDrawable(new FluXisMarkdownLink(link) { OnLinkClicked = markdown.OnLinkClicked });

    protected override void AddCodeInLine(CodeInline codeInline)
    {
        var code = new Container
        {
            AutoSizeAxes = Axes.X,
            Height = 20,
            Children = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 28,
                    CornerRadius = 6,
                    Masking = true,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Theme.Background1,
                    }
                },
                new FluXisSpriteText
                {
                    WebFontSize = 14,
                    Text = codeInline.Content,
                    Margin = new MarginPadding { Horizontal = 8 },
                    Font = FluXisFont.JetBrainsMono
                }
            }
        };

        AddDrawable(code);
    }
}
