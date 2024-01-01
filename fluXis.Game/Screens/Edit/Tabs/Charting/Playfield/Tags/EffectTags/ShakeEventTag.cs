using fluXis.Game.Map.Events;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Charting.Effect.EffectEdit;
using osu.Framework.Allocation;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags.EffectTags;

public partial class ShakeEventTag : EditorTag
{
    [Resolved]
    private FluXisGameBase game { get; set; }

    [Resolved]
    private EditorValues values { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    public override int TagWidth => 120;

    public ShakeEvent Shake => (ShakeEvent)TimedObject;

    public ShakeEventTag(EditorTagContainer parent, TimedObject timedObject)
        : base(parent, timedObject)
    {
    }

    protected override void Update()
    {
        base.Update();
        Text.Text = $"Shake {(int)Shake.Duration}ms {Shake.Magnitude}px";
    }

    protected override bool OnClick(ClickEvent e)
    {
        game.Overlay = new ShakeEditorPanel
        {
            Event = Shake,
            MapEvents = values.MapEvents,
            EditorClock = clock
        };
        return true;
    }
}
