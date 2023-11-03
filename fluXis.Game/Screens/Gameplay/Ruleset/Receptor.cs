using fluXis.Game.Map.Events;
using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.Ruleset;

public partial class Receptor : CompositeDrawable
{
    [Resolved]
    private SkinManager skinManager { get; set; }

    [Resolved]
    private GameplayScreen screen { get; set; }

    [Resolved]
    private Playfield playfield { get; set; }

    private readonly int idx;

    private Drawable up;
    private Drawable down;
    private Drawable hitLighting;

    private int currentKeyCount;
    private bool visible;

    public bool IsDown;

    public Receptor(int idx)
    {
        this.idx = idx;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        currentKeyCount = playfield.Map.InitialKeyCount;

        RelativeSizeAxes = Axes.Y;
        Masking = true;

        InternalChildren = new[]
        {
            up = skinManager.GetReceptor(idx + 1, playfield.RealmMap.KeyCount, false),
            down = skinManager.GetReceptor(idx + 1, playfield.RealmMap.KeyCount, true),
            hitLighting = skinManager.GetColumnLighting(idx + 1, playfield.RealmMap.KeyCount)
        };

        hitLighting.Margin = new MarginPadding
        {
            Bottom = skinManager.SkinJson.GetKeymode(playfield.RealmMap.KeyCount).HitPosition
        };
    }

    protected override void LoadComplete()
    {
        updateKeyCount(true);
    }

    protected override void Update()
    {
        if (!playfield.Manager.AutoPlay)
            IsDown = screen.Input.Pressed[idx];

        up.Alpha = IsDown ? 0 : 1;
        down.Alpha = IsDown ? 1 : 0;

        if (visible)
            hitLighting.FadeTo(IsDown ? 0.5f : 0, IsDown ? 0 : 100);

        if (currentKeyCount != playfield.Manager.CurrentKeyCount)
        {
            currentKeyCount = playfield.Manager.CurrentKeyCount;
            updateKeyCount(false);
        }
    }

    private void updateKeyCount(bool instant)
    {
        // Since the current count is the same as the maximum,
        // every receptor should be visible
        if (currentKeyCount == playfield.RealmMap.KeyCount)
            visible = true;
        else
        {
            bool[][] mode = LaneSwitchEvent.SWITCH_VISIBILITY[playfield.RealmMap.KeyCount - 2];
            bool[] current = mode[currentKeyCount - 1];
            visible = current[idx];
        }

        var width = visible ? skinManager.SkinJson.GetKeymode(currentKeyCount).ColumnWidth : 0;

        if (instant)
        {
            FinishTransforms();
            Width = width;
        }
        else
        {
            float duration = playfield.Manager.CurrentLaneSwitchEvent?.Speed ?? 200;
            this.ResizeWidthTo(width, duration, Easing.OutQuint);
        }
    }
}
