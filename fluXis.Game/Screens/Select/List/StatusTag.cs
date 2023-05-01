using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Select.List;

public partial class StatusTag : Container
{
    public StatusTag(RealmMapSet set)
    {
        AutoSizeAxes = Axes.Both;
        Anchor = Anchor.CentreRight;
        Origin = Anchor.CentreRight;

        RealmMap map = set.Maps[0];

        Colour4 colour = map.Status switch
        {
            -4 => Colour4.FromHex("#e7659f"),
            -3 => Colour4.FromHex("#0cb2d8"),
            -2 => Colour4.FromHex("#8fffc8"),
            -1 => Colour4.FromHex("#888888"),
            0 => Colour4.FromHex("#888888"),
            1 => Colour4.FromHex("#f7b373"),
            2 => Colour4.FromHex("#ff7b74"),
            3 => Colour4.FromHex("#55b2ff"),
            _ => Colour4.Black
        };

        Children = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Shear = new Vector2(.1f, 0),
                Masking = true,
                CornerRadius = 5,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colour
                },
                EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Shadow,
                    Colour = Colour4.Black.Opacity(0.2f),
                    Radius = 5,
                    Offset = new Vector2(0, 1)
                }
            },
            new SpriteText
            {
                Text = map.Status switch
                {
                    -4 => "osu!mania",
                    -3 => "QUAVER",
                    -2 => "LOCAL",
                    -1 => "UNSUBMITTED", // blacklisted, but we show it as "unsubmitted"
                    0 => "UNSUBMITTED",
                    1 => "PENDING",
                    2 => "IMPURE",
                    3 => "PURE",
                    _ => "UNKNOWN"
                },
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = colour.ToHSL().Z > 0.5f ? Colour4.FromHex("#1a1a20") : Colour4.White,
                Font = FluXisFont.Default(),
                Margin = new MarginPadding { Horizontal = 8 }
            }
        };
    }
}
