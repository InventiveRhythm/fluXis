using fluXis.Map.Structures;
using osu.Framework.Extensions.IEnumerableExtensions;

namespace fluXis.Screens.Edit.Actions.Notes;

public class NoteMultiPlaceAction : EditorAction
{
    public override string Description => $"Place {infos.Length} notes at {(int)infos[0].Time}ms";
    private HitObject[] infos { get; }

    public NoteMultiPlaceAction(HitObject[] infos)
    {
        this.infos = infos;
    }

    public override void Run(EditorMap map) => infos.ForEach(map.Add);
    public override void Undo(EditorMap map) => infos.ForEach(map.Remove);
}
