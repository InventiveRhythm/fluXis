using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Gameplay.Input;
using fluXis.Game.Screens.Gameplay.Ruleset.Playfields;
using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.Ruleset.HitObjects;

public partial class HitObjectManager : Container<HitObjectColumn>
{
    [Resolved]
    private SkinManager skinManager { get; set; }

    [Resolved]
    private GameplayScreen screen { get; set; }

    [Resolved]
    private Playfield playfield { get; set; }

    [Resolved]
    private LaneSwitchManager laneSwitchManager { get; set; }

    private GameplayInput input => screen.Input;

    private Bindable<bool> useSnapColors;
    public bool UseSnapColors => useSnapColors.Value;

    private Bindable<float> scrollSpeed;
    public float ScrollSpeed => scrollSpeed.Value * (scrollSpeed.Value / (scrollSpeed.Value * screen.Rate));

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
                                               .Select(i => new HitObjectColumn(screen.Map, this, i + 1));

        scrollSpeed = config.GetBindable<float>(FluXisSetting.ScrollSpeed);
        useSnapColors = config.GetBindable<bool>(FluXisSetting.SnapColoring);
        hitsounds = config.GetBindable<bool>(FluXisSetting.Hitsounding);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

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

    protected override void Update()
    {
        Finished = Children.All(l => l.Finished);
    }

    public float PositionAtLane(float lane)
    {
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
        var events = screen.MapEvents.HitObjectEaseEvents;

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

        if (playfield.Index > 0)
            idx += playfield.Index * (screen.Input.Keys.Count / 2);

        if (screen.Input.Keys.Count > idx)
            drawable.Keybind = screen.Input.Keys[idx];

        drawable.OnLoadComplete += _ =>
        {
            for (var i = 0; i < screen.Input.Pressed.Length; i++)
            {
                if (!screen.Input.Pressed[i])
                    continue;

                var bind = screen.Input.Keys[i];
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

        var channel = screen.Hitsounding.GetSample(sound, hitsounds.Value);
        channel?.Play();
    }
}
