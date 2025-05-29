using System.ComponentModel;

namespace fluXis.Screens.Edit.Input;

public enum EditorKeybinding
{
    Undo,
    Redo,
    Save,

    [Description("Open help/wiki")]
    OpenHelp,

    [Description("Open folder")]
    OpenFolder,

    [Description("Flip current selection")]
    FlipSelection,

    [Description("Shuffle current selection")]
    ShuffleSelection,

    [Description("Clone current selection")]
    CloneSelection,

    [Description("Delete current selection")]
    DeleteSelection,

    [Description("Toggle sidebar")]
    ToggleSidebar,

    [Description("Create timing point at current position")]
    AddTiming,

    [Description("Create note/bookmark at current position")]
    AddNote,

    [Description("Seek to previous note/bookmark")]
    PreviousNote,

    [Description("Seek to next note/bookmark")]
    NextNote
}
