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
using osuTK;

namespace fluXis.Game.Screens.Select.List;

public partial class StatusTag : CircularContainer
{
    private RealmMapSet set { get; }
    private RealmMap map { get; }

    [Resolved]
    private FluXisRealm realm { get; set; }

    public float WebFontSize { set => FontSize = FluXisSpriteText.GetWebFontSize(value); }
    public float FontSize { get; set; } = 20;
    public float WidthStep { get; set; } = 35;

    private Box box;
    private FluXisSpriteText text;

    public StatusTag(RealmMapSet set)
    {
        this.set = set;
        map = set.Maps.First();
        Size = new Vector2(100, 20);
        EdgeEffect = FluXisStyles.ShadowSmall;
    }

    public StatusTag(RealmMap map)
        : this(map.MapSet)
    {
        this.map = map;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Masking = true;

        Children = new Drawable[]
        {
            box = new Box
            {
                RelativeSizeAxes = Axes.Both
            },
            text = new FluXisSpriteText
            {
                FontSize = FontSize,
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
        var widthLeft = WidthStep - text.DrawWidth % WidthStep;

        if (widthLeft < 20)
            widthLeft += WidthStep;

        Width = text.DrawWidth + widthLeft;
    }

    private (string, Colour4) getStatus()
    {
        var color = FluXisColors.GetStatusColor(map.StatusInt);

        switch (map.StatusInt)
        {
            case < 100:
                return map.StatusInt switch
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
                    var info = r.All<ImporterInfo>().FirstOrDefault(i => i.Id == map.StatusInt);

                    if (info != null)
                        return (info.Name, Colour4.TryParseHex(info.Color, out var c) ? c : color);

                    Logger.Log($"ImporterInfo with id {map.StatusInt} not found!", level: LogLevel.Error);

                    return ("UNKNOWN", color);
                });
        }
    }
}
