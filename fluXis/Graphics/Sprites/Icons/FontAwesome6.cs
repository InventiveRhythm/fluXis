using osu.Framework.Graphics.Sprites;

namespace fluXis.Graphics.Sprites.Icons;

public class FontAwesome6
{
    private static IconUsage get(int icon) => new((char)icon, "FontAwesome6");

    // I really don't want to add all of them
    // so these are just the ones being used so far
    public static class Solid
    {
        public static IconUsage GetSolid(int icon) => get(icon).With(weight: "Solid");

        public static IconUsage AngleDoubleRight => GetSolid(0xf101);
        public static IconUsage AngleDown => GetSolid(0xf107);
        public static IconUsage AngleLeft => GetSolid(0xf104);
        public static IconUsage AngleRight => GetSolid(0xf105);
        public static IconUsage AngleUp => GetSolid(0xf106);
        public static IconUsage Aperture => GetSolid(0xe2df);
        public static IconUsage ArrowDown => GetSolid(0xf063);
        public static IconUsage ArrowDownToLine => GetSolid(0xf33d);
        public static IconUsage ArrowPointer => GetSolid(0xf245);
        public static IconUsage ArrowRight => GetSolid(0xf061);
        public static IconUsage ArrowRightArrowLeft => GetSolid(0xf0ec);
        public static IconUsage ArrowTrendUp => GetSolid(0xe098);
        public static IconUsage ArrowsRotate => GetSolid(0xf021);
        public static IconUsage ArrowsToLine => GetSolid(0xe0a7);
        public static IconUsage BackwardStep => GetSolid(0xf048);
        public static IconUsage Bars => GetSolid(0xf0c9);
        public static IconUsage Bell => GetSolid(0xf0f3);
        public static IconUsage Bomb => GetSolid(0xf1e2);
        public static IconUsage Book => GetSolid(0xf02d);
        public static IconUsage BoxOpen => GetSolid(0xf49e);
        public static IconUsage Bug => GetSolid(0xf188);
        public static IconUsage BullseyeArrow => GetSolid(0xf648);
        public static IconUsage CandyCane => GetSolid(0xf786);
        public static IconUsage ChartLine => GetSolid(0xf201);
        public static IconUsage Check => GetSolid(0xf00c);
        public static IconUsage Circle => GetSolid(0xf111);
        public static IconUsage CircleNodes => GetSolid(0xe4e2);
        public static IconUsage Clock => GetSolid(0xf017);
        public static IconUsage Clone => GetSolid(0xf24d);
        public static IconUsage ComputerMouse => GetSolid(0xf8cc);
        public static IconUsage Copy => GetSolid(0xf0c5);
        public static IconUsage Crown => GetSolid(0xf521);
        public static IconUsage Cube => GetSolid(0xf1b2);
        public static IconUsage Cubes => GetSolid(0xf1b3);
        public static IconUsage Cut => GetSolid(0xf0c4);
        public static IconUsage Diamond => GetSolid(0xf219);
        public static IconUsage Display => GetSolid(0xe163);
        public static IconUsage DoorOpen => GetSolid(0xf52b);
        public static IconUsage DownLeftAndUpRightToCenter => GetSolid(0xf422);
        public static IconUsage Download => GetSolid(0xf019);
        public static IconUsage Drum => GetSolid(0xf569);
        public static IconUsage EarthAmericas => GetSolid(0xf57d);
        public static IconUsage EllipsisVertical => GetSolid(0xf142);
        public static IconUsage Eraser => GetSolid(0xf12d);
        public static IconUsage ExclamationTriangle => GetSolid(0xf071);
        public static IconUsage Eye => GetSolid(0xf06e);
        public static IconUsage File => GetSolid(0xf15b);
        public static IconUsage Film => GetSolid(0xf008);
        public static IconUsage Flag => GetSolid(0xf024);
        public static IconUsage Flask => GetSolid(0xf0c3);
        public static IconUsage FloppyDisk => GetSolid(0xf0c7);
        public static IconUsage Folder => GetSolid(0xf07b);
        public static IconUsage FolderOpen => GetSolid(0xf07c);
        public static IconUsage ForwardStep => GetSolid(0xf051);
        public static IconUsage Gamepad => GetSolid(0xf11b);
        public static IconUsage Gear => GetSolid(0xf013);
        public static IconUsage HandsClapping => GetSolid(0xe1a8);
        public static IconUsage HardDrive => GetSolid(0xf0a0);
        public static IconUsage Hashtag => GetSolid(0x23);
        public static IconUsage Headphones => GetSolid(0xf025);
        public static IconUsage Heart => GetSolid(0xf004);
        public static IconUsage HeartCrack => GetSolid(0xf7a9);
        public static IconUsage House => GetSolid(0xf015);
        public static IconUsage Image => GetSolid(0xf03e);
        public static IconUsage Images => GetSolid(0xf302);
        public static IconUsage Info => GetSolid(0xf129);
        public static IconUsage Keyboard => GetSolid(0xf11c);
        public static IconUsage Clipboard => GetSolid(0xf328);
        public static IconUsage LayerGroup => GetSolid(0xf5fd);
        public static IconUsage LeftRight => GetSolid(0xf337);
        public static IconUsage Link => GetSolid(0xf0c1);
        public static IconUsage ListMusic => GetSolid(0xf8c9);
        public static IconUsage Lock => GetSolid(0xf023);
        public static IconUsage Magnet => GetSolid(0xf076);
        public static IconUsage MagnifyingGlass => GetSolid(0xf002);
        public static IconUsage Map => GetSolid(0xf279);
        public static IconUsage Message => GetSolid(0xf27a);
        public static IconUsage Minus => GetSolid(0xf068);
        public static IconUsage Music => GetSolid(0xf001);
        public static IconUsage Newspaper => GetSolid(0xf1ea);
        public static IconUsage ObjectGroup => GetSolid(0xf247);
        public static IconUsage PaintBrush => GetSolid(0xf1fc);
        public static IconUsage Palette => GetSolid(0xf53f);
        public static IconUsage Paste => GetSolid(0xf0ea);
        public static IconUsage Pause => GetSolid(0xf04c);
        public static IconUsage Pen => GetSolid(0xf304);
        public static IconUsage PenRuler => GetSolid(0xf5ae);
        public static IconUsage Pencil => GetSolid(0xf303);
        public static IconUsage Percent => GetSolid(0x25);
        public static IconUsage PersonToDoor => GetSolid(0xe433);
        public static IconUsage Plane => GetSolid(0xf072);
        public static IconUsage Play => GetSolid(0xf04b);
        public static IconUsage Plug => GetSolid(0xf1e6);
        public static IconUsage PlugCircleXMark => GetSolid(0xe560);
        public static IconUsage Plus => GetSolid(0x2b);
        public static IconUsage PuzzlePiece => GetSolid(0xf12e);
        public static IconUsage Question => GetSolid(0x3f);
        public static IconUsage RankingStar => GetSolid(0xe561);
        public static IconUsage RectangleWide => GetSolid(0xf2fc);
        public static IconUsage RightLeft => GetSolid(0xf362);
        public static IconUsage Rotate => GetSolid(0xf2f1);
        public static IconUsage RotateLeft => GetSolid(0xf2ea);
        public static IconUsage RotateRight => GetSolid(0xf2f9);
        public static IconUsage ScrewdriverWrench => GetSolid(0xf7d9);
        public static IconUsage Shapes => GetSolid(0xf61f);
        public static IconUsage ShareNodes => GetSolid(0xf1e0);
        public static IconUsage ShieldHalved => GetSolid(0xf3ed);
        public static IconUsage Shuffle => GetSolid(0xf074);
        public static IconUsage Skull => GetSolid(0xf54c);
        public static IconUsage SquareCheck => GetSolid(0xf14a);
        public static IconUsage Star => GetSolid(0xf005);
        public static IconUsage Stopwatch => GetSolid(0xf2f2);
        public static IconUsage ThumbsUp => GetSolid(0xf164);
        public static IconUsage Trash => GetSolid(0xf1f8);
        public static IconUsage TriangleExclamation => GetSolid(0xf071);
        public static IconUsage Trophy => GetSolid(0xf091);
        public static IconUsage UpDown => GetSolid(0xf338);
        public static IconUsage UpRightAndDownLeftFromCenter => GetSolid(0xf424);
        public static IconUsage Upload => GetSolid(0xf093);
        public static IconUsage User => GetSolid(0xf007);
        public static IconUsage UserAstronaut => GetSolid(0xf4fb);
        public static IconUsage UserGroup => GetSolid(0xf500);
        public static IconUsage UserMinus => GetSolid(0xf503);
        public static IconUsage UserPlus => GetSolid(0xf234);
        public static IconUsage UserShield => GetSolid(0xf505);
        public static IconUsage Users => GetSolid(0xf0c0);
        public static IconUsage VolumeHigh => GetSolid(0xf028);
        public static IconUsage WandMagicSparkles => GetSolid(0xe2ca);
        public static IconUsage WaveformLines => GetSolid(0xf8f2);
        public static IconUsage WindowRestore => GetSolid(0xf2d2);
        public static IconUsage WineGlassEmpty => GetSolid(0xf5ce);
        public static IconUsage XMark => GetSolid(0xf00d);
    }

    public static class Regular
    {
        public static IconUsage GetRegular(int icon) => get(icon).With(weight: "Regular");

        public static IconUsage Circle => GetRegular(0xf111);
        public static IconUsage Clock => GetRegular(0xf017);
        public static IconUsage Heart => GetRegular(0xf004);
    }

    public static class Brands
    {
        private static IconUsage getBrands(int icon) => get(icon).With(weight: "Brands");

        public static IconUsage Discord => getBrands(0xf392);
        public static IconUsage GitHub => getBrands(0xf09b);
        public static IconUsage Steam => getBrands(0xf1b6);
    }
}
