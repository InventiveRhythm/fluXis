using System;
using System.Linq;
using fluXis.Configuration;
using fluXis.Modes.Keys.HitObjects;
using fluXis.Modes.Keys.TimingLines;
using fluXis.Modes.Keys.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;

namespace fluXis.Modes.Keys;

public partial class KeysPlayfield : Playfield
{
    [Resolved]
    private LaneSwitchManager laneSwitchManager { get; set; }

    public override bool IsFlipped => scrollDirection.Value == ScrollDirection.Up;
    public override bool IsFinished => HitManager.Finished;

    public Stage Stage { get; private set; }
    public FillFlowContainer<Receptor> Receptors { get; private set; }
    public HitObjectManager HitManager { get; private set; }

    private Drawable hitline;
    private Drawable topCover;
    private Drawable bottomCover;

    private Bindable<float> topCoverHeight;
    private Bindable<float> bottomCoverHeight;
    private Bindable<ScrollDirection> scrollDirection;
    private Bindable<double> hitsoundPanStrength;

    public KeysPlayfield(int playerIndex, int playfieldIndex)
        : base(playerIndex, playfieldIndex)
    {
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        AutoSizeAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;

        topCoverHeight = config.GetBindable<float>(FluXisSetting.LaneCoverTop);
        bottomCoverHeight = config.GetBindable<float>(FluXisSetting.LaneCoverBottom);
        scrollDirection = config.GetBindable<ScrollDirection>(FluXisSetting.ScrollDirection);
        hitsoundPanStrength = config.GetBindable<double>(FluXisSetting.HitsoundPanning);

        Receptors = new FillFlowContainer<Receptor>
        {
            AutoSizeAxes = Axes.X,
            RelativeSizeAxes = Axes.Y,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Direction = FillDirection.Horizontal,
            ChildrenEnumerable = Enumerable.Range(0, RealmMap.KeyCount).Select(i => new Receptor(i)),
            Padding = new MarginPadding { Bottom = Skin.SkinJson.GetKeymode(RealmMap.KeyCount).ReceptorOffset }
        };

        Dependencies.CacheAs(HitManager = new HitObjectManager
        {
            AlwaysPresent = true,
            Masking = true
        });

        var receptorsFirst = Skin.SkinJson.GetKeymode(RealmMap.KeyCount).ReceptorsFirst;

        AddRangeInternal([
            new LaneSwitchAlert(),
            Stage = new Stage(),
            new TimingLineManager(),

            receptorsFirst ? Receptors : HitManager,
            receptorsFirst ? HitManager : Receptors,

            hitline = Skin.GetHitLine().With(d =>
            {
                d.Width = 1;
                d.RelativeSizeAxes = Axes.X;
            }),
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Masking = true,
                Children = [topCover = Skin.GetLaneCover(false), bottomCover = Skin.GetLaneCover(true)]
            },
            new KeyOverlay()
        ]);

        MapEvents.TimeOffsetEvents.ForEach(e => e.Apply(HitManager));
    }

    protected override void Update()
    {
        base.Update();

        var newReceptorOffset = laneSwitchManager.ReceptorOffset;

        hitline.Y = -laneSwitchManager.HitPosition;
        if (!Precision.AlmostEquals(newReceptorOffset, Receptors.Padding.Bottom))
            Receptors.Padding = Receptors.Padding with { Bottom = newReceptorOffset };

        topCover.Y = (topCoverHeight.Value - 2f) / 2f;
        bottomCover.Y = (2f - bottomCoverHeight.Value) / 2f;

        if (!IsSubPlayfield)
            HitSounds.PlayfieldPanning.Value = Math.Clamp(RelativePosition * 2 - 1, -1, 1) * hitsoundPanStrength.Value;
    }
}
