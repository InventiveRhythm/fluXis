using System;
using System.Collections;
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
using fluXis.Map.Structures.Events.Camera;
using fluXis.Map.Structures.Events.Groups;
using fluXis.Map.Structures.Events.Playfields;
using fluXis.Map.Structures.Events.Scrolling;
using fluXis.Modes;
using fluXis.Screens.Edit.Tabs.Verify;
using fluXis.Storyboards;
using fluXis.Utils;
using fluXis.Utils.Extensions;
using JetBrains.Annotations;
using Midori.Utils;
using osu.Framework.Graphics;
using osu.Framework.Threading;

namespace fluXis.Screens.Edit;

public class EditorMap : IVerifyContext
{
    public PlayableMap Playable { get; set; }
    public RealmMap RealmMap { get; set; }

    public FileWatcher ScriptWatcher { get; private set; }

    public Storyboard Storyboard => Playable.Storyboard;
    public RealmMapSet MapSet => RealmMap?.MapSet;

    private readonly Action<Drawable> loadComponent;
    private readonly Scheduler scheduler;
    private readonly Editor editor;

    public string MapInfoHash => MapUtils.GetHash(Playable.Serialize());
    public string StoryboardHash => MapUtils.GetHash(Storyboard.Serialize());

    public bool IsNew => RealmMap == null || Playable == null;

    public PanelContainer Panels { get; set; }

    private List<IChangeNotifier> notifiers = [];

    public EditorMap(PlayableMap playable, RealmMap realm, Action<Drawable> loadComponent, Scheduler scheduler, Editor editor)
    {
        Playable = playable;
        RealmMap = realm;

        this.loadComponent = loadComponent;
        this.scheduler = scheduler;
        this.editor = editor;
    }

    public void SetupWatcher()
    {
        ScriptWatcher = new FileWatcher(RealmMap.MapSet, "*.lua");
        ScriptWatcher.Changed += (_, e) => Schedule(() => ScriptChanged?.Invoke(e.Name));
    }

    #region Events

    public event Action<int> KeyModeChanged;

    public event Action AudioChanged;
    public event Action BackgroundChanged;
    public event Action CoverChanged;

    public event Action<string> ScriptChanged;

#nullable enable
    public event Action<ITimedObject?>? AnyChange;
#nullable disable

    public event Action HitSoundsChanged;

    #endregion

    public void LoadComponent(Drawable drawable) => loadComponent.Invoke(drawable);
    public void Schedule(Action action) => scheduler.ScheduleIfNeeded(action);

    public void TriggerAnyChange(ITimedObject obj = null) => AnyChange?.Invoke(obj);

    public void SetupNotifiers()
    {
        // TODO: none of these change notifiers save objects into the map
        notifiers = new List<IChangeNotifier>
        {
            new ChangeNotifier<HitObject>(Playable.ObjectsOfType<HitObject>()),
            new ChangeNotifier<TimingPoint>(Playable.ObjectsOfType<TimingPoint>()),
            new ChangeNotifier<HitSoundFade>(Playable.ObjectsOfType<HitSoundFade>()),
            new ChangeNotifier<ScrollVelocity>(Playable.ObjectsOfType<ScrollVelocity>()),
            new ChangeNotifier<LaneSwitchEvent>(Playable.ObjectsOfType<LaneSwitchEvent>()),
            new ChangeNotifier<FlashEvent>(Playable.ObjectsOfType<FlashEvent>()),
            new ChangeNotifier<ColorFadeEvent>(Playable.ObjectsOfType<ColorFadeEvent>()),
            new ChangeNotifier<PulseEvent>(Playable.ObjectsOfType<PulseEvent>()),
            new ChangeNotifier<PlayfieldMoveEvent>(Playable.ObjectsOfType<PlayfieldMoveEvent>()),
            new ChangeNotifier<PlayfieldScaleEvent>(Playable.ObjectsOfType<PlayfieldScaleEvent>()),
            new ChangeNotifier<PlayfieldRotateEvent>(Playable.ObjectsOfType<PlayfieldRotateEvent>()),
            new ChangeNotifier<LayerFadeEvent>(Playable.ObjectsOfType<LayerFadeEvent>()),
            new ChangeNotifier<HitObjectEaseEvent>(Playable.ObjectsOfType<HitObjectEaseEvent>()),
            new ChangeNotifier<ShakeEvent>(Playable.ObjectsOfType<ShakeEvent>()),
            new ChangeNotifier<ShaderEvent>(Playable.ObjectsOfType<ShaderEvent>()),
            new ChangeNotifier<BeatPulseEvent>(Playable.ObjectsOfType<BeatPulseEvent>()),
            new ChangeNotifier<ScrollMultiplierEvent>(Playable.ObjectsOfType<ScrollMultiplierEvent>()),
            new ChangeNotifier<TimeOffsetEvent>(Playable.ObjectsOfType<TimeOffsetEvent>()),
            new ChangeNotifier<CameraMoveEvent>(Playable.ObjectsOfType<CameraMoveEvent>()),
            new ChangeNotifier<CameraScaleEvent>(Playable.ObjectsOfType<CameraScaleEvent>()),
            new ChangeNotifier<CameraRotateEvent>(Playable.ObjectsOfType<CameraRotateEvent>()),
            new ChangeNotifier<LoopEvent>(Playable.ObjectsOfType<LoopEvent>()),
            new ChangeNotifier<NoteEvent>(Playable.ObjectsOfType<NoteEvent>()),
            new ChangeNotifier<StoryboardAnimation>(new List<StoryboardAnimation>()),
            Playable.Storyboard
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
        var highestLane = Playable.ObjectsOfType<HitObject>().MaxBy(o => o.Lane)?.Lane ?? 0;
        highestLane = Math.Max(highestLane, Playable.ObjectsOfType<LaneSwitchEvent>().MaxBy(o => o.Count)?.Count ?? 0);
        return highestLane <= mode;
    }

    #region Assets

    public void SetAudio(FileInfo file)
    {
        if (file == null || !copyFile(file))
            return;

        Playable.AudioFile = file.Name;
        RealmMap.AudioHash = MapUtils.GetXXHash(file.OpenRead());
        AudioChanged?.Invoke();
    }

    public void SetBackground(FileInfo file)
    {
        if (file == null || !copyFile(file))
            return;

        Playable.BackgroundFile = file.Name;
        BackgroundChanged?.Invoke();

        // update accent color
        using var stream = RealmMap.GetBackgroundStream();
        var color = ImageUtils.GetAverageColour(stream);
        Playable.Colors["$accent"] = color != Colour4.Transparent ? color : Colour4.Transparent;
    }

    public void SetCover(FileInfo file)
    {
        if (file == null || !copyFile(file))
            return;

        Playable.CoverFile = file.Name;
        CoverChanged?.Invoke();
    }

    public void SetVideo(FileInfo file)
    {
        if (file == null || !copyFile(file))
            return;

        Playable.VideoFile = file.Name;
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

    public List<object> GetObjectsOfType(Type type)
    {
        if (!type.IsClass || !typeof(ITimedObject).IsAssignableFrom(type))
            throw new ArgumentException($"The type must be a class and implement {nameof(ITimedObject)}.");

        var method = GetType().GetMethod(nameof(GetObjectsOfType), Type.EmptyTypes)!;
        var generic = method.MakeGenericMethod(type);
        var result = generic.Invoke(this, null)!;
        return ((IEnumerable)result).Cast<object>().ToList();
    }

    public List<T> GetObjectsOfType<T>()
        where T : class, ITimedObject
    {
        if (!tryFindNotifier<T>(out var n))
            throwMissingHandler(typeof(T));

        return n is not IHoldsList<T> hold ? throw new InvalidOperationException() : hold.Objects.ToList();
    }

    private void throwMissingHandler(ITimedObject obj) => throwMissingHandler(obj.GetType());
    private void throwMissingHandler(Type type) => throw new ArgumentException($"Type '{type.Name}' does not have a change handler associated with it.");

    public void ApplyOffsetToAll(double offset) => notifiers.ForEach(n => n.ApplyOffset(offset));

    public void Sort() => Playable.Sort();

    #endregion

    #region IVerifyContext Implementation

    GameModeManager IVerifyContext.Modes => editor.GameModes;
    PlayableMap IVerifyContext.Map => Playable;
    RealmMap IVerifyContext.RealmMap => RealmMap;

    #endregion

    public interface IChangeNotifier
    {
        event Action<ITimedObject> OnAdd;
        event Action<ITimedObject> OnRemove;
        event Action<ITimedObject> OnUpdate;

        void Add(ITimedObject obj);
        void Remove(ITimedObject obj);
        void Update(ITimedObject obj);

        void ApplyOffset(double offset);

        bool Matches(Type type);
    }

    public interface IHoldsList<T>
        where T : class, ITimedObject
    {
        List<T> Objects { get; }
    }

    private class ChangeNotifier<T> : IChangeNotifier, IHoldsList<T>
        where T : class, ITimedObject
    {
        public List<T> Objects { get; }

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

        public ChangeNotifier(IEnumerable<T> list, Action<T> add = null, Action<T> remove = null, Action<T> update = null)
        {
            Objects = [.. list];
            this.add = add;
            this.remove = remove;
            this.update = update;
        }

        public void Add(ITimedObject obj)
        {
            Objects.Add((T)obj);
            add?.Invoke((T)obj);
            OnAdd?.Invoke(obj);
            OnTypedAdd?.Invoke((T)obj);
        }

        public void Remove(ITimedObject obj)
        {
            Objects.Remove((T)obj);
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

        public void ApplyOffset(double offset)
        {
            foreach (var obj in Objects)
            {
                obj.Time += offset;
                Update(obj);
            }
        }

        public bool Matches(Type type) => typeof(T) == type;
    }
}
