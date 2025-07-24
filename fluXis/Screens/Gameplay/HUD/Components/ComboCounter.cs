using fluXis.Audio;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Utils.Attributes;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Screens.Gameplay.HUD.Components;

public partial class ComboCounter : GameplayHUDComponent
{
    [Resolved]
    private IBeatSyncProvider beatSync { get; set; }

    private FluXisSpriteText text;

    [BindableSetting("Additive Scale", "Adds to the scale when combo changes.", "scale-additive")]
    public BindableBool ScaleAdditive { get; set; } = new(true);

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;

        InternalChild = text = new FluXisSpriteText
        {
            FontSize = 64,
            FixedWidth = true,
            Shadow = true,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Alpha = 0
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        ScoreProcessor.Combo.BindValueChanged(comboChanged, true);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        ScoreProcessor.Combo.ValueChanged -= comboChanged;
    }

    private void comboChanged(ValueChangedEvent<int> e)
    {
        switch (e.NewValue)
        {
            case 0:
                text.FadeColour(Theme.Red).FadeColour(Theme.Text, 200).FadeOut(200).ScaleTo(1.4f, 300);
                break;

            case >= 5:
                text.Text = $"{e.NewValue}";
                text.FadeColour(Theme.Text).FadeIn(400);
                text.ScaleTo(ScaleAdditive.Value ? text.Scale.X + .05f : 1.05f).ScaleTo(1f, beatSync.BeatTime * 2, Easing.OutQuint);
                break;
        }
    }
}
