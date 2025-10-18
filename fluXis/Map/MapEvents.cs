using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Scripting;
using fluXis.Scripting.Runners;
using fluXis.Utils;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace fluXis.Map;

public class MapEvents
{
    [JsonProperty("laneswitch")]
    public List<LaneSwitchEvent> LaneSwitchEvents { get; private set; } = new();

    [JsonProperty("flash")]
    public List<FlashEvent> FlashEvents { get; private set; } = new();

    [JsonProperty("colorfade")]
    public List<ColorFadeEvent> ColorFadeEvents { get; private set; } = new();

    [JsonProperty("pulse")]
    public List<PulseEvent> PulseEvents { get; private set; } = new();

    [JsonProperty("playfieldmove")]
    public List<PlayfieldMoveEvent> PlayfieldMoveEvents { get; private set; } = new();

    [JsonProperty("playfieldscale")]
    public List<PlayfieldScaleEvent> PlayfieldScaleEvents { get; private set; } = new();

    [JsonProperty("playfieldrotate")]
    public List<PlayfieldRotateEvent> PlayfieldRotateEvents { get; private set; } = new();

    [JsonProperty("hitease")]
    public List<HitObjectEaseEvent> HitObjectEaseEvents { get; private set; } = new();

    [JsonProperty("layerfade")]
    public List<LayerFadeEvent> LayerFadeEvents { get; private set; } = new();

    [JsonProperty("shake")]
    public List<ShakeEvent> ShakeEvents { get; private set; } = new();

    [JsonProperty("shader")]
    public List<ShaderEvent> ShaderEvents { get; private set; } = new();

    [JsonProperty("beatpulse")]
    public List<BeatPulseEvent> BeatPulseEvents { get; private set; } = new();

    [JsonProperty("scroll-multiply")]
    public List<ScrollMultiplierEvent> ScrollMultiplyEvents { get; private set; } = new();

    [JsonProperty("time-offset")]
    public List<TimeOffsetEvent> TimeOffsetEvents { get; private set; } = new();

    [JsonProperty("scripts")]
    public List<ScriptEvent> ScriptEvents { get; private set; } = new();

    [JsonProperty("notes")]
    public List<NoteEvent> NoteEvents { get; private set; } = new();

    #region Old Prop Names

    [Obsolete($"Use {nameof(LayerFadeEvents)} instead."), JsonProperty("hitfade")]
    public List<LayerFadeEvent> LegacyHitObjectFade { set => LayerFadeEvents = LayerFadeEvents.Concat(value).ToList(); }

    [Obsolete($"Use {nameof(LayerFadeEvents)} instead."), JsonProperty("playfieldfade")]
    public List<LayerFadeEvent> LegacyPlayfieldFade
    {
        set
        {
            value.ForEach(v => v.Layer = LayerFadeEvent.FadeLayer.Playfield);
            LayerFadeEvents = LayerFadeEvents.Concat(value).ToList();
        }
    }

    #endregion

    [JsonIgnore]
    public bool Empty
    {
        get
        {
            var count = 0;
            ForAllEvents(_ => count++);
            return count == 0;
        }
    }

    #region Server-Side Stuff

    [JsonIgnore]
    public string RawContent { get; set; } = "";

    #endregion

    public static T Load<T>(string content)
        where T : MapEvents, new()
    {
        if (!content.Trim().StartsWith('{'))
            return new T().loadLegacy(content) as T;

        var events = content.Deserialize<T>();
        events.RawContent = content;
        return events.Sort() as T;
    }

    private MapEvents loadLegacy(string content)
    {
        var lines = content.Split(Environment.NewLine);

        foreach (var line in lines)
        {
            int index = line.IndexOf('(');
            int index2 = line.IndexOf(')');

            if (index == -1 || index2 == -1)
                continue;

            var type = line[..index];
            var args = line[(index + 1)..index2].Split(',');

            switch (type)
            {
                case "LaneSwitch":
                {
                    var laneSwitch = new LaneSwitchEvent
                    {
                        Time = float.Parse(args[0], CultureInfo.InvariantCulture),
                        Count = int.Parse(args[1])
                    };

                    if (args.Length > 2)
                        laneSwitch.Duration = float.Parse(args[2], CultureInfo.InvariantCulture);

                    LaneSwitchEvents.Add(laneSwitch);
                    break;
                }

                case "Flash":
                    if (args.Length < 8)
                        continue;

                    float duration = float.Parse(args[1], CultureInfo.InvariantCulture);
                    bool inBackground = args[2] == "true";
                    Easing easing = (Easing)Enum.Parse(typeof(Easing), args[3]);
                    Colour4 startColor = Colour4.FromHex(args[4]);
                    float startOpacity = float.Parse(args[5], CultureInfo.InvariantCulture);
                    Colour4 endColor = Colour4.FromHex(args[6]);
                    float endOpacity = float.Parse(args[7], CultureInfo.InvariantCulture);

                    FlashEvents.Add(new FlashEvent
                    {
                        Time = float.Parse(args[0], CultureInfo.InvariantCulture),
                        Duration = duration,
                        InBackground = inBackground,
                        Easing = easing,
                        StartColor = startColor,
                        StartOpacity = startOpacity,
                        EndColor = endColor,
                        EndOpacity = endOpacity
                    });
                    break;

                case "Pulse":
                    PulseEvents.Add(new PulseEvent
                    {
                        Time = float.Parse(args[0], CultureInfo.InvariantCulture)
                    });
                    break;

                case "PlayfieldMove":
                    if (args.Length < 4)
                        continue;

                    PlayfieldMoveEvents.Add(new PlayfieldMoveEvent
                    {
                        Time = float.Parse(args[0], CultureInfo.InvariantCulture),
                        OffsetX = float.Parse(args[1], CultureInfo.InvariantCulture),
                        Duration = float.Parse(args[2], CultureInfo.InvariantCulture),
                        Easing = Enum.TryParse<Easing>(args[3], out var ease) ? ease : Easing.None
                    });
                    break;

                case "PlayfieldScale":
                    if (args.Length < 5)
                        continue;

                    PlayfieldScaleEvents.Add(new PlayfieldScaleEvent
                    {
                        Time = float.Parse(args[0], CultureInfo.InvariantCulture),
                        ScaleX = float.Parse(args[1], CultureInfo.InvariantCulture),
                        ScaleY = float.Parse(args[2], CultureInfo.InvariantCulture),
                        Duration = float.Parse(args[3], CultureInfo.InvariantCulture),
                        Easing = Enum.TryParse<Easing>(args[4], out var ease2) ? ease2 : Easing.None
                    });
                    break;

                case "Shake":
                    if (args.Length < 3)
                        continue;

                    ShakeEvents.Add(new ShakeEvent
                    {
                        Time = float.Parse(args[0], CultureInfo.InvariantCulture),
                        Duration = float.Parse(args[1], CultureInfo.InvariantCulture),
                        Magnitude = float.Parse(args[2], CultureInfo.InvariantCulture)
                    });
                    break;

                case "PlayfieldFade":
                    if (args.Length < 3)
                        continue;

                    LayerFadeEvents.Add(new LayerFadeEvent()
                    {
                        Time = float.Parse(args[0], CultureInfo.InvariantCulture),
                        Duration = float.Parse(args[1], CultureInfo.InvariantCulture),
                        Alpha = float.Parse(args[2], CultureInfo.InvariantCulture),
                        Layer = LayerFadeEvent.FadeLayer.Playfield
                    });
                    break;

                case "Shader":
                    if (args.Length < 3)
                        continue;

                    var startIdx = line.IndexOf('{');
                    var endIdx = line.LastIndexOf('}');

                    if (startIdx == -1 || endIdx == -1)
                        continue;

                    var dataJson = line[startIdx..(endIdx + 1)];
                    var data = dataJson.Deserialize<ShaderEvent.ShaderParameters>();

                    ShaderEvents.Add(new ShaderEvent
                    {
                        Time = float.Parse(args[0], CultureInfo.InvariantCulture),
                        ShaderName = args[1],
                        EndParameters = data
                    });
                    break;
            }
        }

        return Sort();
    }

    [JsonIgnore]
    public IEnumerable<PropertyInfo> AllListProperties
    {
        get
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.GetField);

            foreach (var prop in properties)
            {
                var type = prop.PropertyType;

                if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(List<>))
                    continue;

                var obsolete = prop.GetCustomAttribute<ObsoleteAttribute>();
                if (obsolete is not null) continue;

                var itemType = prop.PropertyType.GetGenericArguments()[0];

                if (!typeof(ITimedObject).IsAssignableFrom(itemType))
                    continue;

                yield return prop;
            }
        }
    }

    public void ForAllEvents(Action<ITimedObject> action)
    {
        foreach (var prop in AllListProperties)
        {
            if (prop.GetValue(this) is not IEnumerable list)
                continue;

            foreach (var o in list)
            {
                var obj = o as ITimedObject;
                action?.Invoke(obj);
            }
        }
    }

    public MapEvents Sort()
    {
        foreach (var prop in AllListProperties)
        {
            var itemType = prop.PropertyType.GetGenericArguments()[0];

            var list = prop.GetValue(this);
            if (list is null) continue;

            var compare = typeof(MapEvents).GetMethod(nameof(MapEvents.compare), BindingFlags.Static | BindingFlags.NonPublic);
            var comparison = Delegate.CreateDelegate(typeof(Comparison<>).MakeGenericType(itemType), compare);

            var sort = typeof(List<>).MakeGenericType(itemType).GetMethod("Sort", new[] { comparison.GetType() });
            sort?.Invoke(list, new object[] { comparison });
        }

        return this;
    }

    public void RunScripts(ScriptStorage storage)
    {
        foreach (var ev in ScriptEvents)
        {
            try
            {
                var runner = storage.GetRunner(ev.ScriptPath, s => new EffectScriptRunner(s)
                {
                    AddFlash = FlashEvents.Add
                }) ?? throw new Exception("Could not create script runner.");

                runner.Handle(ev);
            }
            catch (Exception ex)
            {
                ScriptRunner.Logger.Add($"Failed to run script at {ev.Time}: {ex.Message}", LogLevel.Error);
            }
        }

        Sort();
    }

    private static int compare(ITimedObject a, ITimedObject b)
    {
        var val = a.Time.CompareTo(b.Time);
        return val == 0 ? a.GetHashCode().CompareTo(b.GetHashCode()) : val;
    }

    public string Save() => Sort().Serialize();
}
