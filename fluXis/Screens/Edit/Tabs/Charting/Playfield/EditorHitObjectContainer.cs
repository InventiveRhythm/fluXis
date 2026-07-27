using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Charting.Playfield.Objects.Events;
using fluXis.Screens.Edit.Tabs.Charting.Playfield.Objects.Hits;
using fluXis.Screens.Edit.Tabs.Verify;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield;

public partial class EditorHitObjectContainer : Container<EditorDrawableObject>
{
    public const int HITPOSITION = 130;
    public const int NOTEWIDTH = 98;

    public IEnumerable<EditorDrawableObject> Objects => back.Concat(InternalChildren.OfType<EditorDrawableObject>());

    private readonly List<EditorDrawableObject> back = new();

    [Resolved]
    private EditorSettings settings { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private ChartingContainer charting { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        register(map.MapInfo.HitObjects);
        register(map.MapInfo.TimingPoints);
        register(map.MapInfo.ScrollVelocities);

        // TODO: please make this use reflection
        register(map.MapEvents.LaneSwitchEvents);
        register(map.MapEvents.FlashEvents);
        register(map.MapEvents.ColorFadeEvents);
        register(map.MapEvents.PulseEvents);
        register(map.MapEvents.ShakeEvents);
        register(map.MapEvents.PlayfieldMoveEvents);
        register(map.MapEvents.PlayfieldScaleEvents);
        register(map.MapEvents.HitObjectEaseEvents);
        register(map.MapEvents.LayerFadeEvents);
        register(map.MapEvents.ShaderEvents);
        register(map.MapEvents.BeatPulseEvents);
        register(map.MapEvents.PlayfieldRotateEvents);
        register(map.MapEvents.ScrollMultiplyEvents);
        register(map.MapEvents.TimeOffsetEvents);
        register(map.MapEvents.CameraMoveEvents);
        register(map.MapEvents.CameraScaleEvents);
        register(map.MapEvents.CameraRotateEvents);
        register(map.MapEvents.LoopEvents);

        void register<T>(List<T> list)
            where T : class, ITimedObject
        {
            map.RegisterAddListener<T>(add);
            map.RegisterRemoveListener<T>(remove);
            list.ForEach(add);
        }
    }

    private void add(ITimedObject obj)
    {
        if (obj.Lane < 1)
        {
            var atTime = Objects.Where(x => x.Data is not HitObject && Math.Abs(x.Data.Time - obj.Time) < 0.1f);
            obj.Lane = atTime.Count() + 1 + ((IVerifyContext)map).MaxKeyCount;
        }

        EditorDrawableObject draw = null;

        switch (obj)
        {
            case HitObject hit:
            {
                switch (hit.Type)
                {
                    case HitObjectType.Normal:
                        if (hit.LongNote)
                            draw = new EditorLongNote(hit);
                        else
                            draw = new EditorSingleNote(hit);

                        break;

                    case HitObjectType.Tick:
                        draw = new EditorTickNote(hit);
                        break;

                    case HitObjectType.Landmine:
                        draw = new EditorLandmine(hit);
                        break;
                }

                break;
            }

            default:
                draw = new EditorDrawableEvent(obj);
                break;
        }

        if (draw is null)
            return;

        LoadComponent(draw);
        charting.ObjectDrawables[obj] = draw;
        back.Add(draw);
    }

    private void remove(ITimedObject info)
    {
        if (!charting.ObjectDrawables.TryGetValue(info, out var draw))
            return;

        Remove(draw, false);
        back.Remove(draw);

        charting.ObjectDrawables.Remove(info);
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
    public float PositionFromLane(float lane) => (lane - 1) * NOTEWIDTH;

    public double TimeAtPosition(float y) => (DrawHeight - HITPOSITION - y) * 2 / settings.Zoom + clock.CurrentTime;
    public int LaneAtPosition(float x) => (int)((x + NOTEWIDTH) / NOTEWIDTH);

    public double TimeAtScreenSpacePosition(Vector2 screenSpacePosition) => TimeAtPosition(ToLocalSpace(screenSpacePosition).Y);
    public int LaneAtScreenSpacePosition(Vector2 position) => LaneAtPosition(ToLocalSpace(position).X);
}
