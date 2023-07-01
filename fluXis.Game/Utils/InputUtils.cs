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
        if (combo.Keys.Contains(InputKey.Alt))
            result += "Alt + ";
        if (combo.Keys.Contains(InputKey.Shift))
            result += "Shift + ";
        if (combo.Keys.Contains(InputKey.Super))
            result += "Super + ";

        return combo.Keys.Where(key => key >= InputKey.F1).Aggregate(result, (current, key) => current + $"{key}");
    }
}
