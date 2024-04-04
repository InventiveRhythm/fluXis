using System;
using System.IO;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Map.Events;
using fluXis.Game.Map.Structures;
using fluXis.Game.Utils;
using fluXis.Shared.Utils;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using SixLabors.ImageSharp;

namespace fluXis.Game.Screens.Edit;

public class EditorMap
{
    public EditorMapInfo MapInfo { get; set; }
    public RealmMap RealmMap { get; set; }

    public MapEvents MapEvents => MapInfo.MapEvents;
    public RealmMapSet MapSet => RealmMap?.MapSet;

    public string MapInfoHash => MapUtils.GetHash(MapInfo.Serialize());
    public string MapEventsHash => MapUtils.GetHash(MapEvents.Save());

    public bool IsNew => RealmMap == null || MapInfo == null;

    #region Events

    public event Action<int> KeyModeChanged;

    public event Action AudioChanged;
    public event Action BackgroundChanged;
    public event Action CoverChanged;

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

    public event Action<ShakeEvent> ShakeEventAdded;
    public event Action<ShakeEvent> ShakeEventRemoved;
    public event Action<ShakeEvent> ShakeEventUpdated;

    public event Action<PlayfieldFadeEvent> PlayfieldFadeEventAdded;
    public event Action<PlayfieldFadeEvent> PlayfieldFadeEventRemoved;
    public event Action<PlayfieldFadeEvent> PlayfieldFadeEventUpdated;

    public event Action<PlayfieldMoveEvent> PlayfieldMoveEventAdded;
    public event Action<PlayfieldMoveEvent> PlayfieldMoveEventRemoved;
    public event Action<PlayfieldMoveEvent> PlayfieldMoveEventUpdated;

    public event Action<PlayfieldScaleEvent> PlayfieldScaleEventAdded;
    public event Action<PlayfieldScaleEvent> PlayfieldScaleEventRemoved;
    public event Action<PlayfieldScaleEvent> PlayfieldScaleEventUpdated;

    #endregion

    public void SetKeyMode(int mode)
    {
        if (!CanChangeTo(mode))
            return;

        RealmMap.KeyCount = mode;
        KeyModeChanged?.Invoke(mode);
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
        if (file == null)
            return;

        copyFile(file);
        MapInfo.AudioFile = file.Name;
        RealmMap.Metadata.Audio = file.Name;
        AudioChanged?.Invoke();
    }

    public void SetBackground(FileInfo file)
    {
        if (file == null)
            return;

        copyFile(file);
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
        if (file == null)
            return;

        copyFile(file);
        RealmMap.MapSet.Cover = file.Name;
        MapInfo.CoverFile = file.Name;
        CoverChanged?.Invoke();
    }

    public void SetVideo(FileInfo file)
    {
        if (file == null)
            return;

        copyFile(file);
        MapInfo.VideoFile = file.Name;
    }

    private void copyFile(FileInfo file)
    {
        var mapDir = new DirectoryInfo(MapFiles.GetFullPath(MapSet.ID.ToString()));

        if (file.Directory != null && file.Directory.FullName == mapDir.FullName)
            return;

        string path = MapFiles.GetFullPath(MapSet.GetPathForFile(file.Name));
        var dir = Path.GetDirectoryName(path);

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        File.Copy(file.FullName, path, true);
    }

    #endregion

    #region Objects

    public void Add(ITimedObject obj)
    {
        switch (obj)
        {
            case HitObject hitObject:
                MapInfo.HitObjects.Add(hitObject);
                HitObjectAdded?.Invoke(hitObject);
                break;

            case TimingPoint timingPoint:
                MapInfo.TimingPoints.Add(timingPoint);
                TimingPointAdded?.Invoke(timingPoint);
                break;

            case ScrollVelocity scrollVelocity:
                MapInfo.ScrollVelocities.Add(scrollVelocity);
                ScrollVelocityAdded?.Invoke(scrollVelocity);
                break;

            case LaneSwitchEvent laneSwitchEvent:
                MapEvents.LaneSwitchEvents.Add(laneSwitchEvent);
                LaneSwitchEventAdded?.Invoke(laneSwitchEvent);
                break;

            case FlashEvent flashEvent:
                MapEvents.FlashEvents.Add(flashEvent);
                FlashEventAdded?.Invoke(flashEvent);
                break;

            case ShakeEvent shakeEvent:
                MapEvents.ShakeEvents.Add(shakeEvent);
                ShakeEventAdded?.Invoke(shakeEvent);
                break;

            case PlayfieldFadeEvent fadeEvent:
                MapEvents.PlayfieldFadeEvents.Add(fadeEvent);
                PlayfieldFadeEventAdded?.Invoke(fadeEvent);
                break;

            case PlayfieldMoveEvent moveEvent:
                MapEvents.PlayfieldMoveEvents.Add(moveEvent);
                PlayfieldMoveEventAdded?.Invoke(moveEvent);
                break;

            case PlayfieldScaleEvent scaleEvent:
                MapEvents.PlayfieldScaleEvents.Add(scaleEvent);
                PlayfieldScaleEventAdded?.Invoke(scaleEvent);
                break;

            default:
                throw new NotImplementedException();
        }
    }

    public void Update(ITimedObject obj)
    {
        switch (obj)
        {
            case HitObject hitObject:
                HitObjectUpdated?.Invoke(hitObject);
                break;

            case TimingPoint timingPoint:
                TimingPointUpdated?.Invoke(timingPoint);
                break;

            case ScrollVelocity scrollVelocity:
                ScrollVelocityUpdated?.Invoke(scrollVelocity);
                break;

            case LaneSwitchEvent laneSwitchEvent:
                LaneSwitchEventUpdated?.Invoke(laneSwitchEvent);
                break;

            case FlashEvent flashEvent:
                FlashEventUpdated?.Invoke(flashEvent);
                break;

            case ShakeEvent shakeEvent:
                ShakeEventUpdated?.Invoke(shakeEvent);
                break;

            case PlayfieldFadeEvent fadeEvent:
                PlayfieldFadeEventUpdated?.Invoke(fadeEvent);
                break;

            case PlayfieldMoveEvent moveEvent:
                PlayfieldMoveEventUpdated?.Invoke(moveEvent);
                break;

            case PlayfieldScaleEvent scaleEvent:
                PlayfieldScaleEventUpdated?.Invoke(scaleEvent);
                break;

            default:
                throw new NotImplementedException();
        }
    }

    public void UpdateHitSounds() => HitSoundsChanged?.Invoke();

    public void Remove(ITimedObject obj)
    {
        switch (obj)
        {
            case HitObject hitObject:
                MapInfo.HitObjects.Remove(hitObject);
                HitObjectRemoved?.Invoke(hitObject);
                break;

            case TimingPoint timingPoint:
                MapInfo.TimingPoints.Remove(timingPoint);
                TimingPointRemoved?.Invoke(timingPoint);
                break;

            case ScrollVelocity scrollVelocity:
                MapInfo.ScrollVelocities.Remove(scrollVelocity);
                ScrollVelocityRemoved?.Invoke(scrollVelocity);
                break;

            case LaneSwitchEvent laneSwitchEvent:
                MapEvents.LaneSwitchEvents.Remove(laneSwitchEvent);
                LaneSwitchEventRemoved?.Invoke(laneSwitchEvent);
                break;

            case FlashEvent flashEvent:
                MapEvents.FlashEvents.Remove(flashEvent);
                FlashEventRemoved?.Invoke(flashEvent);
                break;

            case ShakeEvent shakeEvent:
                MapEvents.ShakeEvents.Remove(shakeEvent);
                ShakeEventRemoved?.Invoke(shakeEvent);
                break;

            case PlayfieldFadeEvent fadeEvent:
                MapEvents.PlayfieldFadeEvents.Remove(fadeEvent);
                PlayfieldFadeEventRemoved?.Invoke(fadeEvent);
                break;

            case PlayfieldMoveEvent moveEvent:
                MapEvents.PlayfieldMoveEvents.Remove(moveEvent);
                PlayfieldMoveEventRemoved?.Invoke(moveEvent);
                break;

            case PlayfieldScaleEvent scaleEvent:
                MapEvents.PlayfieldScaleEvents.Remove(scaleEvent);
                PlayfieldScaleEventRemoved?.Invoke(scaleEvent);
                break;

            default:
                throw new NotImplementedException();
        }
    }

    public void ApplyOffsetToAll(float offset)
    {
        foreach (var hitObject in MapInfo.HitObjects)
        {
            hitObject.Time += offset;
            Update(hitObject);
        }

        foreach (var timingPoint in MapInfo.TimingPoints)
        {
            timingPoint.Time += offset;
            Update(timingPoint);
        }

        foreach (var scrollVelocity in MapInfo.ScrollVelocities)
        {
            scrollVelocity.Time += offset;
            Update(scrollVelocity);
        }

        foreach (var laneSwitchEvent in MapEvents.LaneSwitchEvents)
        {
            laneSwitchEvent.Time += offset;
            Update(laneSwitchEvent);
        }

        foreach (var flashEvent in MapEvents.FlashEvents)
        {
            flashEvent.Time += offset;
            Update(flashEvent);
        }

        foreach (var shakeEvent in MapEvents.ShakeEvents)
        {
            shakeEvent.Time += offset;
            Update(shakeEvent);
        }
    }

    public void Sort()
    {
        MapInfo.Sort();
        MapEvents.Sort();
    }

    #endregion

    public class EditorMapInfo : MapInfo, IDeepCloneable<EditorMapInfo>
    {
        [JsonIgnore]
        public MapEvents MapEvents { get; set; }

        public EditorMapInfo(MapMetadata metadata)
            : base(metadata)
        {
        }

        public EditorMapInfo() { }

        public override T GetMapEvents<T>() => MapEvents as T;

        public EditorMapInfo DeepClone()
        {
            var clone = (EditorMapInfo)MemberwiseClone();
            clone.MapEvents = MapEvents.DeepClone();
            return clone;
        }
    }
}
