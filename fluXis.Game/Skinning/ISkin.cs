using fluXis.Game.Scoring.Enums;
using fluXis.Game.Skinning.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Skinning;

public interface ISkin
{
    public Skin Skin { get; }

    public Texture GetDefaultBackground();

    public Drawable GetStageBackground();
    public Drawable GetStageBorder(bool right);
    public Drawable GetLaneCover(bool bottom);

    public Drawable GetHitObject(int lane, int keyCount);
    public Drawable GetLongNoteBody(int lane, int keyCount);
    public Drawable GetLongNoteEnd(int lane, int keyCount);

    public Drawable GetColumnLighting(int lane, int keyCount);
    public Drawable GetReceptor(int lane, int keyCount, bool down);
    public Drawable GetHitLine();

    public Drawable GetJudgement(Judgement judgement);
}
