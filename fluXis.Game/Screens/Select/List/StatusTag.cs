using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Logging;
using osuTK;

namespace fluXis.Game.Screens.Select.List;

public partial class StatusTag : CircularContainer
{
    private readonly RealmMapSet set;
    private RealmMap map => set.Maps.First();

    [Resolved]
    private FluXisRealm realm { get; set; }

    private FluXisSpriteText text;

    public StatusTag(RealmMapSet set)
    {
        this.set = set;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 100;
        Height = 20;
        Masking = true;
        EdgeEffect = new EdgeEffectParameters
        {
            Type = EdgeEffectType.Shadow,
            Colour = Colour4.Black.Opacity(.25f),
            Radius = 5,
            Offset = new Vector2(0, 2)
        };

        var (status, colour) = getStatus();

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = colour
            },
            text = new FluXisSpriteText
            {
                Text = status,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = FluXisColors.IsBright(colour) ? Colour4.Black : Colour4.White,
                Alpha = .75f
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        var widthLeft = 50 - text.DrawWidth % 50;

        if (widthLeft < 20)
            widthLeft += 50;

        Width = text.DrawWidth + widthLeft;
    }

    private (string, Colour4) getStatus()
    {
        var color = FluXisColors.GetStatusColor(map.Status);

        switch (map.Status)
        {
            case < 100:
                return map.Status switch
                {
                    -2 => ("LOCAL", color),
                    -1 => ("UNSUBMITTED", color), // blacklisted, but we show it as "unsubmitted"
                    0 => ("UNSUBMITTED", color),
                    1 => ("PENDING", color),
                    2 => ("IMPURE", color),
                    3 => ("PURE", color),
                    _ => ("UNKNOWN", color)
                };

            case >= 100:
                return realm.Run(r =>
                {
                    var info = r.All<ImporterInfo>().FirstOrDefault(i => i.Id == map.Status);

                    if (info != null)
                        return (info.Name, Colour4.TryParseHex(info.Color, out var c) ? c : color);

                    Logger.Log($"ImporterInfo with id {map.Status} not found!", level: LogLevel.Error);

                    return ("UNKNOWN", color);
                });
        }
    }
}
