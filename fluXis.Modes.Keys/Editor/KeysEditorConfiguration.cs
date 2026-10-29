using fluXis.Map;
using fluXis.Screens.Edit;

namespace fluXis.Modes.Keys.Editor;

public class KeysEditorConfiguration : EditorConfiguration
{
    public override int ColumnCount { get; }

    public KeysEditorConfiguration(PlayableMap map)
    {
        ColumnCount = KeysGameMode.ParseMode(map.Mode);
    }
}
