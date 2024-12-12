using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Map.Structures.Events;
using fluXis.Game.Screens.Edit.Tabs.Charting.Effect.EffectEdit;
using osu.Framework.Allocation;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags.EffectTags;

public partial class ShakeEventTag : EditorTag
{
    [Resolved]
    private PanelContainer panels { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    public ShakeEvent Shake => (ShakeEvent)TimedObject;

    public ShakeEventTag(EditorTagContainer parent, ITimedObject timedObject)
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
        panels.Content = new ShakeEditorPanel
        {
            Event = Shake,
            Map = map,
            EditorClock = clock
        };
        return true;
    }
}
