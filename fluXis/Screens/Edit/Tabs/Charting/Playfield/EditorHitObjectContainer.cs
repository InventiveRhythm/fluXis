using System.Collections.Generic;
using System.Linq;
using fluXis.Map.Structures;
using fluXis.Screens.Edit.Tabs.Charting.Playfield.Objects;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield;

public partial class EditorHitObjectContainer : Container<EditorHitObject>
{
    public const int HITPOSITION = 130;
    public const int NOTEWIDTH = 98;

    public IEnumerable<EditorHitObject> HitObjects => back.Concat(InternalChildren.OfType<EditorHitObject>());

    private readonly List<EditorHitObject> back = new();

    [Resolved]
    private EditorSettings settings { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorPlayfield playfield { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    public int LaneOffset => playfield.Index * map.RealmMap.KeyCount;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        map.RegisterAddListener<HitObject>(add);
        map.RegisterRemoveListener<HitObject>(remove);
        map.MapInfo.HitObjects.ForEach(add);
    }

    private void add(HitObject info)
    {
        var idx = (info.Lane - 1) / map.RealmMap.KeyCount;
        if (idx != playfield.Index) return;

        EditorHitObject draw = null;

        switch (info.Type)
        {
            case 0:
                if (info.LongNote)
                    draw = new EditorLongNote(info);
                else
                    draw = new EditorSingleNote(info);

                break;

            case 1:
                draw = new EditorTickNote(info);
                break;

            case 2:
                draw = new EditorLandmine(info);
                break;
        }

        if (draw is null)
            return;

        LoadComponent(draw);
        info.EditorDrawable = draw;
        back.Add(draw);
    }

    private void remove(HitObject info)
    {
        var idx = (info.Lane - 1) / map.RealmMap.KeyCount;
        if (idx != playfield.Index) return;

        var draw = info.EditorDrawable;
        if (draw == null) return;

        Remove(draw, false);
        back.Remove(draw);

        info.EditorDrawable = null;
        draw.Dispose();
    }

    protected override void Update()
    {
        base.Update();

        var remove = Children.Where(x => !x.Visible).ToList();
        remove.ForEach(x =>
        {
            Remove(x, false);
            back.Add(x);
        });

        var add = back.Where(x => x.Visible).ToList();
        add.ForEach(x =>
        {
            Add(x);
            back.Remove(x);
        });
    }

    public Vector2 ScreenSpacePositionAtTime(double time, int lane) => ToScreenSpace(new Vector2(PositionFromLane(lane), PositionAtTime(time)));
    public float PositionAtTime(double time) => (float)(DrawHeight - HITPOSITION - .5f * ((time - clock.CurrentTime) * settings.Zoom));
    public float PositionFromLane(float lane) => (lane - 1 - LaneOffset) * NOTEWIDTH;

    public double TimeAtPosition(float y) => (DrawHeight - HITPOSITION - y) * 2 / settings.Zoom + clock.CurrentTime;
    public int LaneAtPosition(float x) => (int)((x + NOTEWIDTH) / NOTEWIDTH) + LaneOffset;

    public double TimeAtScreenSpacePosition(Vector2 screenSpacePosition) => TimeAtPosition(ToLocalSpace(screenSpacePosition).Y);
    public int LaneAtScreenSpacePosition(Vector2 position) => LaneAtPosition(ToLocalSpace(position).X);
}
