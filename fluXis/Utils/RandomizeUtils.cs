using System;
using System.Linq;

namespace fluXis.Utils;

public static class RandomizeUtils
{
    public static string GenerateRandomString(int length, CharacterType characters = CharacterType.Letters)
    {
        var chars = getCharacters(characters);

        if (string.IsNullOrEmpty(chars))
            throw new Exception("No characters to generate string with.");

        var random = new Random();
        return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private static string getCharacters(CharacterType type)
    {
        var chars = "";

        // ReSharper disable StringLiteralTypo

        if (type.HasFlag(CharacterType.Uppercase))
            chars += "ABCDEFJHIJKLMNOPQRSTUVWXYZ";
        if (type.HasFlag(CharacterType.Lowercase))
            chars += "abcdefjhijklmnopqrstuvwxyz";
        if (type.HasFlag(CharacterType.Numbers))
            chars += "0123456789";
        if (type.HasFlag(CharacterType.Symbols))
            chars += "._-";

        // ReSharper restore StringLiteralTypo

        return chars;
    }
}

[Flags]
public enum CharacterType
{
    Uppercase = 1 << 0,
    Lowercase = 1 << 1,
    Numbers = 1 << 2,
    Symbols = 1 << 3,

    Letters = Uppercase | Lowercase,
    AllOfIt = Uppercase | Lowercase | Numbers | Symbols
}
