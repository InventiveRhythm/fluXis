using System;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Skinning;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Screens.Gameplay.HUD.Components;

public partial class HealthBar : GameplayHUDComponent
{
    [Resolved]
    private ISkin skin { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private IBeatSyncProvider beatSync { get; set; }

    private Drawable bar;
    private SpriteIcon icon;

    private bool showingIcon;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;

        InternalChildren = new[]
        {
            skin.GetHealthBarBackground(),
            bar = skin.GetHealthBar(HealthProcessor),
            icon = new FluXisSpriteIcon
            {
                Size = new Vector2(24),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Icon = FontAwesome6.Solid.XMark,
                Alpha = 0
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (beatSync is not null)
            beatSync.OnBeat += onBeat;
    }

    protected override void Update()
    {
        if (!showingIcon && HealthProcessor.FailedAlready)
            showingIcon = true;

        var percent = HealthProcessor.SmoothHealth / 100;

        if (!float.IsFinite(percent))
            percent = 0;

        bar.Height = Math.Clamp(percent, 0, 1);

        base.Update();
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        if (beatSync is not null)
            beatSync.OnBeat += onBeat;
    }

    private void onBeat(int beat, bool finish)
    {
        if (!showingIcon || beat % 4 != 0)
            return;

        var duration = beatSync!.BeatTime;
        icon.FadeIn(duration * 1.5f).Then(duration).FadeOut(duration * 1.5f);
    }
}
