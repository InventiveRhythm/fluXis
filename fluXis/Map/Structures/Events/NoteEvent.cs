using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Menus.Items;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Map.Structures.Events;

public class NoteEvent : IMapEvent, IWithContext
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("lane")]
    public int Lane { get; set; }

    [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Group { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }

    IEnumerable<Drawable> ITimedObject.CreateObjectOverlay(EditorDrawableObject obj)
    {
        var flow = (FillFlowContainer)ITimedObject.CreateDefaultOverlay(obj).First();
        flow.Add(ITimedObject.CreateSmallText(obj, () => Content));
        yield return flow;
    }

    IEnumerable<MenuItem> IWithContext.CreateContextItems(EditorMap map, EditorSnapProvider snaps)
    {
        yield return new MenuActionItem("Split by word", Phosphor.Bold.ArrowsSplit, () =>
        {
            var words = Content.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            map.Remove(this);

            for (var i = 0; i < words.Length; i++)
            {
                var word = words[i];

                map.Add(new NoteEvent
                {
                    Time = Time + snaps.CurrentStep * i,
                    Content = $"{(i == 0 ? "" : "+ ")}{word}",
                    Group = Group,
                    Lane = Lane
                });
            }
        });
    }
}
