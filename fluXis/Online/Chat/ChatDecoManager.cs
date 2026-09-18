using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using fluXis.Online.Chat.Segments;
using fluXis.Online.Chat.Syntax;
using osu.Framework.Allocation;
using osu.Framework.Extensions.ListExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Framework.Lists;

namespace fluXis.Online.Chat;

#nullable enable

/// <summary>
/// Manages the parsing of chat message decorations like emojis, urls and mentions.
/// </summary>
public partial class ChatDecoManager : Component
{
    private readonly List<ISyntaxMatcher> matchers = [];
    private readonly Dictionary<char, ISyntaxMatcher> autocomplete = [];
    private readonly Regex regex;

    private readonly List<string> emojis = [];
    private readonly Dictionary<char, string> emojiIndexes = [];
    private readonly Dictionary<string, char> emojiIndexesReverse = [];

    public SlimReadOnlyListWrapper<string> Emojis => emojis.AsSlimReadOnly();
    public SlimReadOnlyDictionaryWrapper<char, string> EmojiIndexes => emojiIndexes.AsSlimReadOnly();
    public SlimReadOnlyDictionaryWrapper<string, char> EmojiIndexesReversed => emojiIndexesReverse.AsSlimReadOnly();

    public ChatDecoManager()
    {
        matchers.Add(new EmojiSyntaxMatcher());
        matchers.Add(new LinkSyntaxMatcher());
        matchers.Add(new MentionSyntaxMatcher());

        regex = new Regex(
            string.Join("|", matchers.Select(x => x.SegmentMatcher.ToString())),
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        var available = textures.GetAvailableResources();

        foreach (var s in available.Where(x => x.StartsWith("Emoji/")))
            emojis.Add(Path.GetFileNameWithoutExtension(s));

        for (var i = 0; i < emojis.Count; i++)
        {
            emojiIndexes[(char)i] = emojis[i];
            emojiIndexesReverse[emojis[i]] = (char)i;
        }
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

            var syn = matchers.First(x => x.SegmentMatcher.IsMatch(match.Value));
            segments.Add(syn.CreateSegment(this, match));

            last = match.Index + match.Length;
        }

        // add remaining text
        if (input.Length > last)
            segments.Add(new TextSegment(input[last..]));

        return segments;
    }

    public CompletionResult Autocomplete(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return CompletionResult.None;

        foreach (var matcher in matchers)
        {
            var rgx = matcher.AutocompleteRegex;
            if (rgx is null) continue;

            Match match = rgx.Match(text);
            if (!match.Success) continue;

            return new CompletionResult(true, matcher, match.Groups[1].Value);
        }

        return CompletionResult.None;
    }

    public record CompletionResult(bool Searching, ISyntaxMatcher? Matcher, string Query)
    {
        public static CompletionResult None => new(false, null, string.Empty);
    }
}
