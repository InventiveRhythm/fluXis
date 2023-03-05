using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs.Compose.HitObjects;
using fluXis.Game.Screens.Edit.Tabs.Compose.Lines;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Compose;

public partial class EditorPlayfield : Container
{
    public const int COLUMN_WIDTH = 80;
    public const int HITPOSITION_Y = 100;

    public float Zoom { get; set; } = 2;
    public int Snap { get; set; } = 4;
    public EditorTool Tool { get; set; } = EditorTool.Long;

    public ComposeTab Tab { get; }
    public RealmMap Map => Tab.Screen.Map;
    public MapInfo MapInfo => Tab.Screen.MapInfo;

    public Container<EditorHitObject> HitObjects { get; }
    public List<EditorHitObject> FutureHitObjects { get; } = new();

    public Container<EditorTimingLine> TimingLines { get; }
    public List<EditorTimingLine> FutureTimingLines { get; } = new();

    private readonly EditorHitObject ghostNote;
    private bool isDragging;

    public EditorPlayfield(ComposeTab tab)
    {
        Tab = tab;

        Width = COLUMN_WIDTH * Map.KeyCount;
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.TopCentre;
        Origin = Anchor.TopCentre;

        // background
        Add(new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = FluXisColors.Background
        });

        // borders
        AddRange(new Drawable[]
        {
            new Box
            {
                Width = 4,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopRight
            },
            new Box
            {
                Width = 4,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopLeft
            }
        });

        // column dividers
        for (int i = 0; i < Map.KeyCount - 1; i++)
        {
            Add(new Box
            {
                Width = 1,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                X = COLUMN_WIDTH * (i + 1)
            });
        }

        // hit position line
        Add(new Box
        {
            Height = 3,
            RelativeSizeAxes = Axes.X,
            Anchor = Anchor.BottomLeft,
            Origin = Anchor.BottomLeft,
            Y = -HITPOSITION_Y
        });

        Add(TimingLines = new Container<EditorTimingLine> { RelativeSizeAxes = Axes.Both });
        Add(HitObjects = new Container<EditorHitObject> { RelativeSizeAxes = Axes.Both });

        Add(ghostNote = new EditorHitObject(this)
        {
            Alpha = 0.5f,
            Info = new HitObjectInfo()
        });

        loadTimingLines();
        loadHitObjects();
    }

    private void loadHitObjects()
    {
        foreach (var hitObject in MapInfo.HitObjects)
        {
            FutureHitObjects.Add(new EditorHitObject(this) { Info = hitObject });
        }
    }

    private void loadTimingLines()
    {
        for (int i = 0; i < MapInfo.TimingPoints.Count; i++)
        {
            var point = MapInfo.TimingPoints[i];

            if (point.HideLines || point.Signature == 0)
                continue;

            float target = i + 1 < MapInfo.TimingPoints.Count ? MapInfo.TimingPoints[i + 1].Time : MapInfo.EndTime;
            float increase = point.Signature * point.MsPerBeat / (4 * Snap);
            float position = point.Time;

            while (position < target)
            {
                FutureTimingLines.Add(new EditorTimingLine(this)
                {
                    Time = position,
                    Colour = getSnapColor(i % Snap, i)
                });
                position += increase;
            }
        }
    }

    protected override bool OnHover(HoverEvent e)
    {
        return true;
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        if (Tool == EditorTool.Long && isDragging)
        {
            float holdTime = getTimeFromMouse(ToLocalSpace(e.MousePosition)) - ghostNote.Info.Time;
            if (holdTime < 0) holdTime = 0;
            ghostNote.Info.HoldTime = holdTime;
        }
        else
        {
            updateGhostNote(ToLocalSpace(e.MousePosition));
        }

        return base.OnMouseMove(e);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        switch (e.Button)
        {
            case MouseButton.Left:
                switch (Tool)
                {
                    case EditorTool.Select:
                        break;

                    case EditorTool.Single:
                        var hitObject = GetHitObjectAt(ghostNote.Info.Time, ghostNote.Info.Lane);
                        if (hitObject != null) return true;

                        HitObjects.Add(new EditorHitObject(this) { Info = ghostNote.Info.Copy() });
                        MapInfo.HitObjects.Add(ghostNote.Info.Copy());
                        break;

                    case EditorTool.Long:
                        isDragging = true;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return true;

            case MouseButton.Right:
            {
                var hitObject = GetHitObjectAt(ghostNote.Info.Time, ghostNote.Info.Lane);

                if (hitObject != null)
                {
                    HitObjects.Remove(hitObject, true);
                    MapInfo.HitObjects.Remove(hitObject.Info);
                }

                return true;
            }

            default:
                return base.OnMouseDown(e);
        }
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        if (e.Button == MouseButton.Left)
        {
            if (Tool == EditorTool.Long && isDragging)
            {
                isDragging = false;

                HitObjects.Add(new EditorHitObject(this) { Info = ghostNote.Info.Copy() });
                MapInfo.HitObjects.Add(ghostNote.Info);

                ghostNote.Info.HoldTime = 0; // reset hold time
            }
        }

        base.OnMouseUp(e);
    }

    private void updateGhostNote(Vector2 mouse)
    {
        ghostNote.Info.Lane = (int)(mouse.X / COLUMN_WIDTH) + 1;
        ghostNote.Info.Time = getTimeFromMouse(mouse);
    }

    private float getTimeFromMouse(Vector2 mouse)
    {
        float hitY = DrawHeight - 60 - HITPOSITION_Y;
        float mouseHitYDelta = hitY - mouse.Y + 10;
        return SnapTime(Conductor.CurrentTime + mouseHitYDelta);
    }

    public EditorHitObject GetHitObjectAt(float time, int lane)
    {
        return HitObjects.FirstOrDefault(h =>
        {
            float minTime = h.Info.Time;
            float maxTime = h.Info.Time + h.Info.HoldTime;

            return h.Info.Lane == lane && time >= minTime && time <= maxTime;
        });
    }

    public float SnapTime(float time)
    {
        var tp = MapInfo.GetTimingPoint(time);
        float t = tp.Time;
        float increase = tp.Signature * tp.MsPerBeat / (4 * Snap);

        while (true)
        {
            float next = t + increase;
            if (next > time) break;

            t = next;
        }

        return t;
    }

    protected override void Update()
    {
        updateHitObjects();
        updateTimingLines();

        base.Update();
    }

    private void updateHitObjects()
    {
        List<EditorHitObject> toAdd = FutureHitObjects.Where(hitObject => hitObject.IsOnScreen).ToList();

        foreach (var hitObject in toAdd)
        {
            FutureHitObjects.Remove(hitObject);
            HitObjects.Add(hitObject);
        }

        List<EditorHitObject> toRemove = HitObjects.Where(hitObject => !hitObject.IsOnScreen).ToList();

        foreach (var hitObject in toRemove)
        {
            HitObjects.Remove(hitObject, false);
            FutureHitObjects.Add(hitObject);
        }
    }

    private void updateTimingLines()
    {
        List<EditorTimingLine> toAdd = FutureTimingLines.Where(timingLine => timingLine.IsOnScreen).ToList();

        foreach (var timingLine in toAdd)
        {
            FutureTimingLines.Remove(timingLine);
            TimingLines.Add(timingLine);
        }

        List<EditorTimingLine> toRemove = TimingLines.Where(timingLine => !timingLine.IsOnScreen).ToList();

        foreach (var timingLine in toRemove)
        {
            TimingLines.Remove(timingLine, false);
            FutureTimingLines.Add(timingLine);
        }
    }

    private Colour4 getSnapColor(int val, int i)
    {
        switch (Snap)
        {
            case 1:
                return Colour4.White;

            case 2:
                return val == 0 ? Colour4.White : Colour4.Red;

            case 4:
                return val switch
                {
                    0 or 4 => Colour4.White,
                    1 or 3 => Colour4.FromHex("#0085ff"),
                    _ => Colour4.Red
                };

            case 3:
            case 6:
            case 12:
                if (val % 3 == 0) return Colour4.Red;
                if (val == 0) return Colour4.White;

                return Colour4.Purple;

            case 8:
            case 16:
                if (val == 0) return Colour4.White;
                if ((i - 1) % 2 == 0) return Colour4.Gold;
                if (i % 4 == 0) return Colour4.Red;

                return Colour4.FromHex("#0085ff");

            default:
                if (val == 0)
                {
                    Logger.Log($"Unknown snap value: {Snap}", LoggingTarget.Runtime, LogLevel.Important);
                    return Colour4.White;
                }

                return Colour4.FromHex(i % 2 == 0 ? "#af4fb8" : "#4e94b7");
        }
    }
}
