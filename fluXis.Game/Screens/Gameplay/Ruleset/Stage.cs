using fluXis.Game.Skinning;
using fluXis.Game.Skinning.Default.Stage;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.Ruleset;

public partial class Stage : Container
{
    [Resolved]
    private SkinManager skinManager { get; set; }

    private const int lane_margin = 0;

    private readonly Playfield playfield;

    private int currentKeyCount;

    public Stage(Playfield playfield)
    {
        this.playfield = playfield;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Width = skinManager.CurrentSkin.ColumnWidth * playfield.Map.InitialKeyCount + lane_margin * 2;

        currentKeyCount = playfield.Map.InitialKeyCount;

        AddRangeInternal(new Drawable[]
        {
            new DefaultStageBackground
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both
            },
            new DefaultStageBorderLeft
            {
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopRight
            },
            new DefaultStageBorderRight
            {
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopLeft
            }
        });
    }

    protected override void Update()
    {
        if (currentKeyCount != playfield.Manager.CurrentKeyCount)
        {
            var currentEvent = playfield.Manager.CurrentLaneSwitchEvent;
            currentKeyCount = currentEvent.Count;
            this.ResizeWidthTo(skinManager.CurrentSkin.ColumnWidth * currentKeyCount + lane_margin * 2, currentEvent.Speed, Easing.OutQuint);
        }
    }
}
