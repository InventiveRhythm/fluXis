using System.Collections.Generic;
using Newtonsoft.Json;
using osu.Framework.Input.Bindings;

namespace fluXis.Screens.Edit.Input;

public class EditorKeymap
{
    [JsonProperty("invert_scroll")]
    public bool InvertScroll { get; set; }

    [JsonProperty("scroll")]
    public Scrolling Scroll { get; set; } = new();

    [JsonProperty("binds")]
    public Dictionary<EditorKeybinding, InputKey[]> Bindings { get; set; } = new();

    public class Scrolling
    {
        [JsonProperty("normal")]
        public EditorScrollAction Normal { get; set; } = EditorScrollAction.Seek;

        [JsonProperty("shift")]
        public EditorScrollAction Shift { get; set; } = EditorScrollAction.Snap;

        [JsonProperty("ctrl")]
        public EditorScrollAction Control { get; set; } = EditorScrollAction.Zoom;

        [JsonProperty("ctrl-shift")]
        public EditorScrollAction ControlShift { get; set; } = EditorScrollAction.Rate;
    }
}
