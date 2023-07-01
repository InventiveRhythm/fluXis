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

    public Playfield Playfield;

    private readonly int id;

    private Drawable up;
    private Drawable down;
    private Drawable hitLighting;

    private int currentKeyCount;
    private bool visible;

    public bool IsDown;

    public Receptor(Playfield playfield, int id)
    {
        this.id = id;
        Playfield = playfield;
        currentKeyCount = playfield.Map.InitialKeyCount;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Origin = Anchor.BottomCentre;
        Anchor = Anchor.BottomCentre;
        RelativeSizeAxes = Axes.Y;
        Masking = true;

        InternalChildren = new[]
        {
            up = skinManager.GetReceptor(id + 1, Playfield.Map.KeyCount, false),
            down = skinManager.GetReceptor(id + 1, Playfield.Map.KeyCount, true),
            hitLighting = skinManager.GetColumLighing(id + 1, Playfield.Map.KeyCount)
        };

        hitLighting.Margin = new MarginPadding
        {
            Bottom = skinManager.CurrentSkin.GetKeymode(Playfield.Map.KeyCount).HitPosition
        };

        updateKeyCount(true);
    }

    protected override void Update()
    {
        if (!Playfield.Manager.AutoPlay)
            IsDown = Playfield.Screen.Input.Pressed[id];

        up.Alpha = IsDown ? 0 : 1;
        down.Alpha = IsDown ? 1 : 0;

        if (visible)
            hitLighting.FadeTo(IsDown ? 0.5f : 0, IsDown ? 0 : 100);

        if (currentKeyCount != Playfield.Manager.CurrentKeyCount)
        {
            currentKeyCount = Playfield.Manager.CurrentKeyCount;
            updateKeyCount(false);
        }
    }

    private void updateKeyCount(bool instant)
    {
        // Since the current count is the same as the maximum,
        // every receptor should be visible
        if (currentKeyCount == Playfield.Map.KeyCount)
            visible = true;
        else
        {
            bool[][] mode = LaneSwitchEvent.SWITCH_VISIBILITY[Playfield.Map.KeyCount - 2];
            bool[] current = mode[currentKeyCount - 1];
            visible = current[id];
        }

        var width = visible ? skinManager.CurrentSkin.GetKeymode(currentKeyCount).ColumnWidth : 0;

        if (instant)
        {
            this.ResizeWidthTo(width);
        }
        else
        {
            float duration = Playfield.Manager.CurrentLaneSwitchEvent?.Speed ?? 200;
            this.ResizeWidthTo(width, duration, Easing.OutQuint);
        }
    }
}
