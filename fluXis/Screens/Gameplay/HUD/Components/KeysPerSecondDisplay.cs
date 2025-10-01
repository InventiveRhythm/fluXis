using System.Collections.Generic;
using fluXis.Graphics.Sprites.Text;
using fluXis.Input;
using fluXis.Utils.Attributes;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Screens.Gameplay.HUD.Components;

public partial class KeysPerSecondDisplay : GameplayHUDComponent
{
    [BindableSetting("Show Suffix", "Show the 'kps' suffix.", "suffix")]
    public BindableBool ShowSuffix { get; } = new(true);

    private FluXisSpriteText text;
    private readonly List<double> times = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;

        InternalChild = text = new FluXisSpriteText
        {
            FontSize = 32,
            Shadow = true
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        Ruleset.Input.OnPress += onPress;
    }

    protected override void Update()
    {
        base.Update();

        times.RemoveAll(x => x < Clock.CurrentTime - 1000);
        text.Text = $"{times.Count}{(ShowSuffix.Value ? " kps" : "")}";
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        Ruleset.Input.OnPress -= onPress;
    }

    private void onPress(FluXisGameplayKeybind _) => times.Add(Clock.CurrentTime);
}
