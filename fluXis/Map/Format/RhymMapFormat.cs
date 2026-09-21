using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Modes;
using fluXis.Storyboards;
using fluXis.Utils;
using fluXis.Utils.Extensions;
using Midori.Utils;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Platform;
using osuTK.Graphics;
using rhym;
using rhym.Format;
using ResourceLocation = rhym.ResourceLocation;

namespace fluXis.Map.Format;

#nullable enable

public class RhymMapFormat : IMapFormat
{
    public static ResourceLocation ChartFormat { get; } = new("flux", "chart");
    public static ResourceLocation EffectFormat { get; } = new("flux", "effect");
    public static ResourceLocation StoryboardFormat { get; } = new("flux", "storyboard");

    private readonly Storage storage;
    private readonly GameModeManager modes;

    public RhymMapFormat(Storage storage, GameModeManager modes)
    {
        this.storage = storage;
        this.modes = modes;
    }

    public bool IsChart(string path)
    {
        if (!storage.Exists(path))
            return false;

        var raw = storage.ReadAllText(path);
        var io = createIo();
        var parsed = io.ParseAs<RawMapFile, RawMapAssets, RawMapMetadata, ITimedObject>(raw);
        return parsed.FormatID == ChartFormat;
    }

    public PlayableMap? Parse(string path)
    {
        if (!storage.Exists(path))
            return null;

        var raw = storage.ReadAllText(path);
        var io = createIo();
        var parsed = io.ParseAs<RawMapFile, RawMapAssets, RawMapMetadata, ITimedObject>(raw);

        if (parsed.FormatID != ChartFormat)
            throw new InvalidOperationException($"Tried to load {parsed.FormatID} as a chart.");

        var playable = new PlayableMap(storage, path, parsed.GameMode)
        {
            AudioFile = parsed.Assets.AudioPath ?? string.Empty,
            BackgroundFile = parsed.Assets.BackgroundPath ?? string.Empty,
            CoverFile = parsed.Assets.CoverPath ?? string.Empty,
            VideoFile = parsed.Assets.VideoPath ?? string.Empty,
            ImportPaths = new Dictionary<string, string>(),

            TimeInEditor = parsed.Editor.TimeSpent,

            ChartHash = MapUtils.GetHash(raw),

            Title = parsed.Metadata.Title,
            TitleRomanized = parsed.Metadata.TitleRomanized,
            Artist = parsed.Metadata.Artist,
            ArtistRomanized = parsed.Metadata.ArtistRomanized,
            Difficulty = parsed.Metadata.Difficulty,
            Creator = parsed.Metadata.Creator,
            PreviewTime = parsed.Metadata.PreviewTime,
            Colors = parsed.Metadata.Colors.ToDictionary(
                x => x.Key,
                x =>
                {
                    if (!Colour4.TryParseHex(x.Value, out var c))
                        return Color4.Transparent;

                    Color4 tk = c;
                    return tk with { A = 1 };
                }),
            Sources = parsed.Metadata.Sources,
            Tags = parsed.Metadata.Tags,

            AccuracyDifficulty = parsed.Properties.AccuracyDifficulty,
            HealthDifficulty = parsed.Properties.HealthDifficulty,
            Dual = parsed.Properties.Dual,
            AudioVisualizations = parsed.Properties.AudioVisualizations,
            ExtraPlayfields = parsed.Properties.ExtraPlayfields,
            ForceAspect = parsed.Properties.ForceAspect,
            LegacyLaneSwitchLayout = false,
        };
        playable.AddObjects(parsed.Objects);

        foreach (var import in parsed.Imports)
        {
            if (!storage.Exists(path))
                continue;

            var importRaw = storage.ReadAllText(import);
            var hash = MapUtils.GetHash(importRaw);

            switch (Path.GetExtension(import))
            {
                case ".rhym":
                {
                    var ev = io.ParseAs<RawEventFile, RhymAssets, RhymMetadata, ITimedObject>(importRaw);
                    if (ev.FormatID != EffectFormat) throw new InvalidOperationException($"Tried to import a non-effect ({ev.FormatID}) file as effects.");

                    playable.EffectHash = hash;
                    playable.ImportPaths["events"] = import;
                    playable.AddObjects(ev.Objects);
                    break;
                }

                case ".fsb":
                {
                    playable.Storyboard = importRaw.Deserialize<Storyboard>();
                    playable.Storyboard?.Update();

                    playable.StoryboardHash = hash;
                    playable.ImportPaths["storyboard"] = import;
                    break;
                }
            }
        }

        playable.Sort();
        return playable;
    }

    public void Save(PlayableMap map)
    {
        var chart = new List<ITimedObject>();
        var events = new List<ITimedObject>();

        foreach (var o in map.Objects)
        {
            switch (o)
            {
                case HitObject:
                case TimingPoint:
                    chart.Add(o);
                    break;

                default:
                    events.Add(o);
                    break;
            }
        }

        var rhym = new RawMapFile
        {
            FormatID = ChartFormat,
            GameMode = map.Mode,
            Metadata = new RawMapMetadata
            {
                Artist = map.Artist,
                ArtistRomanized = map.ArtistRomanized,
                Colors = map.Colors.ToDictionary(x => x.Key, x => x.Value.ToHex()),
                Creator = map.Creator,
                Difficulty = map.Difficulty,
                PreviewTime = map.PreviewTime,
                Sources = map.Sources,
                Tags = map.Tags,
                Title = map.Title,
                TitleRomanized = map.TitleRomanized
            },
            Assets = new RawMapAssets
            {
                AudioPath = map.AudioFile,
                BackgroundPath = map.BackgroundFile,
                CoverPath = map.CoverFile,
                VideoPath = map.VideoFile
            },
            Editor = new RawMapEditor { TimeSpent = map.TimeInEditor },
            Properties = new RawMapProperties
            {
                AccuracyDifficulty = map.AccuracyDifficulty,
                AudioVisualizations = map.AudioVisualizations,
                Dual = map.Dual,
                ExtraPlayfields = map.ExtraPlayfields,
                ForceAspect = map.ForceAspect,
                HealthDifficulty = map.HealthDifficulty
            },
            Objects = [.. chart]
        };

        var io = createIo();
        var raw = io.Serialize(rhym);
        File.WriteAllText("/home/flux/map.yaml", raw);

        File.WriteAllText("/home/flux/map-events.yaml", io.Serialize(new RawEventFile
        {
            FormatID = EffectFormat,
            Objects = [.. events]
        }));
    }

    private RhymIO createIo()
    {
        var io = new RhymIO();
        io.RegisterConverter(new YamlColorConverter());
        io.RegisterObject<TimingPoint>("flux:timing");
        io.RegisterObject<ScrollVelocity>("flux:event/scroll/velocity");

        foreach (var type in IMapEvent.GetAllTypes())
        {
            var attr = type.GetCustomAttribute<ResourceLocationAttribute>();
            if (attr is null) throw new InvalidOperationException($"Map event {type.Name} does not have a resource ID.");

            io.RegisterObject(type, attr.Location);
        }

        modes.Loaded.ForEach(x => x.RegisterObjects(io));
        return io;
    }
}
