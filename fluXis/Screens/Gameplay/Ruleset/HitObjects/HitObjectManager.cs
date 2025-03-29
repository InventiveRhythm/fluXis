using System.Linq;
using fluXis.Configuration;
using fluXis.Map.Structures;
using fluXis.Screens.Gameplay.Audio.Hitsounds;
using fluXis.Screens.Gameplay.Input;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
using fluXis.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Gameplay.Ruleset.HitObjects;

public partial class HitObjectManager : Container<HitObjectColumn>
{
    [Resolved]
    private SkinManager skinManager { get; set; }

    [Resolved]
    private RulesetContainer ruleset { get; set; }

    [Resolved]
    private Playfield playfield { get; set; }

    [Resolved]
    private Hitsounding hitsounding { get; set; }

    [Resolved]
    private LaneSwitchManager laneSwitchManager { get; set; }

    private GameplayInput input => ruleset.Input;

    private Bindable<bool> useSnapColors;
    public bool UseSnapColors => useSnapColors.Value;

    private Bindable<float> scrollSpeed;
    public float ScrollSpeed => scrollSpeed.Value * (scrollSpeed.Value / (scrollSpeed.Value * ruleset.Rate));

    private Bindable<bool> hitsounds;

    public float DirectScrollMultiplier { get; set; } = 1;
    public double VisualTimeOffset { get; set; } = 0;

    public int KeyCount => playfield.RealmMap.KeyCount;

    public float HitPosition => DrawHeight - laneSwitchManager.HitPosition;

    public bool Finished { get; private set; }

    public bool Break => timeUntilNextHitObject >= 2000;
    private double timeUntilNextHitObject => (nextHitObject?.Time ?? double.MaxValue) - Clock.CurrentTime;

    private HitObject nextHitObject
    {
        get
        {
            var all = this.Select(l => l.NextUp);
            return all.MinBy(h => h?.Time);
        }
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        RelativeSizeAxes = Axes.Both;

        InternalChildrenEnumerable = Enumerable.Range(0, KeyCount)
                                               .Select(i =>
                                               {
                                                   var lane = i + 1;

                                                   if (ruleset.MapInfo.IsSplit)
                                                       lane += KeyCount * playfield.Index;

                                                   return new HitObjectColumn(ruleset.MapInfo, this, lane);
                                               });

        scrollSpeed = config.GetBindable<float>(FluXisSetting.ScrollSpeed);
        useSnapColors = config.GetBindable<bool>(FluXisSetting.SnapColoring);
        hitsounds = config.GetBindable<bool>(FluXisSetting.Hitsounding);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (!playfield.IsSubPlayfield)
        {
            input.OnPress += key =>
            {
                var lane = input.Keys.IndexOf(key) + 1;
                lane -= KeyCount * playfield.Index;

                if (lane > KeyCount || lane <= 0)
                    return;

                var hit = this[lane - 1].NextUp;

                if (hit == null)
                    return;

                PlayHitSound(hit);
            };
        }
    }

    protected override void Update()
    {
        Finished = Children.All(l => l.Finished);
    }

    public float PositionAtLane(float lane)
    {
        while (lane > KeyCount)
            lane -= KeyCount;

        var receptors = playfield.Receptors;
        var x = 0f;

        var frac = lane % 1;
        var colWidth = skinManager.SkinJson.GetKeymode(KeyCount).ColumnWidth;

        for (int i = 1; i < (int)lane; i++)
        {
            if (i > receptors.Count)
                x += colWidth;
            else
                x += receptors[i - 1].Width;
        }

        x += frac * colWidth;
        return x;
    }

    public Easing EasingAtTime(double time)
    {
        var events = ruleset.MapEvents.HitObjectEaseEvents;

        if (events.Count == 0)
            return Easing.None;

        var first = events.LastOrDefault(e => e.Time <= time);
        return first?.Easing ?? Easing.None;
    }

    public float WidthOfLane(int lane) => laneSwitchManager.WidthFor(lane);

    public DrawableHitObject CreateHitObject(HitObject hitObject)
    {
        var drawable = GetDrawableFor(hitObject);
        var idx = hitObject.Lane - 1;

        if (playfield.Index > 0 && !playfield.MapInfo.IsSplit)
            idx += playfield.Index * (input.Keys.Count / 2);

        if (input.Keys.Count > idx)
            drawable.Keybind = input.Keys[idx];

        drawable.OnLoadComplete += _ =>
        {
            for (var i = 0; i < input.Pressed.Length; i++)
            {
                if (!input.Pressed[i])
                    continue;

                var bind = input.Keys[i];
                drawable.OnPressed(bind);
            }
        };

        return drawable;
    }

    public static DrawableHitObject GetDrawableFor(HitObject hit)
    {
        switch (hit.Type)
        {
            case 1:
                return new DrawableTickNote(hit);

            default:
            {
                if (hit.LongNote)
                    return new DrawableLongNote(hit);

                return new DrawableNote(hit);
            }
        }
    }

    public void PlayHitSound(HitObject hitObject, bool userTriggered = true)
    {
        if (ruleset.CatchingUp || playfield.IsSubPlayfield)
            return;

        // ignore hitsounds when the next is a
        // tick note since it would be played twice
        // when hitting them as a normal note
        if (hitObject is { Type: 1 } && userTriggered) return;

        var sound = hitObject.HitSound;

        if (sound == ":normal" && hitObject.Type == 1)
        {
            sound = ":tick-big";

            if (hitObject.HoldTime > 0)
                sound = ":tick-small";
        }

        var channel = hitsounding.GetSample(sound, hitsounds.Value);
        channel?.Play();
    }
}
