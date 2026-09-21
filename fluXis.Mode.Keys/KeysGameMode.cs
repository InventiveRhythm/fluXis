using System;
using fluXis.Map;
using fluXis.Map.Format;
using fluXis.Map.Structures.Bases;
using fluXis.Mode.Keys.Editor.Objects;
using fluXis.Mode.Keys.Editor.Tools;
using fluXis.Mode.Keys.Format;
using fluXis.Mode.Keys.Gameplay;
using fluXis.Mode.Keys.Objects;
using fluXis.Modes;
using fluXis.Modes.Gameplay;
using fluXis.Mods;
using fluXis.Replays;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Screens.Edit.Tabs.Charting.Tools;
using fluXis.Screens.Gameplay.Ruleset;
using osu.Framework.Platform;
using rhym;

namespace fluXis.Mode.Keys;

public class KeysGameMode : GameMode
{
    public override ResourceLocation Location => new("flux", "keys");

    public override void RegisterObjects(RhymIO io)
    {
        io.RegisterObject<Note>("flux:keys/note");
        io.RegisterObject<LongNote>("flux:keys/long");
        io.RegisterObject<Tick>("flux:keys/tick");
        io.RegisterObject<Landmine>("flux:keys/mine");
    }

    public override IMapFormat? GetFormat(Storage storage, string ext)
    {
        if (ext == ".fsc")
            return new LegacyMapFormat(storage);

        return base.GetFormat(storage, ext);
    }

    public override EditorDrawableObject? CreateEditorObject(ITimedObject obj) => obj switch
    {
        Note o => new EditorSingleNote(o),
        LongNote o => new EditorLongNote(o),
        Tick o => new EditorTickNote(o),
        Landmine o => new EditorLandmine(o),
        _ => null
    };

    public override ChartingTool[] GetEditorTools()
        => [new SingleNoteTool(), new LongNoteTool(), new TickNoteTool(), new LandmineTool()];

    public override AutoGenerator CreateAutoGenerator(PlayableMap map) => new KeysAutoGenerator(map);

    public override PlayableGameMode CreatePlayable(RulesetContainer ruleset, PlayableMap map, IMod[] mods) => new KeysPlayableGameMode(ruleset, map, mods);

    public static int ParseMode(ResourceLocation loc)
    {
        var count = 4;
        var split = loc.Path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        for (var i = 0; i < split.Length; i++)
        {
            switch (i)
            {
                case 1:
                    count = int.Parse(split[i]);
                    break;
            }
        }

        return count;
    }
}
