using System;
using System.Linq;
using fluXis.Configuration;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Events;
using fluXis.Map.Structures.Events.Scrolling;
using fluXis.Mode.Keys.Gameplay.Objects;
using fluXis.Mode.Keys.Gameplay.UI;
using fluXis.Modes.Gameplay;
using fluXis.Screens.Gameplay.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;

namespace fluXis.Mode.Keys.Gameplay;

public partial class KeysPlayfield : Playfield
{
    [Resolved]
    private LaneSwitchManager laneSwitchManager { get; set; } = null!;

    public override bool IsFlipped => scrollDirection.Value == ScrollDirection.Up;
    public override bool IsFinished => Objects.Finished;

    public Stage Stage { get; private set; } = null!;
    public FillFlowContainer<Receptor> Receptors { get; private set; } = null!;
    public KeysHitObjectManager Objects { get; private set; } = null!;

    private Drawable hitline = null!;
    private Drawable topCover = null!;
    private Drawable bottomCover = null!;

    private Bindable<float> topCoverHeight = null!;
    private Bindable<float> bottomCoverHeight = null!;
    private Bindable<ScrollDirection> scrollDirection = null!;
    private Bindable<double> hitsoundPanStrength = null!;

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
            ChildrenEnumerable = Enumerable.Range(0, (Ruleset.PlayableMode as KeysPlayableGameMode)!.KeyCount).Select(i => new Receptor(i)),
            Padding = new MarginPadding { Bottom = Skin.SkinJson.GetKeymode((Ruleset.PlayableMode as KeysPlayableGameMode)!.KeyCount).ReceptorOffset }
        };

        Objects = new KeysHitObjectManager(Ruleset, Map, Map.ObjectsOfType<HitObject>());

        var receptorsFirst = Skin.SkinJson.GetKeymode((Ruleset.PlayableMode as KeysPlayableGameMode)!.KeyCount).ReceptorsFirst;

        AddRangeInternal([
            new LaneSwitchAlert(),
            Stage = new Stage(),
            receptorsFirst ? Receptors : Objects,
            receptorsFirst ? Objects : Receptors,
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

        Map.ObjectsOfType<TimeOffsetEvent>().ForEach(x => x.Apply(Objects));
    }

    public override Drawable? GetFadeLayer(LayerFadeEvent.FadeLayer layer) => layer switch
    {
        LayerFadeEvent.FadeLayer.HitObjects => Objects,
        LayerFadeEvent.FadeLayer.Stage => Stage,
        LayerFadeEvent.FadeLayer.Receptors => Receptors,
        _ => base.GetFadeLayer(layer)
    };

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
