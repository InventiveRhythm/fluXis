using System.Collections.Generic;
using fluXis.Screens.Edit;
using JetBrains.Annotations;
using osu.Framework.Graphics;

namespace fluXis.Plugins.Capabilities;

[UsedImplicitly]
public interface IEditorCapability : IPluginCapability
{
    [CanBeNull]
    IReadOnlyList<EditorTab> CustomTabs => [];

    [CanBeNull]
    IReadOnlyList<Drawable> AddToTimeline => [];

    void OnEditorLoaded(EditorMap map)
    {
    }
}
