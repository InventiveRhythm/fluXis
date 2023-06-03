using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Logging;
using osuTK;

namespace fluXis.Game.Screens.Select.List;

public partial class StatusTag : Container
{
    private readonly RealmMapSet set;

    public StatusTag(RealmMapSet set)
    {
        this.set = set;
    }

    [BackgroundDependencyLoader]
    private void load(FluXisRealm realm)
    {
        AutoSizeAxes = Axes.Both;
        Anchor = Anchor.CentreRight;
        Origin = Anchor.CentreRight;

        RealmMap map = set.Maps[0];

        Colour4 colour = map.Status switch
        {
            -2 => Colour4.FromHex("#8fffc8"),
            -1 => Colour4.FromHex("#888888"),
            0 => Colour4.FromHex("#888888"),
            1 => Colour4.FromHex("#f7b373"),
            2 => Colour4.FromHex("#ff7b74"),
            3 => Colour4.FromHex("#55b2ff"),
            _ => Colour4.Black
        };

        string text = map.Status switch
        {
            -2 => "LOCAL",
            -1 => "UNSUBMITTED", // blacklisted, but we show it as "unsubmitted"
            0 => "UNSUBMITTED",
            1 => "PENDING",
            2 => "IMPURE",
            3 => "PURE",
            _ => "UNKNOWN"
        };

        if (map.Status >= 100)
        {
            realm.Run(r =>
            {
                var info = r.All<ImporterInfo>().FirstOrDefault(i => i.Id == map.Status);

                if (info != null)
                {
                    colour = Colour4.FromHex(info.Color);
                    text = info.Name;
                }
                else Logger.Log($"ImporterInfo with id {map.Status} not found!", level: LogLevel.Error);
            });
        }

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
            new FluXisSpriteText
            {
                Text = text,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = FluXisColors.IsBright(colour) ? FluXisColors.TextDark : Colour4.White,
                Margin = new MarginPadding { Horizontal = 8 }
            }
        };
    }
}
