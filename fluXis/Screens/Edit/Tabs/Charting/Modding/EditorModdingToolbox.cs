using fluXis.Graphics.Sprites.Icons;
using fluXis.Screens.Edit.Modding;
using fluXis.Screens.Edit.Tabs.Charting.Tools;
using fluXis.Screens.Edit.Tabs.Shared.Toolbox;

namespace fluXis.Screens.Edit.Tabs.Charting.Modding;

public partial class EditorModdingToolbox : ToolboxCategory
{
    public EditorModdingToolbox(EditorModding modding)
    {
        Title = "Modding";
        Icon = FontAwesome6.Solid.Pen;
        Tools = new ChartingTool[] { new EditorChangeRequestTool(modding) };
        Alpha = 0;

        modding.OnEnable += Show;
        modding.OnDisable += Hide;
    }
}
