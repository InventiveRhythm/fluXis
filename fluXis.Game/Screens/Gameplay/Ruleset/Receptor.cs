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

    [Resolved]
    private LaneSwitchManager laneSwitchManager { get; set; }

    public override bool RemoveCompletedTransforms => true;

    private readonly int idx;

    private Drawable up;
    private Drawable down;
    private VisibilityContainer hitLighting;

    public bool IsDown;

    public Receptor(int idx)
    {
        this.idx = idx;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Y;
        Masking = true;

        InternalChildren = new[]
        {
            up = skinManager.GetReceptor(idx + 1, playfield.RealmMap.KeyCount, false),
            down = skinManager.GetReceptor(idx + 1, playfield.RealmMap.KeyCount, true),
            hitLighting = skinManager.GetColumnLighting(idx + 1, playfield.RealmMap.KeyCount).With(l => l.AlwaysPresent = true)
        };

        hitLighting.Margin = new MarginPadding
        {
            Bottom = skinManager.SkinJson.GetKeymode(playfield.RealmMap.KeyCount).HitPosition
        };
    }

    protected override void Update()
    {
        IsDown = screen.Input.Pressed[idx];

        up.Alpha = IsDown ? 0 : 1;
        down.Alpha = IsDown ? 1 : 0;

        if (IsDown)
            hitLighting.Show();
        else
            hitLighting.Hide();

        Width = laneSwitchManager.WidthFor(idx + 1);
    }
}
