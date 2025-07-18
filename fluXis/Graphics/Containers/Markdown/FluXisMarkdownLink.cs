using System;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using JetBrains.Annotations;
using Markdig.Syntax.Inlines;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers.Markdown;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Graphics.Containers.Markdown;

public partial class FluXisMarkdownLink : MarkdownLinkText, IHasTooltip
{
    public new LocalisableString TooltipText => Url.StartsWith("/wiki") ? Url[5..] : Url;

    public Action<string> OnLinkClicked;

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private FluXisGame game { get; set; }

    public FluXisMarkdownLink([NotNull] string text, [NotNull] LinkInline linkInline)
        : base(text, linkInline)
    {
    }

    public FluXisMarkdownLink([NotNull] AutolinkInline link)
        : base(link)
    {
    }

    protected override void OnLinkPressed()
    {
        if (Url.StartsWith('/'))
            OnLinkClicked?.Invoke(Url);
        else if (game is not null)
            game.OpenLink(Url);
        else
            base.OnLinkPressed();
    }

    public override SpriteText CreateSpriteText() => new FluXisSpriteText
    {
        WebFontSize = 14,
        Colour = Theme.Highlight
    };
}
