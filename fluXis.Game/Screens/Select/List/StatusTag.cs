using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Logging;

namespace fluXis.Game.Screens.Select.List;

public partial class StatusTag : CircularContainer
{
    private readonly RealmMapSet set;
    private RealmMap map => set.Maps.First();

    [Resolved]
    private FluXisRealm realm { get; set; }

    private Box box;
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
        EdgeEffect = FluXisStyles.ShadowSmall;

        Children = new Drawable[]
        {
            box = new Box
            {
                RelativeSizeAxes = Axes.Both
            },
            text = new FluXisSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = .75f
            }
        };

        updateStatus();
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        set.StatusChanged += updateStatus;
        updateSize();
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        set.StatusChanged -= updateStatus;
    }

    private void updateStatus()
    {
        var (status, colour) = getStatus();

        text.Text = status;
        box.Colour = colour;
        text.Colour = FluXisColors.IsBright(colour) ? Colour4.Black : Colour4.White;

        updateSize();
    }

    private void updateSize()
    {
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
