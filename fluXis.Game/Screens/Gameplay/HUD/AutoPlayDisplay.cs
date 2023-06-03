using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Gameplay.HUD;

public partial class AutoPlayDisplay : GameplayHUDElement
{
    public AutoPlayDisplay(GameplayScreen screen)
        : base(screen)
    {
    }

    private FluXisSpriteText text;
    private readonly Bindable<bool> autoPlay = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.TopCentre;
        Origin = Anchor.TopCentre;
        Y = 80;
        Height = 64;

        Add(text = new FluXisSpriteText
        {
            FontSize = 32,
            Anchor = Anchor.TopCentre,
            Origin = Anchor.TopCentre,
            Text = "AutoPlay",
            Alpha = 0
        });

        autoPlay.ValueChanged += onAutoPlayChanged;
    }

    protected override void Update()
    {
        if (Screen.Playfield.Manager.AutoPlay != autoPlay.Value)
            autoPlay.Value = Screen.Playfield.Manager.AutoPlay;
    }

    private void onAutoPlayChanged(ValueChangedEvent<bool> v)
    {
        if (v.NewValue)
            text.FadeIn(400).Then(400).FadeOut(400).Loop();
        else
            text.FadeOut(100);
    }
}
