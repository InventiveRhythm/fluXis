using System;
using System.Collections.Generic;
using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Map.Structures.Attributes;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Gameplay.Audio.Hitsounds;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Skinning;
using fluXis.Skinning.Default;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Modes;

public abstract partial class Playfield : CompositeDrawable
{
    [Resolved]
    protected RulesetContainer Ruleset { get; private set; }

    [Resolved]
    protected Hitsounding HitSounds { get; private set; }

    [Resolved]
    protected ISkin Skin { get; private set; }

    public override bool RemoveCompletedTransforms => false;

    public int PlayerIndex { get; }
    public int PlayfieldIndex { get; }
    public bool IsSubPlayfield => PlayfieldIndex > 0;

    public virtual bool IsFlipped => false;
    public abstract bool IsFinished { get; }

    protected new DependencyContainer Dependencies { get; private set; }

    public float RelativePosition
    {
        get
        {
            var screenWidth = Parent!.DrawWidth;
            return (X + screenWidth / 2) / screenWidth;
        }
    }

    public MapInfo MapInfo => Ruleset.MapInfo;
    public MapEvents MapEvents => Ruleset.MapEvents;
    public RealmMap RealmMap => MapInfo.RealmEntry!;

    public ColorManager ColorManager { get; private set; }
    public float HUDAlpha { get; set; } = 1f;

    private ShakeProxy shakeProxy;

    protected Playfield(int playerIndex, int playfieldIndex)
    {
        PlayerIndex = playerIndex;
        PlayfieldIndex = playfieldIndex;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        AlwaysPresent = true;
        Alpha = IsSubPlayfield ? 0 : 1;

        Dependencies.CacheAs(this);

        LoadComponent(ColorManager = new ColorManager());
        Dependencies.CacheAs<ICustomColorProvider>(ColorManager);

        AddRangeInternal([shakeProxy = new ShakeProxy(), ColorManager]);

        RegisterReloadableEvent(MapEvents.ColorFadeEvents);
        RegisterReloadableEvent(MapEvents.LayerFadeEvents);
        RegisterReloadableEvent(MapEvents.PlayfieldMoveEvents);
        RegisterReloadableEvent(MapEvents.PlayfieldScaleEvents);
        RegisterReloadableEvent(MapEvents.PlayfieldRotateEvents);

        if (PlayerIndex == 0 && !IsSubPlayfield)
            registerReloadableShake(MapEvents.ShakeEvents);
    }

    #region Event Registration

    protected void RegisterReloadableEvent<T>(List<T> initial) where T : IApplicableToPlayfield
    {
        var props = typeof(T).GetAnimatedProperties();
        initial.ForEach(x => x.Apply(this));

        Ruleset.RegisterReload<T>(objs =>
        {
            props.ForEach(x => ClearTransforms(false, x));
            objs.ForEach(x => x.Apply(this));
        });
    }

    private void registerReloadableShake(List<ShakeEvent> shakes)
    {
        applyShakes(shakes);

        Ruleset.RegisterReload<ShakeEvent>(objs =>
        {
            shakeProxy.ClearTransforms(false, nameof(Position));
            applyShakes(objs);
        });

        void applyShakes(List<ShakeEvent> list)
        {
            shakeProxy.Position = Vector2.Zero;

            foreach (var shake in list)
            {
                using (shakeProxy.BeginAbsoluteSequence(shake.Time))
                {
                    shakeProxy.Shake(Math.Max(shake.Duration, 0), shake.Magnitude);
                }
            }
        }
    }

    #endregion

    protected override void Update()
    {
        base.Update();
        updatePositionScale();

        if (PlayerIndex == 0 && !IsSubPlayfield && Ruleset.ShakeTarget != null)
            Ruleset.ShakeTarget.Position = shakeProxy.Position;
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => Dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    #region Positioning

    public float AnimationX { get; set; }
    public float AnimationY { get; set; }
    public float AnimationZ { get; set; }
    public Vector2 AnimationScale { get; set; } = Vector2.One;

    private readonly Vector3 camera = new(0, 0, -100);

    private void updatePositionScale()
    {
        var scale = scaleForZ(AnimationZ);

        if (!float.IsFinite(scale))
            scale = 1;

        var result = (new Vector2(AnimationX, AnimationY) - camera.Xy) * scale + camera.Xy;
        Position = result;
        Scale = new Vector2(scale) * AnimationScale * new Vector2(1, IsFlipped ? -1 : 1);
    }

    private float scaleForZ(float z) => -camera.Z / Math.Max(1f, z - camera.Z);

    #endregion

    private partial class ShakeProxy : Container
    {
        public override bool RemoveCompletedTransforms => false;
    }
}
