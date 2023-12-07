using System;
using fluXis.Game.Configuration;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Game.Graphics.Containers;

public partial class ParallaxContainer : Container
{
    public float Strength { get; set; } = .05f;

    protected override Container<Drawable> Content => content;
    private readonly Container content;

    private InputManager input;
    private Bindable<bool> parallaxEnabled;

    public ParallaxContainer()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Origin = Anchor.Centre;

        InternalChild = content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        };
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        parallaxEnabled = config.GetBindable<bool>(FluXisSetting.Parallax);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        input = GetContainingInputManager();
        parallaxEnabled.BindValueChanged(onChange, true);
    }

    protected override void Update()
    {
        base.Update();

        if (!parallaxEnabled.Value) return;
        if (input?.CurrentState.Mouse is null) return;

        var half = DrawSize / 2;
        var relative = ToLocalSpace(input.CurrentState.Mouse.Position) - half;

        relative.X = (float)(Math.Sign(relative.X) * Interpolation.Damp(0, 1, .999f, Math.Abs(relative.X)));
        relative.Y = (float)(Math.Sign(relative.Y) * Interpolation.Damp(0, 1, .999f, Math.Abs(relative.Y)));

        const int duration = 500;

        var elapsed = Math.Clamp(Clock.ElapsedFrameTime, 0, duration);
        content.Position = Interpolation.ValueAt(elapsed, content.Position, relative * half * Strength, 0, duration, Easing.Out);
        content.Scale = Interpolation.ValueAt(elapsed, content.Scale, new Vector2(1 + Math.Abs(Strength)), 0, duration, Easing.Out);
    }

    protected override void Dispose(bool isDisposing)
    {
        parallaxEnabled.ValueChanged -= onChange;
        base.Dispose(isDisposing);
    }

    private void onChange(ValueChangedEvent<bool> enabled)
    {
        this.MoveTo(Vector2.Zero, 400, Easing.OutQuint);
    }
}
