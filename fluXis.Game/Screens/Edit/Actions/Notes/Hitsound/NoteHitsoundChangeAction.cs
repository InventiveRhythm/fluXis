using System.Linq;
using fluXis.Game.Map.Structures;

namespace fluXis.Game.Screens.Edit.Actions.Notes.Hitsound;

public class NoteHitsoundChangeAction : EditorAction
{
    public override string Description => $"Change sounds of {infos.Length} note(s) to {newSample}";

    private HitObject[] infos { get; }
    private string newSample { get; }
    private string[] samples { get; }

    public NoteHitsoundChangeAction(HitObject[] infos, string newSample)
    {
        this.infos = infos;
        this.newSample = newSample;

        samples = infos.Select(i => i.HitSound).ToArray();
    }

    public override void Run(EditorMap map)
    {
        foreach (var info in infos)
            info.HitSound = newSample;

        map.UpdateHitSounds();
    }

    public override void Undo(EditorMap map)
    {
        for (int i = 0; i < infos.Length; i++)
            infos[i].HitSound = samples[i];

        map.UpdateHitSounds();
    }
}
