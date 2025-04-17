using System.Collections.Generic;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Mods;
using fluXis.Replays;
using fluXis.Scoring;
using fluXis.Screens.Gameplay;
using fluXis.Screens.Gameplay.Practice;
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

    protected override SelectFooter CreateFooter()
    {
        var footer = base.CreateFooter();
        footer.PracticeAction = (s, e) =>
        {
            var map = Maps.CurrentMap;
            var mods = CurrentMods.ToList();
            this.Push(new GameplayLoader(Maps.CurrentMap, mods, () => new PracticeGameplayScreen(map, mods, s, e)));
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
            this.Push(new GameplayLoader(map, mods, () => new GameplayScreen(map, mods) { Scores = scores }));
    }
}
