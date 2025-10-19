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
using fluXis.Map.Structures.Events.Playfields;
using fluXis.Map.Structures.Events.Scrolling;
using fluXis.Screens.Edit.Tabs.Verify;
using fluXis.Storyboards;
using fluXis.Utils;
using JetBrains.Annotations;
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

    public event Action HitSoundsChanged;

    #endregion

    public void LoadComponent(Drawable drawable) => loadComponent.Invoke(drawable);

    public void TriggerAnyChange(ITimedObject obj = null) => AnyChange?.Invoke(obj);

    public void SetupNotifiers()
    {
        notifiers = new List<IChangeNotifier>
        {
            new ChangeNotifier<HitObject>(MapInfo.HitObjects),
            new ChangeNotifier<TimingPoint>(MapInfo.TimingPoints),
            new ChangeNotifier<ScrollVelocity>(MapInfo.ScrollVelocities),
            new ChangeNotifier<LaneSwitchEvent>(MapEvents.LaneSwitchEvents),
            new ChangeNotifier<FlashEvent>(MapEvents.FlashEvents),
            new ChangeNotifier<ColorFadeEvent>(MapEvents.ColorFadeEvents),
            new ChangeNotifier<PulseEvent>(MapEvents.PulseEvents),
            new ChangeNotifier<PlayfieldMoveEvent>(MapEvents.PlayfieldMoveEvents),
            new ChangeNotifier<PlayfieldScaleEvent>(MapEvents.PlayfieldScaleEvents),
            new ChangeNotifier<PlayfieldRotateEvent>(MapEvents.PlayfieldRotateEvents),
            new ChangeNotifier<LayerFadeEvent>(MapEvents.LayerFadeEvents),
            new ChangeNotifier<HitObjectEaseEvent>(MapEvents.HitObjectEaseEvents),
            new ChangeNotifier<ShakeEvent>(MapEvents.ShakeEvents),
            new ChangeNotifier<ShaderEvent>(MapEvents.ShaderEvents),
            new ChangeNotifier<BeatPulseEvent>(MapEvents.BeatPulseEvents),
            new ChangeNotifier<ScrollMultiplierEvent>(MapEvents.ScrollMultiplyEvents),
            new ChangeNotifier<TimeOffsetEvent>(MapEvents.TimeOffsetEvents),
            new ChangeNotifier<ScriptEvent>(MapEvents.ScriptEvents),
            new ChangeNotifier<NoteEvent>(MapEvents.NoteEvents),
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

    #region Listeners

    public void RegisterAddListener<T>(Action<T> act)
        where T : class, ITimedObject
    {
        if (!tryFindNotifier<T>(out var n))
            throw new InvalidOperationException($"Tried to register a listener for a type that doesn't exist! [{typeof(T).Name}]");

        n.OnTypedAdd += act;
    }

    public void RegisterUpdateListener<T>(Action<T> act)
        where T : class, ITimedObject
    {
        if (!tryFindNotifier<T>(out var n))
            throw new InvalidOperationException($"Tried to register a listener for a type that doesn't exist! [{typeof(T).Name}]");

        n.OnTypedUpdate += act;
    }

    public void RegisterRemoveListener<T>(Action<T> act)
        where T : class, ITimedObject
    {
        if (!tryFindNotifier<T>(out var n))
            throw new InvalidOperationException($"Tried to register a listener for a type that doesn't exist! [{typeof(T).Name}]");

        n.OnTypedRemove += act;
    }

    public void DeregisterAddListener<T>(Action<T> act)
        where T : class, ITimedObject
    {
        if (!tryFindNotifier<T>(out var n))
            throw new InvalidOperationException($"Tried to register a listener for a type that doesn't exist! [{typeof(T).Name}]");

        n.OnTypedAdd -= act;
    }

    public void DeregisterUpdateListener<T>(Action<T> act)
        where T : class, ITimedObject
    {
        if (!tryFindNotifier<T>(out var n))
            throw new InvalidOperationException($"Tried to register a listener for a type that doesn't exist! [{typeof(T).Name}]");

        n.OnTypedUpdate -= act;
    }

    public void DeregisterRemoveListener<T>(Action<T> act)
        where T : class, ITimedObject
    {
        if (!tryFindNotifier<T>(out var n))
            throw new InvalidOperationException($"Tried to register a listener for a type that doesn't exist! [{typeof(T).Name}]");

        n.OnTypedRemove -= act;
    }

    #endregion

    #region Objects

    private bool tryFindNotifier<T>(out ChangeNotifier<T> notifier)
        where T : class, ITimedObject
    {
        notifier = notifiers.FirstOrDefault(n => n.Matches(typeof(T))) as ChangeNotifier<T>;
        return notifier != null;
    }

    private bool tryRunNotifier(ITimedObject obj, Action<IChangeNotifier> action)
    {
        var n = notifiers.FirstOrDefault(n => n.Matches(obj.GetType()));

        if (n is not null)
            action?.Invoke(n);

        return n != null;
    }

    public void Add(ITimedObject obj)
    {
        if (tryRunNotifier(obj, n => n.Add(obj)))
            return;

        throwMissingHandler(obj);
    }

    public void Update(ITimedObject obj)
    {
        if (tryRunNotifier(obj, n => n.Update(obj)))
            return;

        throwMissingHandler(obj);
    }

    public void UpdateHitSounds() => HitSoundsChanged?.Invoke();

    public void Remove(ITimedObject obj)
    {
        if (tryRunNotifier(obj, n => n.Remove(obj)))
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

        [CanBeNull]
        private Action<T> add { get; }

        [CanBeNull]
        private Action<T> remove { get; }

        [CanBeNull]
        private Action<T> update { get; }

        public event Action<ITimedObject> OnAdd;
        public event Action<ITimedObject> OnRemove;
        public event Action<ITimedObject> OnUpdate;

        public event Action<T> OnTypedAdd;
        public event Action<T> OnTypedRemove;
        public event Action<T> OnTypedUpdate;

        public ChangeNotifier(List<T> list, Action<T> add = null, Action<T> remove = null, Action<T> update = null)
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
            OnTypedAdd?.Invoke((T)obj);
        }

        public void Remove(ITimedObject obj)
        {
            list.Remove((T)obj);
            remove?.Invoke((T)obj);
            OnRemove?.Invoke(obj);
            OnTypedRemove?.Invoke((T)obj);
        }

        public void Update(ITimedObject obj)
        {
            update?.Invoke((T)obj);
            OnUpdate?.Invoke(obj);
            OnTypedUpdate?.Invoke((T)obj);
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
