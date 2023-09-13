using System.Linq;
using osu.Framework.Input.Bindings;

namespace fluXis.Game.Utils;

public static class InputUtils
{
    public static string GetReadableCombination(KeyCombination combo)
    {
        var result = string.Empty;
        if (combo.Keys.Contains(InputKey.Control))
            result += "Ctrl";
        if (combo.Keys.Contains(InputKey.LControl))
            result += "LeftCtrl";
        if (combo.Keys.Contains(InputKey.RControl))
            result += "RightCtrl";

        if (combo.Keys.Contains(InputKey.Shift))
            result += "Shift";
        if (combo.Keys.Contains(InputKey.LShift))
            result += "LeftShift";
        if (combo.Keys.Contains(InputKey.RShift))
            result += "RightShift";

        if (combo.Keys.Contains(InputKey.Alt))
            result += "Alt";
        if (combo.Keys.Contains(InputKey.LAlt))
            result += "LeftAlt";
        if (combo.Keys.Contains(InputKey.RAlt))
            result += "RightAlt";

        if (combo.Keys.Contains(InputKey.Super))
            result += "Super";
        if (combo.Keys.Contains(InputKey.LSuper))
            result += "LeftSuper";
        if (combo.Keys.Contains(InputKey.RSuper))
            result += "RightSuper";

        foreach (var inputKey in combo.Keys.Where(key => key >= InputKey.F1))
        {
            if (inputKey is InputKey.LControl or InputKey.RControl
                or InputKey.LShift or InputKey.RShift
                or InputKey.LAlt or InputKey.RAlt
                or InputKey.LSuper or InputKey.RSuper) continue;

            if (!result.EndsWith(" ") && !string.IsNullOrEmpty(result))
                result += " + ";

            result += $"{GetKeyShort(inputKey)}";
        }

        return result;
    }

    public static string GetKeyShort(InputKey key)
    {
        return key switch
        {
            InputKey.Escape => "Esc",
            InputKey.Keypad0 => "KP0",
            InputKey.Keypad1 => "KP1",
            InputKey.Keypad2 => "KP2",
            InputKey.Keypad3 => "KP3",
            InputKey.Keypad4 => "KP4",
            InputKey.Keypad5 => "KP5",
            InputKey.Keypad6 => "KP6",
            InputKey.Keypad7 => "KP7",
            InputKey.Keypad8 => "KP8",
            InputKey.Keypad9 => "KP9",
            InputKey.KeypadDivide => "KP/",
            InputKey.KeypadMultiply => "KP*",
            InputKey.KeypadSubtract => "KP-",
            InputKey.KeypadAdd => "KP+",
            InputKey.KeypadDecimal => "KP.",
            InputKey.KeypadEnter => "KPEnter",
            InputKey.Number0 => "0",
            InputKey.Number1 => "1",
            InputKey.Number2 => "2",
            InputKey.Number3 => "3",
            InputKey.Number4 => "4",
            InputKey.Number5 => "5",
            InputKey.Number6 => "6",
            InputKey.Number7 => "7",
            InputKey.Number8 => "8",
            InputKey.Number9 => "9",
            InputKey.Tilde => "~",
            InputKey.Minus => "-",
            InputKey.Plus => "+",
            InputKey.BracketLeft => "[",
            InputKey.BracketRight => "]",
            InputKey.Semicolon => ";",
            InputKey.Quote => "\"",
            InputKey.Comma => ",",
            InputKey.Period => ".",
            InputKey.Slash => "/",
            InputKey.BackSlash => @"\",
            InputKey.NonUSBackSlash => @"\",
            InputKey.FirstMouseButton => "M1",
            InputKey.MouseMiddle => "M3",
            InputKey.MouseRight => "M2",
            InputKey.ExtraMouseButton1 => "M4",
            InputKey.ExtraMouseButton2 => "M5",
            InputKey.ExtraMouseButton3 => "M6",
            InputKey.ExtraMouseButton4 => "M7",
            InputKey.ExtraMouseButton5 => "M8",
            InputKey.ExtraMouseButton6 => "M9",
            InputKey.ExtraMouseButton7 => "M10",
            InputKey.ExtraMouseButton8 => "M11",
            InputKey.ExtraMouseButton9 => "M12",
            _ => key.ToString()
        };
    }
}
