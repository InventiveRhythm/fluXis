using System;
using fluXis.Game.Configuration;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Game.Graphics.Containers;

public partial class ParallaxContainer : Container
{
    public float Strength
    {
        get => strength;
        set
        {
            strength = value;
            updatePosition();
        }
    }

    private Vector2 lastMousePos;
    private Bindable<bool> parallaxEnabled;
    private float strength = 10;

    public ParallaxContainer()
    {
        Anchor = Origin = Anchor.Centre;
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        parallaxEnabled = config.GetBindable<bool>(FluXisSetting.Parallax);
        parallaxEnabled.BindValueChanged(onChange, true);
    }

    protected override void Dispose(bool isDisposing)
    {
        parallaxEnabled.ValueChanged -= onChange;
        base.Dispose(isDisposing);
    }

    private void onChange(ValueChangedEvent<bool> enabled)
    {
        if (enabled.NewValue)
            updatePosition();
        else
            this.MoveTo(Vector2.Zero, 400, Easing.OutQuint);
    }

    protected override void Update()
    {
        updatePosition();
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        lastMousePos = ToLocalSpace(e.ScreenSpaceMousePosition);
        return false;
    }

    private Vector2 getParallaxPosition(Vector2 mousepos)
    {
        float x = (mousepos.X - DrawSize.X / 2) / (DrawSize.X / 2);
        float y = (mousepos.Y - DrawSize.Y / 2) / (DrawSize.Y / 2);
        return new Vector2(x * Strength, y * Strength);
    }

    private void updatePosition()
    {
        if ((!parallaxEnabled?.Value ?? false) || !IsLoaded)
            return;

        var pos = getParallaxPosition(lastMousePos);
        X = (float)Interpolation.Lerp(pos.X, X, Math.Exp(-0.01f * Time.Elapsed));
        Y = (float)Interpolation.Lerp(pos.Y, Y, Math.Exp(-0.01f * Time.Elapsed));
    }
}
