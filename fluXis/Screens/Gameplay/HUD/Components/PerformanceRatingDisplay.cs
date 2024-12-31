using fluXis.Graphics.Sprites;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Screens.Gameplay.HUD.Components;

public partial class PerformanceRatingDisplay : GameplayHUDComponent
{
    private FluXisSpriteText text;
    private float pr = 0;

    private bool showSuffix;
    private bool showDecimals;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;

        showSuffix = Settings.GetSetting("suffix", true);
        showDecimals = Settings.GetSetting("decimals", false);

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
    {
        this.TransformTo(nameof(pr), e.NewValue, 400, Easing.OutQuint);
    }

    protected override void Update()
    {
        var format = showDecimals ? "00.00" : "00";
        text.Text = $"{pr.ToStringInvariant(format)}";

        if (showSuffix)
            text.Text += "pr";

        base.Update();
    }
}
