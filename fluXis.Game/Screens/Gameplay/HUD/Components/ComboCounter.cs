using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Gameplay.HUD.Components;

public partial class ComboCounter : GameplayHUDComponent
{
    [Resolved]
    private IBeatSyncProvider beatSync { get; set; }

    private bool scaleAdditive;
    private FluXisSpriteText text;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;

        scaleAdditive = Settings.GetSetting("scale-additive", false);

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

        Screen.ScoreProcessor.Combo.BindValueChanged(comboChanged, true);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        Screen.ScoreProcessor.Combo.ValueChanged -= comboChanged;
    }

    private void comboChanged(ValueChangedEvent<int> e)
    {
        switch (e.NewValue)
        {
            case 0:
                text.FadeColour(Colour4.Red).FadeColour(Colour4.White, 200).FadeOut(200).ScaleTo(1.4f, 300);
                break;

            case >= 5:
                text.Text = $"{e.NewValue}";
                text.FadeColour(Colour4.White).FadeIn(400);
                text.ScaleTo(scaleAdditive ? text.Scale.X + .05f : 1.05f).ScaleTo(1f, beatSync.BeatTime * 2, Easing.OutQuint);
                break;
        }
    }
}
