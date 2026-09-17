namespace fluXis.Screens.Edit;

public class EditorInspectContext
{
    public EditorMap Map { get; }
    public bool Update { get; }

    public EditorInspectContext(EditorMap map, bool update)
    {
        Map = map;
        Update = update;
    }
}
