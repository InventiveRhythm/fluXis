using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Map;
using fluXis.Map.Format;
using fluXis.Map.Structures;
using fluXis.Mode.Keys.Objects;
using fluXis.Online.API.Models.Maps;
using fluXis.Storyboards;
using fluXis.Utils;
using fluXis.Utils.Extensions;
using Midori.Utils;
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
                ImportPaths = new Dictionary<string, string>
                {
                    { "effects", map.EffectFile },
                    { "storyboard", map.StoryboardFile },
                },

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

                playable.EffectHash = MapUtils.GetHash(eff);
                playable.ImportPaths["events"] = map.EffectFile;
            }

            if (!string.IsNullOrWhiteSpace(map.StoryboardFile) && storage.Exists(map.StoryboardFile))
            {
                var sbj = storage.ReadAllText(map.StoryboardFile);
                playable.Storyboard = json.Deserialize<Storyboard>()!;
                playable.Storyboard?.Update();

                playable.StoryboardHash = MapUtils.GetHash(sbj);
                playable.ImportPaths["storyboard"] = map.StoryboardFile;
            }

            return playable;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to load map from path: " + storage.GetFullPath(path));
            return null;
        }
    }

    public void Save(PlayableMap map) => throw new NotImplementedException();
}
