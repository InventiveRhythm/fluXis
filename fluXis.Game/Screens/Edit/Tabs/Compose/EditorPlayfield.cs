using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs.Compose.Effect;
using fluXis.Game.Screens.Edit.Tabs.Compose.HitObjects;
using fluXis.Game.Screens.Edit.Tabs.Compose.Lines;
using fluXis.Game.Screens.Edit.Tabs.Compose.Toolbox;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Compose;

public partial class EditorPlayfield : Container
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorValues values { get; set; }

    [Resolved]
    private EditorChangeHandler changeHandler { get; set; }

    public const int COLUMN_WIDTH = 80;
    public const int HITPOSITION_Y = 100;
    public static readonly int[] SNAP_DIVISORS = { 1, 2, 3, 4, 6, 8, 12, 16 };

    public EditorTool Tool
    {
        get => values.Tool;
        set => values.Tool = value;
    }

    private ComposeTab tab { get; }
    public RealmMap Map => tab.Screen.Map;
    private MapInfo mapInfo => tab.Screen.MapInfo;

    private Container<EditorHitObject> hitObjects { get; set; }
    private List<EditorHitObject> futureHitObjects { get; } = new();

    private Container<EditorTimingLine> timingLines { get; set; }
    private List<EditorTimingLine> futureTimingLines { get; } = new();

    private Container playfieldContainer { get; set; }
    private Container selectionContainer { get; set; }
    private WaveformGraph waveform { get; set; }
    private EditorEffectContainer effectContainer { get; set; }
    private Container columnDividerContainer { get; set; }

    private Vector2 selectionStart { get; set; }
    private Vector2 selectionNow { get; set; }
    private float selectionStartTime { get; set; }
    private int selectionStartLane { get; set; }
    private bool selecting { get; set; }

    private List<EditorHitObject> selectedHitObjects { get; } = new();

    private Sample hitSound;

    private bool notePlacable;
    private EditorHitObject ghostNote;
    private bool isDragging;

    private Box hitPosLine;

    public EditorPlayfield(ComposeTab tab)
    {
        this.tab = tab;
    }

    [BackgroundDependencyLoader]
    private void load(Bindable<Waveform> waveform, ISampleStore samples)
    {
        RelativeSizeAxes = Axes.Both;
        Masking = true;

        hitSound = samples.Get("Gameplay/hitsound");

        InternalChildren = new Drawable[]
        {
            playfieldContainer = new Container
            {
                Width = COLUMN_WIDTH * Map.KeyCount,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Children = new Drawable[]
                {
                    new Box // background
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background
                    },
                    new Box // border left
                    {
                        Width = 4,
                        RelativeSizeAxes = Axes.Y,
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopRight
                    },
                    new Box // border right
                    {
                        Width = 4,
                        RelativeSizeAxes = Axes.Y,
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopLeft
                    },
                    this.waveform = new WaveformGraph
                    {
                        Height = COLUMN_WIDTH * Map.KeyCount,
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomLeft,
                        Rotation = -90,
                        BaseColour = FluXisColors.Accent.Lighten(.2f),
                        LowColour = FluXisColors.Accent.Lighten(.2f),
                        MidColour = FluXisColors.Accent.Lighten(.2f),
                        HighColour = FluXisColors.Accent.Lighten(.2f)
                    },
                    columnDividerContainer = getColumnDividers(),
                    hitPosLine = new Box
                    {
                        Height = 3,
                        RelativeSizeAxes = Axes.X,
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.TopLeft,
                        Y = -HITPOSITION_Y
                    },
                    timingLines = new Container<EditorTimingLine>
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0
                    },
                    hitObjects = new Container<EditorHitObject>
                    {
                        RelativeSizeAxes = Axes.Both
                    },
                    ghostNote = new EditorHitObject
                    {
                        Playfield = this,
                        Alpha = 0.5f,
                        Info = new HitObjectInfo()
                    },
                    effectContainer = new EditorEffectContainer()
                }
            },
            selectionContainer = new Container
            {
                BorderColour = Colour4.White,
                BorderThickness = 4,
                CornerRadius = 10,
                Masking = true,
                Alpha = 0,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.2f
                }
            },
            new EditorToolbox { Playfield = this }
        };

        loadTimingLines();
        loadHitObjects();
        loadEvents();

        waveform.BindValueChanged(w => this.waveform.Waveform = w.NewValue, true);
        values.WaveformOpacity.BindValueChanged(opacity => this.waveform.FadeTo(opacity.NewValue, 200), true);

        changeHandler.OnTimingPointAdded += redrawLines;
        changeHandler.OnTimingPointRemoved += redrawLines;
        changeHandler.OnTimingPointChanged += redrawLines;
        changeHandler.OnKeyModeChanged += onKeyModeChanged;
        changeHandler.SnapDivisorChanged += redrawLines;
    }

    private Container getColumnDividers()
    {
        var dividers = new Container
        {
            RelativeSizeAxes = Axes.Both
        };

        for (int i = 0; i < Map.KeyCount - 1; i++)
        {
            dividers.Add(new Box
            {
                Width = 1,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                X = COLUMN_WIDTH * (i + 1)
            });
        }

        return dividers;
    }

    private void loadHitObjects()
    {
        foreach (var hitObject in mapInfo.HitObjects)
        {
            futureHitObjects.Add(new EditorHitObject { Info = hitObject, Playfield = this });
        }
    }

    private void loadTimingLines()
    {
        for (int i = 0; i < mapInfo.TimingPoints.Count; i++)
        {
            var point = mapInfo.TimingPoints[i];

            if (point.HideLines || point.Signature == 0)
                continue;

            float target = i + 1 < mapInfo.TimingPoints.Count ? mapInfo.TimingPoints[i + 1].Time : clock.TrackLength;
            float increase = point.Signature * point.MsPerBeat / (4 * values.SnapDivisor);
            float position = point.Time;

            int j = 0;

            while (position < target)
            {
                futureTimingLines.Add(new EditorTimingLine
                {
                    Time = position,
                    Colour = getSnapColor(j % values.SnapDivisor, j)
                });
                position += increase;
                j++;
            }
        }
    }

    private void loadEvents()
    {
        foreach (var flashEvent in values.MapEvents.FlashEvents)
            effectContainer.AddFlash(flashEvent);

        foreach (var laneSwitch in values.MapEvents.LaneSwitchEvents)
            effectContainer.AddLaneSwitch(laneSwitch);
    }

    private void redrawLines()
    {
        futureTimingLines.Clear();
        timingLines.Clear();
        loadTimingLines();
    }

    private void onKeyModeChanged(int keys)
    {
        playfieldContainer.Width = COLUMN_WIDTH * keys;
        waveform.Height = COLUMN_WIDTH * keys;
        effectContainer.ClearAll();
        loadEvents();

        columnDividerContainer.Clear();
        columnDividerContainer.Add(getColumnDividers());

        hitObjects.ForEach(h => h.UpdateColors());
        futureHitObjects.ForEach(h => h.UpdateColors());
    }

    protected override bool OnHover(HoverEvent e)
    {
        timingLines.FadeIn(100);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        timingLines.FadeOut(100);
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        switch (Tool)
        {
            case EditorTool.Long when isDragging:
            {
                float holdTime = getTimeFromMouseSnapped(e.MousePosition) - ghostNote.Info.Time;

                if (holdTime < 0)
                {
                    ghostNote.Info.Time += holdTime;
                    ghostNote.Info.HoldTime += holdTime;
                }
                else
                {
                    ghostNote.Info.HoldTime = holdTime;
                }

                break;
            }

            case EditorTool.Select when selecting:
                selectionNow = e.MousePosition;
                break;

            case EditorTool.Select when isDragging:
                var time = getTimeFromMouseSnapped(e.MousePosition);
                var lane = getLaneFromMouse(e.ScreenSpaceMousePosition);
                var delta = time - SnapTime(selectionStartTime);
                var deltaLane = lane - selectionStartLane;

                var minLane = selectedHitObjects.Min(h => h.Info.Lane);
                var maxLane = selectedHitObjects.Max(h => h.Info.Lane);

                if (minLane + deltaLane < 1 || maxLane + deltaLane > Map.KeyCount)
                    deltaLane = 0;

                if (Math.Abs(delta) >= 1)
                    selectionStartTime = time;

                if (Math.Abs(deltaLane) >= 1)
                    selectionStartLane = lane;

                foreach (var hitObject in selectedHitObjects)
                {
                    hitObject.Info.Time += delta;
                    hitObject.Info.Time = SnapTime(hitObject.Info.Time);
                    hitObject.Info.Lane += deltaLane;
                }

                break;

            default:
                updateGhostNote(e);
                break;
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
                        if (selectedHitObjects.Any(h => h.IsHovered))
                        {
                            selectionStart = e.MousePosition;
                            selectionStartTime = getTimeFromMouse(e.MousePosition);
                            selectionStartLane = getLaneFromMouse(e.ScreenSpaceMousePosition);
                            isDragging = true;
                            break;
                        }

                        selectionStart = e.MousePosition;
                        selectionStartTime = getTimeFromMouse(e.MousePosition);
                        selectionStartLane = getLaneFromMouse(e.ScreenSpaceMousePosition);
                        selectionContainer.FadeIn();
                        selectionNow = e.MousePosition;
                        selecting = true;
                        break;

                    case EditorTool.Single:
                        var hitObject = getHitObjectAt(ghostNote.Info.Time, ghostNote.Info.Lane);
                        if (hitObject != null) return true;

                        if (!notePlacable) return true;

                        var copy = ghostNote.Info.Copy();
                        hitObjects.Add(new EditorHitObject { Info = copy, Playfield = this });
                        mapInfo.HitObjects.Add(copy);
                        break;

                    case EditorTool.Long:
                        isDragging = true;
                        break;
                }

                return true;

            case MouseButton.Right when Tool is EditorTool.Single or EditorTool.Long:
            {
                var hitObject = getHitObjectAt(ghostNote.Info.Time, ghostNote.Info.Lane);

                if (hitObject != null)
                {
                    hitObjects.Remove(hitObject, true);
                    mapInfo.HitObjects.Remove(hitObject.Info);
                }

                return true;
            }

            default:
                return false;
        }
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        if (e.Button == MouseButton.Left)
        {
            switch (Tool)
            {
                case EditorTool.Long when isDragging:
                    isDragging = false;

                    if (!notePlacable) return;

                    var copy = ghostNote.Info.Copy();
                    hitObjects.Add(new EditorHitObject { Info = copy, Playfield = this });
                    mapInfo.HitObjects.Add(copy);

                    ghostNote.Info.HoldTime = 0; // reset hold time
                    break;

                case EditorTool.Select:
                    if (selecting)
                    {
                        selecting = false;
                        selectionContainer.FadeOut(100);
                        selectHitObjects();
                    }

                    isDragging = false;

                    break;
            }
        }
    }

    private void updateSelection()
    {
        var width = Math.Abs(selectionNow.X - selectionStart.X);
        float timeEnd = getYFromTime(getTimeFromMouse(selectionNow));

        selectionContainer.Width = width;
        selectionContainer.Height = Math.Abs(timeEnd - getYFromTime(selectionStartTime));
        selectionContainer.X = Math.Min(selectionStart.X, selectionNow.X);
        selectionContainer.Y = Math.Min(getYFromTime(selectionStartTime), timeEnd);
    }

    private void selectHitObjects()
    {
        selectedHitObjects.ForEach(h => h.UpdateSelection(false));
        selectedHitObjects.Clear();

        var timeEnd = getTimeFromMouseSnapped(selectionNow);
        var laneEnd = getLaneFromMouse(selectionNow);

        bool laneReversed = laneEnd < selectionStartLane;
        bool timeReversed = timeEnd < selectionStartTime;

        Logger.Log($"Selecting from {selectionStartTime} to {timeEnd} and from {selectionStartLane} to {laneEnd}");

        foreach (var hitObject in mapInfo.HitObjects)
        {
            bool inLane = hitObject.Lane >= selectionStartLane && hitObject.Lane <= laneEnd;
            bool inTime = hitObject.Time >= selectionStartTime && hitObject.Time <= timeEnd;

            if (laneReversed) inLane = hitObject.Lane <= selectionStartLane && hitObject.Lane >= laneEnd;
            if (timeReversed) inTime = hitObject.Time <= selectionStartTime && hitObject.Time >= timeEnd;

            if (inLane && inTime)
            {
                var editorHitObject = getHitObjectAt(hitObject.Time, hitObject.Lane);
                if (editorHitObject == null) continue;

                editorHitObject.UpdateSelection(true);
                selectedHitObjects.Add(editorHitObject);
            }
        }
    }

    private void updateGhostNote(MouseMoveEvent e)
    {
        var mouse = e.MousePosition;
        int lane = getLaneFromMouse(e.ScreenSpaceMousePosition);

        if (lane < 1 || lane > Map.KeyCount || Tool is EditorTool.Select)
        {
            ghostNote.FadeOut(100);
            notePlacable = false;
            return;
        }

        ghostNote.Info.Lane = lane;
        ghostNote.Info.Time = getTimeFromMouseSnapped(mouse);
        ghostNote.FadeTo(0.5f, 100);
        notePlacable = true;
    }

    private float getTimeFromMouse(Vector2 mouse)
    {
        float distance = DrawHeight - HITPOSITION_Y - mouse.Y;
        distance *= 2;
        distance /= values.Zoom;
        return distance + (float)clock.CurrentTime;
    }

    private float getTimeFromMouseSnapped(Vector2 mouse) => SnapTime(getTimeFromMouse(mouse));

    private int getLaneFromMouse(Vector2 mouse) // 🍝 i think
    {
        float distance = mouse.X - hitPosLine.ScreenSpaceDrawQuad.TopLeft.X;
        float columnWidth = hitPosLine.ScreenSpaceDrawQuad.Width / Map.KeyCount;
        return (int)Math.Ceiling(distance / columnWidth);
    }

    private float getYFromTime(float time) => DrawHeight - HITPOSITION_Y - .5f * ((time - (float)clock.CurrentTime) * values.Zoom);

    private EditorHitObject getHitObjectAt(float time, int lane)
    {
        return hitObjects.FirstOrDefault(h =>
        {
            float minTime = h.Info.Time;
            float maxTime = h.Info.Time + h.Info.HoldTime;

            return h.Info.Lane == lane && time >= minTime && time <= maxTime;
        });
    }

    public float SnapTime(float time)
    {
        var tp = mapInfo.GetTimingPoint(time);
        float t = tp.Time;
        float increase = tp.Signature * tp.MsPerBeat / (4 * values.SnapDivisor);
        if (increase == 0) return time; // no snapping, the game will just freeze because it loops infinitely

        if (time < t)
        {
            while (true)
            {
                float next = t - increase;

                if (next < time)
                {
                    t = next;
                    break;
                }

                t = next;
            }
        }
        else
        {
            while (true)
            {
                float next = t + increase;
                if (next > time) break;

                t = next;
            }
        }

        if (t < 0) t = 0;
        if (t > clock.TrackLength) t = clock.TrackLength;

        return t;
    }

    protected override void Update()
    {
        updateHitObjects();
        updateTimingLines();
        updateSelection();

        float songLengthInPixels = .5f * (clock.TrackLength * values.Zoom);
        float songTimeInPixels = -HITPOSITION_Y - .5f * (-(float)clock.CurrentTime * values.Zoom);

        waveform.Width = songLengthInPixels;
        waveform.Y = songTimeInPixels;

        base.Update();
    }

    private void updateHitObjects()
    {
        List<EditorHitObject> toAdd = futureHitObjects.Where(hitObject => hitObject.IsOnScreen).ToList();

        foreach (var hitObject in toAdd)
        {
            futureHitObjects.Remove(hitObject);
            hitObjects.Add(hitObject);
        }

        List<EditorHitObject> toRemove = hitObjects.Where(hitObject => !hitObject.IsOnScreen).ToList();

        foreach (var hitObject in toRemove)
        {
            hitObjects.Remove(hitObject, false);
            futureHitObjects.Add(hitObject);
        }

        foreach (var hitObject in hitObjects.Where(h => !h.PlayedHitSound))
        {
            if (!clock.IsRunning || !(hitObject.Info.Time <= clock.CurrentTime)) continue;

            hitSound?.Play();
            hitObject.PlayedHitSound = true;
        }
    }

    private void updateTimingLines()
    {
        List<EditorTimingLine> toAdd = futureTimingLines.Where(timingLine => timingLine.IsOnScreen).ToList();

        foreach (var timingLine in toAdd)
        {
            futureTimingLines.Remove(timingLine);
            timingLines.Add(timingLine);
        }

        List<EditorTimingLine> toRemove = timingLines.Where(timingLine => !timingLine.IsOnScreen).ToList();

        foreach (var timingLine in toRemove)
        {
            timingLines.Remove(timingLine, false);
            futureTimingLines.Add(timingLine);
        }
    }

    private Colour4 getSnapColor(int val, int i)
    {
        switch (values.SnapDivisor)
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

                return val == 0 ? Colour4.White : Colour4.Purple;

            case 8:
            case 16:
                if (val == 0) return Colour4.White;
                if ((i - 1) % 2 == 0) return Colour4.Gold;

                return i % 4 == 0 ? Colour4.Red : Colour4.FromHex("#0085ff");

            default:
                if (val != 0) return Colour4.FromHex(i % 2 == 0 ? "#af4fb8" : "#4e94b7");

                Logger.Log($"Unknown snap value: {values.SnapDivisor}", LoggingTarget.Runtime, LogLevel.Important);
                return Colour4.White;
        }
    }
}
