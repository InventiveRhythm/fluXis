using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Attributes;
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

    public float ScaledNoteWidth => NOTEWIDTH * settings.ObjectZoom;

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

        registerEffect(map.MapInfo.HitObjects);
        registerEffect(map.MapInfo.TimingPoints);
        registerEffect(map.MapInfo.ScrollVelocities);

        foreach (var (type, list) in map.MapEvents.GetListsForTypes())
        {
            if (type.GetCustomAttribute<DoNotShowInEditorPlayfieldAttribute>() != null)
                continue;

            var method = GetType().GetMethod(nameof(registerEffect), BindingFlags.Instance | BindingFlags.NonPublic)!;
            method = method.MakeGenericMethod(type);
            method.Invoke(this, [list]);
        }
    }

    private void registerEffect<T>(List<T> list) where T : class, ITimedObject
    {
        map.RegisterAddListener<T>(add);
        map.RegisterRemoveListener<T>(remove);
        list.ForEach(add);
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
    public float PositionFromLane(float lane) => (lane - 1) * (ScaledNoteWidth);

    public double TimeAtPosition(float y) => (DrawHeight - HITPOSITION - y) * 2 / settings.Zoom + clock.CurrentTime;
    public int LaneAtPosition(float x) => (int)((x + ScaledNoteWidth) / (ScaledNoteWidth));

    public double TimeAtScreenSpacePosition(Vector2 screenSpacePosition) => TimeAtPosition(ToLocalSpace(screenSpacePosition).Y);
    public int LaneAtScreenSpacePosition(Vector2 position) => LaneAtPosition(ToLocalSpace(position).X);
}
