using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Panel.Types;
using fluXis.Mods;
using fluXis.Replays;
using fluXis.Scoring;
using fluXis.Screens.Edit;
using fluXis.Screens.Gameplay;
using fluXis.Screens.Gameplay.Capabilities;
using fluXis.Screens.Select.Footer;
using osu.Framework.Input;
using osu.Framework.Screens;

namespace fluXis.Screens.Select;

public partial class SoloSelectScreen : SelectScreen
{
    private InputManager input;

    protected override void LoadComplete()
    {
        base.LoadComplete();
        input = GetContainingInputManager();
    }

    public override Action<RealmMap> CreateEditAction() => map =>
    {
        if (map == null) return;

        Maps.Select(map, true);
        if (Maps.CurrentMap == null) return;

        if (map.MapSet.AutoImported)
        {
            Panels.Content = new SingleButtonPanel(
                Phosphor.Bold.Warning,
                "This map cannot be edited.",
                "This map is auto-imported from a different game and cannot be opened in the editor.");
            return;
        }

        var loadedMap = map.GetMapInfo();
        if (loadedMap == null) return;

        var editor = new EditorLoader(map, loadedMap);
        this.Push(editor);
    };

    protected override SelectFooter CreateFooter()
    {
        var footer = base.CreateFooter();
        footer.PracticeAction = (s, e) =>
        {
            var map = Maps.CurrentMap;
            var mods = CurrentMods.ToList();
            this.Push(new GameplayLoader(
                Maps.CurrentMap, mods,
                () => new GameplayScreen(map, mods)
                    .RegisterCapability(new PracticeCapability(s, e))
            ));
        };
        return footer;
    }

    protected override void StartMap(RealmMap map, List<IMod> mods, List<ScoreInfo> scores)
    {
        var hasAuto = mods.Any(m => m is AutoPlayMod);
        var auto = hasAuto || input.CurrentState.Keyboard.ControlPressed;

        if (auto && !hasAuto)
            mods.Add(new AutoPlayMod());

        if (auto)
        {
            ContinueToReplay(map, mods, () =>
            {
                var info = map.GetMapInfo(mods);
                var autogen = new AutoGenerator(info, map.KeyCount);
                return autogen.Generate();
            });
        }
        else
            this.Push(new GameplayLoader(map, mods, () => GameplayScreen.Solo(map, mods, scores)));
    }
}
