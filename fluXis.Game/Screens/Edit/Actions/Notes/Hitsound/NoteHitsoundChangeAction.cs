using System.Linq;
using fluXis.Game.Map;

namespace fluXis.Game.Screens.Edit.Actions.Notes.Hitsound;

public class NoteHitsoundChangeAction : EditorAction
{
    private readonly HitObjectInfo[] infos;
    private readonly EditorMapInfo mapInfo;
    private readonly string newSample;
    private readonly string[] samples;

    public NoteHitsoundChangeAction(EditorMapInfo mapInfo, HitObjectInfo[] infos, string newSample)
    {
        this.mapInfo = mapInfo;
        this.infos = infos;
        this.newSample = newSample;

        samples = infos.Select(i => i.HitSound).ToArray();
    }

    public override void Run()
    {
        foreach (var info in infos)
            info.HitSound = newSample;

        mapInfo.ChangeHitSounds();
    }

    public override void Undo()
    {
        for (int i = 0; i < infos.Length; i++)
            infos[i].HitSound = samples[i];

        mapInfo.ChangeHitSounds();
    }
}
