using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using fluXis.Online.Chat.Deco;
using Midori.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;

namespace fluXis.Online.Chat;

/// <summary>
/// Manages the parsing of chat message decorations like emojis, urls and mentions.
/// </summary>
public partial class ChatDecoManager : Component
{
    private readonly Regex regex;
    private readonly List<string> emojis = [];

    public ChatDecoManager()
    {
        var str = "(?<Emoji>:[a-zA-Z0-9_]+:)";
        str += @"|(?<Mention>\<@[0-9]+\>)";
        str += @"|(?<Link>https?://[^\s/$.?#].[^\s]*)";

        regex = new Regex(str, RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        var available = textures.GetAvailableResources();

        foreach (var s in available.Where(x => x.StartsWith("Emoji/")))
            emojis.Add(Path.GetFileNameWithoutExtension(s));
    }

    public List<IMessageSegment> ParseSegments(string input)
    {
        var segments = new List<IMessageSegment>();
        if (string.IsNullOrWhiteSpace(input)) return segments;

        var last = 0;

        foreach (Match match in regex.Matches(input))
        {
            // add any text before match
            if (match.Index > last)
                segments.Add(new TextSegment(input[last..match.Index]));

            if (match.Groups["Emoji"].Success)
            {
                var trim = match.Value.Trim(':');

                // return raw text if emoji doesn't exist
                if (!emojis.Contains(trim)) segments.Add(new TextSegment(match.Value));
                else segments.Add(new EmojiSegment(trim));
            }
            else if (match.Groups["Mention"].Success)
            {
                var trim = match.Value[2..^1];
                if (trim.TryParseLongInvariant(out var id))
                    segments.Add(new MentionSegment(id));
                else
                    segments.Add(new TextSegment(match.Value));
            }
            else if (match.Groups["Link"].Success)
                segments.Add(new LinkSegment(match.Value));

            last = match.Index + match.Length;
        }

        // add remaining text
        if (input.Length > last)
            segments.Add(new TextSegment(input[last..]));

        return segments;
    }
}
