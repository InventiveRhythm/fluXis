using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;

public partial class EditorHitObjectContainer : Container
{
    public const int HITPOSITION = 80;
    public const int NOTEWIDTH = 80;

    [Resolved]
    private EditorValues values { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        foreach (var hitObject in values.MapInfo.HitObjects)
        {
            Add(new EditorHitObject
            {
                Data = hitObject
            });
        }

        Add(new Box
        {
            RelativeSizeAxes = Axes.X,
            Height = 3,
            Anchor = Anchor.BottomCentre,
            Origin = Anchor.BottomCentre,
            Y = -HITPOSITION
        });
    }

    public Vector2 ScreenSpacePositionAtTime(float time, int lane) => ToScreenSpace(new Vector2(PositionFromLane(lane), PositionAtTime(time)));
    public float PositionFromLane(float lane) => (lane - 1) * NOTEWIDTH;
    public float PositionAtTime(float time) => DrawHeight - HITPOSITION - .5f * ((time - (float)Time.Current) * values.Zoom);
}
