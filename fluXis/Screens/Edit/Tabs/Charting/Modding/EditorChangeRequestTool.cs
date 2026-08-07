using fluXis.Graphics.Sprites.Icons;
using fluXis.Screens.Edit.Modding;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using fluXis.Screens.Edit.Tabs.Charting.Tools;
using osu.Framework.Graphics;
using osu.Framework.Localisation;

namespace fluXis.Screens.Edit.Tabs.Charting.Modding;

public class EditorChangeRequestTool : ChartingTool
{
    public override LocalisableString Name => "Comment";
    public override LocalisableString Description => "Creates a highlighted area with a comment.";
    public override Drawable CreateIcon() => new FluXisSpriteIcon { Icon = Phosphor.Bold.ChatsCircle };
    public override PlacementBlueprint CreateBlueprint() => new EditorChangeRequestBlueprint(modding);

    private EditorModding modding { get; }

    public EditorChangeRequestTool(EditorModding modding)
    {
        this.modding = modding;
    }
}
