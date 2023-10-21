using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.Ruleset;

public partial class Stage : Container
{
    [Resolved]
    private SkinManager skinManager { get; set; }

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
        Width = skinManager.Skin.GetKeymode(playfield.Map.InitialKeyCount).ColumnWidth * playfield.Map.InitialKeyCount;

        currentKeyCount = playfield.Map.InitialKeyCount;

        AddRangeInternal(new[]
        {
            skinManager.GetStageBackground(),
            skinManager.GetStageBorder(false),
            skinManager.GetStageBorder(true)
        });
    }

    protected override void Update()
    {
        if (currentKeyCount != playfield.Manager.CurrentKeyCount)
        {
            var currentEvent = playfield.Manager.CurrentLaneSwitchEvent;
            currentKeyCount = currentEvent.Count;
            this.ResizeWidthTo(skinManager.Skin.GetKeymode(playfield.Manager.CurrentKeyCount).ColumnWidth * playfield.Manager.CurrentKeyCount, currentEvent.Speed, Easing.OutQuint);
        }
    }
}
