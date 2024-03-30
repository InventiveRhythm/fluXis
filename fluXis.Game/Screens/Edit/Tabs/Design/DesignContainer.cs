using System.Collections.Generic;
using fluXis.Game.Screens.Edit.Tabs.Design.Effects;
using fluXis.Game.Screens.Edit.Tabs.Design.Playfield;
using fluXis.Game.Screens.Edit.Tabs.Design.Points;
using fluXis.Game.Screens.Edit.Tabs.Design.Toolbox;
using fluXis.Game.Screens.Edit.Tabs.Shared;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points;
using fluXis.Game.Screens.Edit.Tabs.Shared.Toolbox;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Design;

public partial class DesignContainer : EditorTabContainer
{
    protected override IEnumerable<Drawable> CreateContent()
    {
        return new Drawable[]
        {
            new EditorFlashLayer { InBackground = true },
            new EditorDesignPlayfield(),
            new EditorFlashLayer()
        };
    }

    protected override EditorToolbox CreateToolbox() => new DesignToolbox();
    protected override PointsSidebar CreatePointsSidebar() => new DesignSidebar();
}
