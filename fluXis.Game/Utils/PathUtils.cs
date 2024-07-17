using System.IO;
using fluXis.Game.Graphics.Sprites;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Utils;

public static class PathUtils
{
    public static string HashToPath(string hash)
    {
        if (hash.Length < 3) return hash;

        return hash[..1] + "/" + hash[..2] + "/" + hash;
    }

    public static bool IsValidDirectory(string path)
    {
        try
        {
            return Directory.Exists(path);
        }
        catch
        {
            return false;
        }
    }

    public static string RemoveAllInvalidPathCharacters(string path)
    {
        path = string.Concat(path.Split(Path.GetInvalidPathChars()));
        return string.Concat(path.Split(Path.GetInvalidFileNameChars()));
    }

    public static IconUsage GetIconForType(FileType type)
    {
        return type switch
        {
            FileType.Folder => FontAwesome6.Solid.Folder,
            FileType.Drive => FontAwesome6.Solid.HardDrive,
            FileType.Audio => FontAwesome6.Solid.Music,
            FileType.Image => FontAwesome6.Solid.Image,
            FileType.Video => FontAwesome6.Solid.Film,
            FileType.Map => FontAwesome6.Solid.Map,
            FileType.Skin => FontAwesome6.Solid.PaintBrush,
            FileType.Unknown => FontAwesome6.Solid.File,
            _ => FontAwesome6.Solid.Question
        };
    }

    public static Colour4 GetColorForType(FileType type)
    {
        return type switch
        {
            FileType.Folder => Colour4.FromHSL(40 / 360f, .8f, .8f),
            FileType.Audio => Colour4.FromHSL(200 / 360f, .8f, .8f),
            FileType.Image => Colour4.FromHSL(120 / 360f, .8f, .8f),
            FileType.Video => Colour4.FromHSL(280 / 360f, .8f, .8f),
            FileType.Map => Colour4.FromHSL(20 / 360f, .8f, .8f),
            FileType.Skin => Colour4.FromHSL(320 / 360f, .8f, .8f),
            FileType.Unknown => Colour4.FromHSL(0 / 360f, .8f, .8f),
            _ => Colour4.Yellow
        };
    }

    public static FileType GetTypeForExtension(string extension)
    {
        extension = extension.ToLowerInvariant();
        extension = extension.TrimStart('.');

        return extension switch
        {
            "mp3" => FileType.Audio,
            "wav" => FileType.Audio,
            "ogg" => FileType.Audio,

            "jpg" => FileType.Image,
            "jpeg" => FileType.Image,
            "png" => FileType.Image,

            "mp4" => FileType.Video,
            "mov" => FileType.Video,
            "avi" => FileType.Video,
            "flv" => FileType.Video,
            "mpg" => FileType.Video,
            "wmv" => FileType.Video,
            "m4v" => FileType.Video,

            "fms" => FileType.Map,
            "osz" => FileType.Map,
            "qp" => FileType.Map,
            "sm" => FileType.Map,

            "fsk" => FileType.Skin,

            _ => FileType.Unknown
        };
    }

    public enum FileType
    {
        Folder,
        Drive,

        Audio,
        Image,
        Video,
        Map,
        Skin,

        Unknown
    }
}
