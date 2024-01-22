using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Graphics.Sprites;

public class FontAwesome6
{
    private static IconUsage get(int icon) => new((char)icon, "FontAwesome6");

    // I really dont want to add all of them
    // so these are just the ones being used so far
    public static class Solid
    {
        private static IconUsage getSolid(int icon) => get(icon).With(weight: "Solid");

        public static IconUsage AngleDoubleRight => getSolid(0xf101);
        public static IconUsage ArrowPointer => getSolid(0xf245);
        public static IconUsage BackwardStep => getSolid(0xf048);
        public static IconUsage Bars => getSolid(0xf0c9);
        public static IconUsage Bell => getSolid(0xf0f3);
        public static IconUsage Bomb => getSolid(0xf1e2);
        public static IconUsage Book => getSolid(0xf02d);
        public static IconUsage BoxOpen => getSolid(0xf49e);
        public static IconUsage Bug => getSolid(0xf188);
        public static IconUsage CandyCane => getSolid(0xf786);
        public static IconUsage ChartLine => getSolid(0xf201);
        public static IconUsage Check => getSolid(0xf00c);
        public static IconUsage ChevronDown => getSolid(0xf078);
        public static IconUsage ChevronLeft => getSolid(0xf053);
        public static IconUsage ChevronRight => getSolid(0xf054);
        public static IconUsage ChevronUp => getSolid(0xf077);
        public static IconUsage Circle => getSolid(0xf111);
        public static IconUsage Clock => getSolid(0xf017);
        public static IconUsage ComputerMouse => getSolid(0xf8cc);
        public static IconUsage Copy => getSolid(0xf0c5);
        public static IconUsage Cube => getSolid(0xf1b2);
        public static IconUsage Cubes => getSolid(0xf1b3);
        public static IconUsage Cut => getSolid(0xf0c4);
        public static IconUsage Diamond => getSolid(0xf219);
        public static IconUsage Display => getSolid(0xe163);
        public static IconUsage DoorOpen => getSolid(0xf52b);
        public static IconUsage Download => getSolid(0xf019);
        public static IconUsage Drum => getSolid(0xf569);
        public static IconUsage EarthAmericas => getSolid(0xf57d);
        public static IconUsage Eraser => getSolid(0xf12d);
        public static IconUsage Eye => getSolid(0xf06e);
        public static IconUsage File => getSolid(0xf15b);
        public static IconUsage Film => getSolid(0xf008);
        public static IconUsage FloppyDisk => getSolid(0xf0c7);
        public static IconUsage Folder => getSolid(0xf07b);
        public static IconUsage FolderOpen => getSolid(0xf07c);
        public static IconUsage ForwardStep => getSolid(0xf051);
        public static IconUsage Gamepad => getSolid(0xf11b);
        public static IconUsage Gear => getSolid(0xf013);
        public static IconUsage HandsClapping => getSolid(0xe1a8);
        public static IconUsage HardDrive => getSolid(0xf0a0);
        public static IconUsage Hashtag => getSolid(0x23);
        public static IconUsage Headphones => getSolid(0xf025);
        public static IconUsage House => getSolid(0xf015);
        public static IconUsage Image => getSolid(0xf03e);
        public static IconUsage Images => getSolid(0xf302);
        public static IconUsage Info => getSolid(0xf129);
        public static IconUsage Keyboard => getSolid(0xf11c);
        public static IconUsage LayerGroup => getSolid(0xf5fd);
        public static IconUsage LeftRight => getSolid(0xf337);
        public static IconUsage Magnet => getSolid(0xf076);
        public static IconUsage MagnifyingGlass => getSolid(0xf002);
        public static IconUsage Map => getSolid(0xf279);
        public static IconUsage Message => getSolid(0xf27a);
        public static IconUsage Minus => getSolid(0xf068);
        public static IconUsage Music => getSolid(0xf001);
        public static IconUsage Newspaper => getSolid(0xf1ea);
        public static IconUsage ObjectGroup => getSolid(0xf247);
        public static IconUsage PaintBrush => getSolid(0xf1fc);
        public static IconUsage Palette => getSolid(0xf53f);
        public static IconUsage Paste => getSolid(0xf0ea);
        public static IconUsage Pause => getSolid(0xf04c);
        public static IconUsage Pen => getSolid(0xf304);
        public static IconUsage PenRuler => getSolid(0xf5ae);
        public static IconUsage Percent => getSolid(0x25);
        public static IconUsage Plane => getSolid(0xf072);
        public static IconUsage Play => getSolid(0xf04b);
        public static IconUsage Plug => getSolid(0xf1e6);
        public static IconUsage Plus => getSolid(0x2b);
        public static IconUsage PuzzlePiece => getSolid(0xf12e);
        public static IconUsage Question => getSolid(0x3f);
        public static IconUsage RightLeft => getSolid(0xf0c7);
        public static IconUsage Rotate => getSolid(0xf2f1);
        public static IconUsage RotateLeft => getSolid(0xf2ea);
        public static IconUsage RotateRight => getSolid(0xf2f9);
        public static IconUsage ScrewdriverWrench => getSolid(0xf7d9);
        public static IconUsage Shapes => getSolid(0xf61f);
        public static IconUsage ShareNodes => getSolid(0xf1e0);
        public static IconUsage ShieldHalved => getSolid(0xf3ed);
        public static IconUsage Shuffle => getSolid(0xf074);
        public static IconUsage Skull => getSolid(0xf54c);
        public static IconUsage Star => getSolid(0xf005);
        public static IconUsage Stopwatch => getSolid(0xf2f2);
        public static IconUsage ThumbsUp => getSolid(0xf164);
        public static IconUsage Trash => getSolid(0xf1f8);
        public static IconUsage TriangleExclamation => getSolid(0xf071);
        public static IconUsage Trophy => getSolid(0xf091);
        public static IconUsage UpDown => getSolid(0xf338);
        public static IconUsage Upload => getSolid(0xf093);
        public static IconUsage User => getSolid(0xf007);
        public static IconUsage UserAstronaut => getSolid(0xf4fb);
        public static IconUsage UserGroup => getSolid(0xf500);
        public static IconUsage UserPlus => getSolid(0xf234);
        public static IconUsage UserShield => getSolid(0xf505);
        public static IconUsage Users => getSolid(0xf0c0);
        public static IconUsage VolumeHigh => getSolid(0xf028);
        public static IconUsage WandMagicSparkles => getSolid(0xe2ca);
        public static IconUsage WindowRestore => getSolid(0xf2d2);
        public static IconUsage WineGlassEmpty => getSolid(0xf5ce);
        public static IconUsage XMark => getSolid(0xf00d);
    }

    public static class Regular
    {
        private static IconUsage getRegular(int icon) => get(icon).With(weight: "Regular");
    }

    public static class Brands
    {
        private static IconUsage getBrands(int icon) => get(icon).With(weight: "Brands");

        public static IconUsage Discord => getBrands(0xf392);
        public static IconUsage GitHub => getBrands(0xf09b);
    }
}
