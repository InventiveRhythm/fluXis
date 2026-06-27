namespace fluXis.Integration;

public class DiscordRichPresence
{
    public string State { get; set; } = "";
    public string Details { get; set; } = "";

    public ActivityTypes ActivityType { get; set; } = ActivityTypes.Playing;
    public StatusDisplayTypes DisplayType { get; set; } = StatusDisplayTypes.Name;

    public string LargeImage { get; set; } = "";
    public string LargeImageText { get; set; } = "";

    public string SmallImage { get; set; } = "";
    public string SmallImageText { get; set; } = "";

    public long PartyID { get; set; }
    public string PartySecret { get; set; }
    public int PartySize { get; set; }
    public int PartyMax { get; set; }

    public ulong StartTime { get; set; }
    public ulong EndTime { get; set; }

    public Button[] Buttons { get; set; }

    public class Button
    {
        public string Label { get; }
        public string Url { get; }

        public Button(string label, string url)
        {
            Label = label;
            Url = url;
        }
    }

    public enum StatusDisplayTypes
    {
        Name = 0,
        State = 1,
        Details = 2,
    }

    public enum ActivityTypes
    {
        Playing = 0,
        Streaming = 1,
        Listening = 2,
        Watching = 3,
        CustomStatus = 4,
        Competing = 5,
        HangStatus = 6,
    }
}
