using fluXis.Game.Map.Events;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags.EffectTags;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags;

public partial class EffectTagContainer : EditorTagContainer
{
    protected override bool RightSide => true;

    protected override void LoadComplete()
    {
        foreach (var shake in Values.MapEvents.ShakeEvents)
            addShake(shake);

        Values.MapEvents.ShakeEventAdded += addShake;
        Values.MapEvents.ShakeEventRemoved += RemoveTag;
    }

    private void addShake(ShakeEvent shake) => AddTag(new ShakeEventTag(this, shake));
}
