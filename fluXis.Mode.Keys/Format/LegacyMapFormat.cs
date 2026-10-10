using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using fluXis.Map;
using fluXis.Map.Format;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Map.Structures.Events.Camera;
using fluXis.Map.Structures.Events.Groups;
using fluXis.Map.Structures.Events.Playfields;
using fluXis.Map.Structures.Events.Scrolling;
using fluXis.Mode.Keys.Objects;
using fluXis.Online.API.Models.Maps;
using fluXis.Storyboards;
using fluXis.Utils;
using fluXis.Utils.Extensions;
using Midori.Utils;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osuTK.Graphics;
using rhym;

namespace fluXis.Mode.Keys.Format;

public class LegacyMapFormat : IMapFormat
{
    private readonly Storage storage;

    public LegacyMapFormat(Storage storage)
    {
        this.storage = storage;
    }

    public bool IsChart(string path) => storage.Exists(path) && path.EndsWith(".fsc");

    public bool Supports(ITimedObject obj)
    {
        return obj is HitObject or TimingPoint or ScrollVelocity
            or HitSoundFade or LaneSwitchEvent or FlashEvent or ColorFadeEvent
            or PulseEvent or PlayfieldMoveEvent or PlayfieldScaleEvent
            or PlayfieldRotateEvent or HitObjectEaseEvent or LayerFadeEvent
            or ShakeEvent or ShaderEvent or BeatPulseEvent or ScrollMultiplierEvent
            or TimeOffsetEvent or CameraMoveEvent or CameraScaleEvent
            or CameraRotateEvent or LoopEvent or NoteEvent;
    }

    public PlayableMap? Parse(string path)
    {
        try
        {
            if (!storage.Exists(path))
                return null;

            var json = storage.ReadAllText(path);
            var map = json.Deserialize<LegacyMapJson>();

            if (map is null)
                return null;

            var keys = map.HitObjects.Select(hitObject => hitObject.Lane).Prepend(0).Max();
            var mode = $"keys/{(int)Math.Ceiling(keys / (map.DualMode == DualMode.Separate ? 2f : 1))}";

            var playable = new PlayableMap(storage, path, new ResourceLocation("flux", mode))
            {
                AudioFile = map.AudioFile,
                BackgroundFile = map.BackgroundFile,
                CoverFile = map.CoverFile,
                VideoFile = map.VideoFile,
                ImportPaths = [],

                TimeInEditor = map.TimeInEditor,

                ChartHash = MapUtils.GetHash(json),

                Title = map.Metadata.Title,
                TitleRomanized = map.Metadata.TitleRomanized,
                Artist = map.Metadata.Artist,
                ArtistRomanized = map.Metadata.ArtistRomanized,
                Difficulty = map.Metadata.Difficulty,
                Creator = map.Metadata.Mapper,
                PreviewTime = map.Metadata.PreviewTime,
                Colors = new Dictionary<string, Color4>
                {
                    { "$accent", map.Colors.Accent },
                    { "$primary", map.Colors.Primary },
                    { "$secondary", map.Colors.Secondary },
                    { "$middle", map.Colors.Middle }
                },
                Sources = new Dictionary<string, string>
                {
                    { "audio", map.Metadata.AudioSource },
                    { "background", map.Metadata.BackgroundSource },
                    { "cover", map.Metadata.CoverSource },
                },
                Tags = [.. map.Metadata.Tags.Split(",").Select(x => x.Trim())],

                AccuracyDifficulty = map.AccuracyDifficulty,
                HealthDifficulty = map.HealthDifficulty,
                Dual = map.DualMode,
                AudioVisualizations = map.EnableVisualization,
                ExtraPlayfields = map.ExtraPlayfields,
                ForceAspect = map.Force16By9,
                LegacyLaneSwitchLayout = !map.NewLaneSwitchLayout,
            };

            playable.AddObjects(map.HitObjects.Select(h =>
            {
                HitObject obj = h.Type switch
                {
                    LegacyHitObjectType.Normal when !h.LongNote => new Note(),
                    LegacyHitObjectType.Normal when h.LongNote => new LongNote { Duration = h.HoldTime },
                    LegacyHitObjectType.Tick => new Tick { Small = h.HoldTime > 0 },
                    LegacyHitObjectType.Landmine => new Landmine { Hidden = h.Hidden },
                    _ => throw new ArgumentOutOfRangeException()
                };

                obj.Time = h.Time;
                obj.Lane = h.Lane;
                obj.Group = h.Group;
                return obj;
            }));

            playable.AddObjects(map.TimingPoints);
            playable.AddObjects(map.ScrollVelocities);

            if (!string.IsNullOrWhiteSpace(map.EffectFile) && storage.Exists(map.EffectFile))
            {
                var eff = storage.ReadAllText(map.EffectFile);
                var effects = LegacyMapEvents.Load<LegacyMapEvents>(eff);
                effects.ForAllEvents(playable.AddObject);

                playable.EventsHash = MapUtils.GetHash(eff);
                playable.ImportPaths[IMapFormat.IMPORT_EVENT] = map.EffectFile;
            }

            if (!string.IsNullOrWhiteSpace(map.StoryboardFile) && storage.Exists(map.StoryboardFile))
            {
                var sbj = storage.ReadAllText(map.StoryboardFile);
                playable.Storyboard = json.Deserialize<Storyboard>()!;
                playable.Storyboard?.Update();

                playable.StoryboardHash = MapUtils.GetHash(sbj);
                playable.ImportPaths[IMapFormat.IMPORT_STORYBOARD] = map.StoryboardFile;
            }

            return playable;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to load map from path: " + storage.GetFullPath(path));
            return null;
        }
    }

    public Dictionary<string, string> Save(PlayableMap map)
    {
        var hashes = new Dictionary<string, string>();

        var json = new LegacyMapJson
        {
            AudioFile = map.AudioFile,
            BackgroundFile = map.BackgroundFile,
            CoverFile = map.CoverFile,
            VideoFile = map.VideoFile,
            EffectFile = string.Empty,
            StoryboardFile = string.Empty,
            Metadata = new LegacyMapMetadata
            {
                Title = map.Title,
                TitleRomanized = map.TitleRomanized,
                Artist = map.Artist,
                ArtistRomanized = map.ArtistRomanized,
                Mapper = map.Creator,
                Difficulty = map.Difficulty,
                AudioSource = map.Sources.GetValueOrDefault("audio", string.Empty),
                BackgroundSource = map.Sources.GetValueOrDefault("background", string.Empty),
                CoverSource = map.Sources.GetValueOrDefault("cover", string.Empty),
                Tags = string.Join(",", map.Tags),
                PreviewTime = map.PreviewTime
            },
            Colors = new LegacyMapColors
            {
                AccentHex = map.Colors.ContainsKey("$accent") ? map.Colors["$accent"].ToHex() : string.Empty,
                PrimaryHex = map.Colors.ContainsKey("$primary") ? map.Colors["$primary"].ToHex() : string.Empty,
                SecondaryHex = map.Colors.ContainsKey("$secondary") ? map.Colors["$secondary"].ToHex() : string.Empty,
                MiddleHex = map.Colors.ContainsKey("$middle") ? map.Colors["$accent"].ToHex() : string.Empty,
            },
            HitObjects = [],
            TimingPoints = [],
            ScrollVelocities = [],
            HitSoundFades = [],
            AccuracyDifficulty = map.AccuracyDifficulty,
            HealthDifficulty = map.HealthDifficulty,
            DualMode = map.Dual,
            NewLaneSwitchLayout = !map.LegacyLaneSwitchLayout,
            Force16By9 = map.ForceAspect,
            TimeInEditor = map.TimeInEditor,
            ExtraPlayfields = map.ExtraPlayfields,
            EnableVisualization = map.AudioVisualizations
        };

        var events = new LegacyMapEvents();

        foreach (var obj in map.Objects)
        {
            switch (obj)
            {
                case HitObject h:
                    json.HitObjects.Add(new LegacyHitObject
                    {
                        Time = h.Time,
                        Lane = h.Lane,
                        EndTime = h.GetEndTime(),
                        HitSound = h.Sample,
                        Group = h.Group,
                        Hidden = h is Landmine { Hidden: true },
                        Type = h switch
                        {
                            Note or LongNote => LegacyHitObjectType.Normal,
                            Tick => LegacyHitObjectType.Tick,
                            Landmine => LegacyHitObjectType.Landmine,
                            _ => throw new ArgumentOutOfRangeException()
                        }
                    });
                    break;

                case TimingPoint tp:
                    json.TimingPoints.Add(tp);
                    break;

                case ScrollVelocity sv:
                    json.ScrollVelocities.Add(sv);
                    break;

                case HitSoundFade hsf:
                    json.HitSoundFades.Add(hsf);
                    break;

                default:
                    var list = events.GetListsForTypes()[obj.GetType()];
                    (list as IList)!.Add(obj);
                    break;
            }
        }

        if (events is { Empty: false })
        {
            var name = map.ImportPaths.GetValueOrDefault(IMapFormat.IMPORT_EVENT, $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.ffx");
            var raw = events.Sort().Serialize(true);

            map.ImportPaths[IMapFormat.IMPORT_EVENT] = name;
            hashes[IMapFormat.IMPORT_EVENT] = MapUtils.GetHash(raw);
            json.EffectFile = name;
            storage.WriteAllText(name, raw);
        }
        else if (map.ImportPaths.TryGetValue(IMapFormat.IMPORT_EVENT, out var path))
        {
            // remove old
            storage.Delete(path);
        }

        if (map.Storyboard is { Empty: false })
        {
            var name = map.ImportPaths.GetValueOrDefault(IMapFormat.IMPORT_STORYBOARD, $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.fsb");
            var raw = map.Storyboard.Save();

            map.ImportPaths[IMapFormat.IMPORT_STORYBOARD] = name;
            hashes[IMapFormat.IMPORT_STORYBOARD] = MapUtils.GetHash(raw);
            json.StoryboardFile = name;
            storage.WriteAllText(name, raw);
        }
        else if (map.ImportPaths.TryGetValue(IMapFormat.IMPORT_STORYBOARD, out var path))
        {
            // remove old
            storage.Delete(path);
        }

        var rawChart = json.Serialize(true);
        hashes[IMapFormat.IMPORT_CHART] = MapUtils.GetHash(rawChart);
        storage.WriteAllText(map.FileName, rawChart);

        return hashes;
    }
}
