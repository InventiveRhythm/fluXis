using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Database;
using fluXis.Database.Maps;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Presets;
using fluXis.Map;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Verify;
using fluXis.Storyboards;
using fluXis.Utils;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using SixLabors.ImageSharp;

namespace fluXis.Screens.Edit;

public class EditorMap : IVerifyContext
{
    public EditorMapInfo MapInfo { get; set; }
    public RealmMap RealmMap { get; set; }

    public MapEvents MapEvents => MapInfo.MapEvents;
    public Storyboard Storyboard => MapInfo.Storyboard;
    public RealmMapSet MapSet => RealmMap?.MapSet;

    private readonly Action<Drawable> loadComponent;

    public string MapInfoHash => MapUtils.GetHash(MapInfo.Serialize());
    public string MapEventsHash => MapUtils.GetHash(MapEvents.Save());
    public string StoryboardHash => MapUtils.GetHash(Storyboard.Serialize());

    public bool IsNew => RealmMap == null || MapInfo == null;

    public PanelContainer Panels { get; set; }

    private List<IChangeNotifier> notifiers = new();

    public EditorMap(EditorMapInfo info, RealmMap map, Action<Drawable> loadComponent)
    {
        MapInfo = info;
        RealmMap = map;
        this.loadComponent = loadComponent;
    }

    #region Events

    public event Action<int> KeyModeChanged;

    public event Action AudioChanged;
    public event Action BackgroundChanged;
    public event Action CoverChanged;

#nullable enable
    public event Action<ITimedObject?>? AnyChange;
#nullable disable

    public event Action<HitObject> HitObjectAdded;
    public event Action<HitObject> HitObjectRemoved;
    public event Action<HitObject> HitObjectUpdated;
    public event Action HitSoundsChanged;

    public event Action<TimingPoint> TimingPointAdded;
    public event Action<TimingPoint> TimingPointRemoved;
    public event Action<TimingPoint> TimingPointUpdated;

    public event Action<ScrollVelocity> ScrollVelocityAdded;
    public event Action<ScrollVelocity> ScrollVelocityRemoved;
    public event Action<ScrollVelocity> ScrollVelocityUpdated;

    public event Action<LaneSwitchEvent> LaneSwitchEventAdded;
    public event Action<LaneSwitchEvent> LaneSwitchEventRemoved;
    public event Action<LaneSwitchEvent> LaneSwitchEventUpdated;

    public event Action<FlashEvent> FlashEventAdded;
    public event Action<FlashEvent> FlashEventRemoved;
    public event Action<FlashEvent> FlashEventUpdated;

    public event Action<PulseEvent> PulseEventAdded;
    public event Action<PulseEvent> PulseEventRemoved;
    public event Action<PulseEvent> PulseEventUpdated;

    public event Action<ShakeEvent> ShakeEventAdded;
    public event Action<ShakeEvent> ShakeEventRemoved;
    public event Action<ShakeEvent> ShakeEventUpdated;

    public event Action<PlayfieldMoveEvent> PlayfieldMoveEventAdded;
    public event Action<PlayfieldMoveEvent> PlayfieldMoveEventRemoved;
    public event Action<PlayfieldMoveEvent> PlayfieldMoveEventUpdated;

    public event Action<PlayfieldScaleEvent> PlayfieldScaleEventAdded;
    public event Action<PlayfieldScaleEvent> PlayfieldScaleEventRemoved;
    public event Action<PlayfieldScaleEvent> PlayfieldScaleEventUpdated;

    public event Action<HitObjectEaseEvent> HitObjectEaseEventAdded;
    public event Action<HitObjectEaseEvent> HitObjectEaseEventRemoved;
    public event Action<HitObjectEaseEvent> HitObjectEaseEventUpdated;

    public event Action<LayerFadeEvent> LayerFadeEventAdded;
    public event Action<LayerFadeEvent> LayerFadeEventRemoved;
    public event Action<LayerFadeEvent> LayerFadeEventUpdated;

    public event Action<ShaderEvent> ShaderEventAdded;
    public event Action<ShaderEvent> ShaderEventRemoved;
    public event Action<ShaderEvent> ShaderEventUpdated;

    public event Action<BeatPulseEvent> BeatPulseEventAdded;
    public event Action<BeatPulseEvent> BeatPulseEventRemoved;
    public event Action<BeatPulseEvent> BeatPulseEventUpdated;

    public event Action<ScrollMultiplierEvent> ScrollMultiplierEventAdded;
    public event Action<ScrollMultiplierEvent> ScrollMultiplierEventRemoved;
    public event Action<ScrollMultiplierEvent> ScrollMultiplierEventUpdated;

    public event Action<PlayfieldRotateEvent> PlayfieldRotateEventAdded;
    public event Action<PlayfieldRotateEvent> PlayfieldRotateEventRemoved;
    public event Action<PlayfieldRotateEvent> PlayfieldRotateEventUpdated;

    public event Action<TimeOffsetEvent> TimeOffsetEventAdded;
    public event Action<TimeOffsetEvent> TimeOffsetEventRemoved;
    public event Action<TimeOffsetEvent> TimeOffsetEventUpdated;

    public event Action<ScriptEvent> ScriptEventAdded;
    public event Action<ScriptEvent> ScriptEventRemoved;
    public event Action<ScriptEvent> ScriptEventUpdated;

    public event Action<NoteEvent> NoteEventAdded;
    public event Action<NoteEvent> NoteEventRemoved;
    public event Action<NoteEvent> NoteEventUpdated;

    #endregion

    public void LoadComponent(Drawable drawable) => loadComponent.Invoke(drawable);

    public void SetupNotifiers()
    {
        notifiers = new List<IChangeNotifier>
        {
            new ChangeNotifier<HitObject>(MapInfo.HitObjects, obj => HitObjectAdded?.Invoke(obj), obj => HitObjectRemoved?.Invoke(obj), obj => HitObjectUpdated?.Invoke(obj)),
            new ChangeNotifier<TimingPoint>(MapInfo.TimingPoints, obj => TimingPointAdded?.Invoke(obj), obj => TimingPointRemoved?.Invoke(obj), obj => TimingPointUpdated?.Invoke(obj)),
            new ChangeNotifier<ScrollVelocity>(MapInfo.ScrollVelocities, obj => ScrollVelocityAdded?.Invoke(obj), obj => ScrollVelocityRemoved?.Invoke(obj), obj => ScrollVelocityUpdated?.Invoke(obj)),
            new ChangeNotifier<LaneSwitchEvent>(MapEvents.LaneSwitchEvents, obj => LaneSwitchEventAdded?.Invoke(obj), obj => LaneSwitchEventRemoved?.Invoke(obj),
                obj => LaneSwitchEventUpdated?.Invoke(obj)),
            new ChangeNotifier<FlashEvent>(MapEvents.FlashEvents, obj => FlashEventAdded?.Invoke(obj), obj => FlashEventRemoved?.Invoke(obj), obj => FlashEventUpdated?.Invoke(obj)),
            new ChangeNotifier<PulseEvent>(MapEvents.PulseEvents, obj => PulseEventAdded?.Invoke(obj), obj => PulseEventRemoved?.Invoke(obj), obj => PulseEventUpdated?.Invoke(obj)),
            new ChangeNotifier<PlayfieldMoveEvent>(MapEvents.PlayfieldMoveEvents, obj => PlayfieldMoveEventAdded?.Invoke(obj), obj => PlayfieldMoveEventRemoved?.Invoke(obj),
                obj => PlayfieldMoveEventUpdated?.Invoke(obj)),
            new ChangeNotifier<PlayfieldScaleEvent>(MapEvents.PlayfieldScaleEvents, obj => PlayfieldScaleEventAdded?.Invoke(obj), obj => PlayfieldScaleEventRemoved?.Invoke(obj),
                obj => PlayfieldScaleEventUpdated?.Invoke(obj)),
            new ChangeNotifier<PlayfieldRotateEvent>(MapEvents.PlayfieldRotateEvents, obj => PlayfieldRotateEventAdded?.Invoke(obj), obj => PlayfieldRotateEventRemoved?.Invoke(obj),
                obj => PlayfieldRotateEventUpdated?.Invoke(obj)),
            new ChangeNotifier<LayerFadeEvent>(MapEvents.LayerFadeEvents, obj => LayerFadeEventAdded?.Invoke(obj), obj => LayerFadeEventRemoved?.Invoke(obj),
                obj => LayerFadeEventUpdated?.Invoke(obj)),
            new ChangeNotifier<HitObjectEaseEvent>(MapEvents.HitObjectEaseEvents, obj => HitObjectEaseEventAdded?.Invoke(obj), obj => HitObjectEaseEventRemoved?.Invoke(obj),
                obj => HitObjectEaseEventUpdated?.Invoke(obj)),
            new ChangeNotifier<ShakeEvent>(MapEvents.ShakeEvents, obj => ShakeEventAdded?.Invoke(obj), obj => ShakeEventRemoved?.Invoke(obj), obj => ShakeEventUpdated?.Invoke(obj)),
            new ChangeNotifier<ShaderEvent>(MapEvents.ShaderEvents, obj => ShaderEventAdded?.Invoke(obj), obj => ShaderEventRemoved?.Invoke(obj), obj => ShaderEventUpdated?.Invoke(obj)),
            new ChangeNotifier<BeatPulseEvent>(MapEvents.BeatPulseEvents, obj => BeatPulseEventAdded?.Invoke(obj), obj => BeatPulseEventRemoved?.Invoke(obj),
                obj => BeatPulseEventUpdated?.Invoke(obj)),
            new ChangeNotifier<ScrollMultiplierEvent>(MapEvents.ScrollMultiplyEvents, obj => ScrollMultiplierEventAdded?.Invoke(obj), obj => ScrollMultiplierEventRemoved?.Invoke(obj),
                obj => ScrollMultiplierEventUpdated?.Invoke(obj)),
            new ChangeNotifier<TimeOffsetEvent>(MapEvents.TimeOffsetEvents, obj => TimeOffsetEventAdded?.Invoke(obj), obj => TimeOffsetEventRemoved?.Invoke(obj),
                obj => TimeOffsetEventUpdated?.Invoke(obj)),
            new ChangeNotifier<ScriptEvent>(MapEvents.ScriptEvents, obj => ScriptEventAdded?.Invoke(obj), obj => ScriptEventRemoved?.Invoke(obj), obj => ScriptEventUpdated?.Invoke(obj)),
            new ChangeNotifier<NoteEvent>(MapEvents.NoteEvents, obj => NoteEventAdded?.Invoke(obj), obj => NoteEventRemoved?.Invoke(obj), obj => NoteEventUpdated?.Invoke(obj)),
            MapInfo.Storyboard
        };

        foreach (var notifier in notifiers)
        {
            notifier.OnAdd += t => AnyChange?.Invoke(t);
            notifier.OnRemove += t => AnyChange?.Invoke(t);
            notifier.OnUpdate += t => AnyChange?.Invoke(t);
        }
    }

    public bool SetKeyMode(int mode)
    {
        if (!CanChangeTo(mode))
            return false;

        RealmMap.KeyCount = mode;
        KeyModeChanged?.Invoke(mode);
        AnyChange?.Invoke(null);
        return true;
    }

    public bool CanChangeTo(int mode)
    {
        var highestLane = MapInfo.HitObjects.MaxBy(o => o.Lane)?.Lane ?? 0;
        highestLane = Math.Max(highestLane, MapEvents.LaneSwitchEvents.MaxBy(o => o.Count)?.Count ?? 0);
        return highestLane <= mode;
    }

    #region Assets

    public void SetAudio(FileInfo file)
    {
        if (file == null || !copyFile(file))
            return;

        MapInfo.AudioFile = file.Name;
        RealmMap.Metadata.Audio = file.Name;
        AudioChanged?.Invoke();
    }

    public void SetBackground(FileInfo file)
    {
        if (file == null || !copyFile(file))
            return;

        MapInfo.BackgroundFile = file.Name;
        RealmMap.Metadata.Background = file.Name;
        BackgroundChanged?.Invoke();

        // update accent color
        using var stream = RealmMap.GetBackgroundStream();
        var color = ImageUtils.GetAverageColour(stream);

        if (color != Colour4.Transparent)
            RealmMap.Metadata.Color = MapInfo.Colors.Accent = color;
        else
            RealmMap.Metadata.ColorHex = MapInfo.Colors.AccentHex = string.Empty;
    }

    public void SetCover(FileInfo file)
    {
        if (file == null || !copyFile(file))
            return;

        RealmMap.MapSet.Cover = file.Name;
        MapInfo.CoverFile = file.Name;
        CoverChanged?.Invoke();
    }

    public void SetVideo(FileInfo file)
    {
        if (file == null || !copyFile(file))
            return;

        MapInfo.VideoFile = file.Name;
    }

    private bool copyFile(FileInfo file)
    {
        try
        {
            var mapDir = new DirectoryInfo(MapFiles.GetFullPath(MapSet.ID.ToString()));

            if (file.Directory != null && file.Directory.FullName == mapDir.FullName)
                return true;

            string path = MapFiles.GetFullPath(MapSet.GetPathForFile(file.Name));
            var dir = Path.GetDirectoryName(path);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.Copy(file.FullName, path, true);
            return true;
        }
        catch (Exception ex)
        {
            Panels.Content = new ExceptionPanel(ex);
            return false;
        }
    }

    #endregion

    #region Objects

    private bool tryFindNotifier(ITimedObject obj, Action<IChangeNotifier> action)
    {
        var n = notifiers.FirstOrDefault(n => n.Matches(obj.GetType()));

        if (n is not null)
            action?.Invoke(n);

        return n != null;
    }

    public void Add(ITimedObject obj)
    {
        if (tryFindNotifier(obj, n => n.Add(obj)))
            return;

        throwMissingHandler(obj);
    }

    public void Update(ITimedObject obj)
    {
        if (tryFindNotifier(obj, n => n.Update(obj)))
            return;

        throwMissingHandler(obj);
    }

    public void UpdateHitSounds() => HitSoundsChanged?.Invoke();

    public void Remove(ITimedObject obj)
    {
        if (tryFindNotifier(obj, n => n.Remove(obj)))
            return;

        throwMissingHandler(obj);
    }

    private void throwMissingHandler(ITimedObject obj) => throw new ArgumentException($"Type '{obj.GetType().Name}' does not have a change handler associated with it.");

    public void ApplyOffsetToAll(float offset) => notifiers.ForEach(n => n.ApplyOffset(offset));

    public void Sort()
    {
        MapInfo.Sort();
        MapEvents.Sort();
    }

    #endregion

    #region IVerifyContext Implementation

    MapInfo IVerifyContext.MapInfo => MapInfo;
    MapEvents IVerifyContext.MapEvents => MapEvents;
    RealmMap IVerifyContext.RealmMap => RealmMap;

    #endregion

    public class EditorMapInfo : MapInfo, IDeepCloneable<EditorMapInfo>
    {
        [JsonIgnore]
        public MapEvents MapEvents { get; set; }

        [JsonIgnore]
        public Storyboard Storyboard { get; set; }

        public EditorMapInfo(MapMetadata metadata)
            : base(metadata)
        {
        }

        public EditorMapInfo() { }

        public override T GetMapEvents<T>() => MapEvents as T;

        public override Storyboard GetStoryboard() => new()
        {
            Resolution = Storyboard.Resolution,
            Elements = Storyboard.Elements.ToList()
        };

        public EditorMapInfo DeepClone()
        {
            var clone = this.JsonCopy();
            clone.MapEvents = MapEvents.JsonCopy();
            clone.Storyboard = Storyboard.JsonCopy();
            return clone;
        }
    }

    public interface IChangeNotifier
    {
        event Action<ITimedObject> OnAdd;
        event Action<ITimedObject> OnRemove;
        event Action<ITimedObject> OnUpdate;

        void Add(ITimedObject obj);
        void Remove(ITimedObject obj);
        void Update(ITimedObject obj);

        void ApplyOffset(float offset);

        bool Matches(Type type);
    }

    private class ChangeNotifier<T> : IChangeNotifier
        where T : class, ITimedObject
    {
        private List<T> list { get; }

        private Action<T> add { get; }
        private Action<T> remove { get; }
        private Action<T> update { get; }

        public event Action<ITimedObject> OnAdd;
        public event Action<ITimedObject> OnRemove;
        public event Action<ITimedObject> OnUpdate;

        public ChangeNotifier(List<T> list, Action<T> add, Action<T> remove, Action<T> update)
        {
            this.list = list;
            this.add = add;
            this.remove = remove;
            this.update = update;
        }

        public void Add(ITimedObject obj)
        {
            list.Add((T)obj);
            add?.Invoke((T)obj);
            OnAdd?.Invoke(obj);
        }

        public void Remove(ITimedObject obj)
        {
            list.Remove((T)obj);
            remove?.Invoke((T)obj);
            OnRemove?.Invoke(obj);
        }

        public void Update(ITimedObject obj)
        {
            update?.Invoke((T)obj);
            OnUpdate?.Invoke(obj);
        }

        public void ApplyOffset(float offset)
        {
            foreach (var obj in list)
            {
                obj.Time += offset;
                Update(obj);
            }
        }

        public bool Matches(Type type) => typeof(T) == type;
    }
}
