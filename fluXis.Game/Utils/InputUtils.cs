using System.Linq;
using osu.Framework.Input.Bindings;

namespace fluXis.Game.Utils;

public static class InputUtils
{
    public static string GetReadableString(KeyCombination combo)
    {
        var result = string.Empty;
        if (combo.Keys.Contains(InputKey.Control))
            result += "Ctrl + ";
        if (combo.Keys.Contains(InputKey.LControl))
            result += "LeftCtrl + ";
        if (combo.Keys.Contains(InputKey.RControl))
            result += "RightCtrl + ";

        if (combo.Keys.Contains(InputKey.Shift))
            result += "Shift + ";
        if (combo.Keys.Contains(InputKey.LShift))
            result += "LeftShift + ";
        if (combo.Keys.Contains(InputKey.RShift))
            result += "RightShift + ";

        if (combo.Keys.Contains(InputKey.Alt))
            result += "Alt + ";
        if (combo.Keys.Contains(InputKey.LAlt))
            result += "LeftAlt + ";
        if (combo.Keys.Contains(InputKey.RAlt))
            result += "RightAlt + ";

        if (combo.Keys.Contains(InputKey.Super))
            result += "Super + ";
        if (combo.Keys.Contains(InputKey.LSuper))
            result += "LeftSuper + ";
        if (combo.Keys.Contains(InputKey.RSuper))
            result += "RightSuper + ";

        return combo.Keys.Where(key => key >= InputKey.F1)
                    .Where(inputKey => inputKey is
                        not (InputKey.LControl or InputKey.RControl
                        or InputKey.LShift or InputKey.RShift
                        or InputKey.LAlt or InputKey.RAlt
                        or InputKey.LSuper or InputKey.RSuper))
                    .Aggregate(result, (current, inputKey) => current + $"{inputKey}");
    }
}
