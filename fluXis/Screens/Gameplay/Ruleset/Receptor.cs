using fluXis.Screens.Gameplay.Ruleset.Playfields;
using fluXis.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Gameplay.Ruleset;

public partial class Receptor : CompositeDrawable
{
    [Resolved]
    private ISkin skin { get; set; }

    [Resolved]
    private RulesetContainer ruleset { get; set; }

    [Resolved]
    private Playfield playfield { get; set; }

    [Resolved]
    private LaneSwitchManager laneSwitchManager { get; set; }

    public override bool RemoveCompletedTransforms => true;

    private readonly int idx;

    private Drawable up;
    private Drawable down;
    private VisibilityContainer hitLighting;

    private BindableBool isDown { get; } = new();

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
            up = skin.GetReceptor(idx + 1, playfield.RealmMap.KeyCount, false),
            down = skin.GetReceptor(idx + 1, playfield.RealmMap.KeyCount, true),
            hitLighting = skin.GetColumnLighting(idx + 1, playfield.RealmMap.KeyCount).With(l => l.AlwaysPresent = true)
        };

        hitLighting.Margin = new MarginPadding
        {
            Bottom = skin.SkinJson.GetKeymode(playfield.RealmMap.KeyCount).HitPosition
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        isDown.BindValueChanged(v =>
        {
            if (v.NewValue)
            {
                up.Hide();
                down.Show();
                hitLighting.Show();
            }
            else
            {
                up.Show();
                down.Hide();
                hitLighting.Hide();
            }
        }, true);

        FinishTransforms(true);
    }

    protected override void Update()
    {
        var i = idx;

        if (playfield.Index > 0)
            i += playfield.RealmMap.KeyCount;

        isDown.Value = ruleset.Input.Pressed[i];
        Width = laneSwitchManager.WidthFor(idx + 1);
    }
}
