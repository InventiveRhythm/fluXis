using fluXis.Modes.Gameplay;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Mode.Keys.Gameplay;

public partial class Receptor : CompositeDrawable
{
    [Resolved]
    private ISkin skin { get; set; } = null!;

    [Resolved]
    private RulesetContainer ruleset { get; set; } = null!;

    [Resolved]
    private KeysKeybindContainer keybinds { get; set; } = null!;

    [Resolved]
    private Playfield playfield { get; set; } = null!;

    [Resolved]
    private LaneSwitchManager laneSwitchManager { get; set; } = null!;

    public override bool RemoveCompletedTransforms => true;

    private readonly int idx;

    private Drawable up = null!;
    private Drawable down = null!;
    private VisibilityContainer hitLighting = null!;

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
            up = skin.GetReceptor(idx + 1, (ruleset.PlayableMode as KeysPlayableGameMode)!.KeyCount, false),
            down = skin.GetReceptor(idx + 1, (ruleset.PlayableMode as KeysPlayableGameMode)!.KeyCount, true),
            hitLighting = skin.GetColumnLighting(idx + 1, (ruleset.PlayableMode as KeysPlayableGameMode)!.KeyCount).With(l => l.AlwaysPresent = true)
        };

        hitLighting.Margin = new MarginPadding
        {
            Bottom = skin.SkinJson.GetKeymode((ruleset.PlayableMode as KeysPlayableGameMode)!.KeyCount).HitPosition
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

        if (playfield.PlayerIndex > 0)
            i += (ruleset.PlayableMode as KeysPlayableGameMode)!.KeyCount;

        isDown.Value = keybinds.PressedActions.Contains(keybinds.Keys[i]);
        Width = laneSwitchManager.WidthFor(idx + 1);
    }
}
