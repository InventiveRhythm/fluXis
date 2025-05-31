using fluXis.Graphics.Sprites.Text;
using fluXis.Utils;
using fluXis.Utils.Attributes;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Screens.Gameplay.HUD.Components;

public partial class PerformanceRatingDisplay : GameplayHUDComponent
{
    private FluXisSpriteText text;

    [UsedImplicitly]
    private float pr;

    [BindableSetting("Show Suffix", "Show the 'pr' suffix.", "suffix")]
    public BindableBool ShowSuffix { get; } = new(true);

    [BindableSetting("Show Decimals", "Show decimals in the performance rating.", "decimals")]
    public BindableBool ShowDecimals { get; } = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        pr = 0;

        InternalChild = text = new FluXisSpriteText
        {
            FontSize = 32,
            Shadow = true
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        ScoreProcessor.PerformanceRating.BindValueChanged(prChanged, true);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        ScoreProcessor.PerformanceRating.ValueChanged -= prChanged;
    }

    private void prChanged(ValueChangedEvent<float> e)
        => this.TransformTo(nameof(pr), e.NewValue, 400, Easing.OutQuint);

    protected override void Update()
    {
        var format = ShowDecimals.Value ? "00.00" : "00";
        text.Text = $"{pr.ToStringInvariant(format)}";

        if (ShowSuffix.Value)
            text.Text += "pr";

        base.Update();
    }
}
