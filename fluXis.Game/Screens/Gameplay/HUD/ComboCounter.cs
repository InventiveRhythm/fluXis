using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Gameplay.HUD;

public partial class ComboCounter : GameplayHUDElement
{
    private FluXisSpriteText text;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.X;
        Height = 64;
        Anchor = Anchor.Centre;
        Origin = Anchor.BottomCentre;

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

        Screen.ScoreProcessor.Combo.BindValueChanged(e =>
        {
            switch (e.NewValue)
            {
                case 0:
                    text.FadeColour(Colour4.Red).FadeColour(Colour4.White, 200).FadeOut(200).ScaleTo(1.4f, 300);
                    break;

                case >= 5:
                    text.Text = $"{e.NewValue}";
                    text.FadeColour(Colour4.White).FadeIn(400);
                    text.ScaleTo(1.05f).ScaleTo(1f, 200, Easing.OutQuint);
                    break;
            }
        }, true);
    }
}
