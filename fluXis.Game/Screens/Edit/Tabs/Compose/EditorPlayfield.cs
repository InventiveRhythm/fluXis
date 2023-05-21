using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs.Compose.Effect;
using fluXis.Game.Screens.Edit.Tabs.Compose.HitObjects;
using fluXis.Game.Screens.Edit.Tabs.Compose.Lines;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Compose;

public partial class EditorPlayfield : Container
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private Storage storage { get; set; }

    [Resolved]
    private EditorValues values { get; set; }

    [Resolved]
    private EditorChangeHandler changeHandler { get; set; }

    public const int COLUMN_WIDTH = 80;
    public const int HITPOSITION_Y = 100;
    public const int SELECTION_FADE = 200;

    public int Snap { get; set; } = 4;
    public EditorTool Tool { get; set; } = EditorTool.Select;

    public ComposeTab Tab { get; }
    public RealmMap Map => Tab.Screen.Map;
    public MapInfo MapInfo => Tab.Screen.MapInfo;

    public Container<EditorHitObject> HitObjects { get; set; }
    public List<EditorHitObject> FutureHitObjects { get; } = new();

    public Container<EditorTimingLine> TimingLines { get; set; }
    public List<EditorTimingLine> FutureTimingLines { get; } = new();

    public Container PlayfieldContainer { get; set; }
    public Container SelectionContainer { get; set; }
    public WaveformGraph Waveform { get; set; }
    public Container LaneSwitchContainer { get; set; }
    public EditorEffectContainer EffectContainer { get; set; }
    public Container ColumnDividerContainer { get; set; }

    public Vector2 SelectionStart { get; set; }
    public Vector2 SelectionNow { get; set; }
    public float SelectionStartTime { get; set; }
    public float SelectionStartLane { get; set; }
    public bool Selecting { get; set; }

    public List<EditorHitObject> SelectedHitObjects { get; } = new();

    private bool notePlacable;
    private EditorHitObject ghostNote;
    private bool isDragging;

    private Box hitPosLine;
    private FluXisTextFlow debugText;

    public EditorPlayfield(ComposeTab tab)
    {
        Tab = tab;
    }

    [BackgroundDependencyLoader]
    private void load(Bindable<Waveform> waveform)
    {
        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            PlayfieldContainer = new Container
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
                    Waveform = new WaveformGraph
                    {
                        Height = COLUMN_WIDTH * Map.KeyCount,
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomLeft,
                        Rotation = -90,
                        BaseColour = FluXisColors.Accent2,
                        LowColour = FluXisColors.Accent,
                        MidColour = FluXisColors.Accent3,
                        HighColour = FluXisColors.Accent4
                    },
                    ColumnDividerContainer = getColumnDividers(),
                    hitPosLine = new Box
                    {
                        Height = 3,
                        RelativeSizeAxes = Axes.X,
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.TopLeft,
                        Y = -HITPOSITION_Y
                    },
                    TimingLines = new Container<EditorTimingLine>
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0
                    },
                    HitObjects = new Container<EditorHitObject>
                    {
                        RelativeSizeAxes = Axes.Both
                    },
                    ghostNote = new EditorHitObject(this)
                    {
                        Alpha = 0.5f,
                        Info = new HitObjectInfo()
                    },
                    LaneSwitchContainer = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Y = -HITPOSITION_Y,
                    },
                    EffectContainer = new EditorEffectContainer()
                }
            },
            SelectionContainer = new Container
            {
                BorderColour = Colour4.White,
                BorderThickness = 2,
                Masking = true,
                Alpha = 0,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.2f
                }
            },
            debugText = new FluXisTextFlow
            {
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                TextAnchor = Anchor.TopRight,
                AutoSizeAxes = Axes.Both,
                Margin = new MarginPadding { Right = 10, Top = 10 }
            }
        };

        loadTimingLines();
        loadHitObjects();
        loadEvents();

        waveform.BindValueChanged(w => Waveform.Waveform = w.NewValue, true);
        values.WaveformOpacity.BindValueChanged(opacity => Waveform.FadeTo(opacity.NewValue, 200), true);

        changeHandler.OnTimingPointAdded += RedrawLines;
        changeHandler.OnTimingPointRemoved += RedrawLines;
        changeHandler.OnTimingPointChanged += RedrawLines;
        changeHandler.OnKeyModeChanged += onKeyModeChanged;
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

            float target = i + 1 < MapInfo.TimingPoints.Count ? MapInfo.TimingPoints[i + 1].Time : clock.TrackLength;
            float increase = point.Signature * point.MsPerBeat / (4 * Snap);
            float position = point.Time;

            int j = 0;

            while (position < target)
            {
                FutureTimingLines.Add(new EditorTimingLine(this)
                {
                    Time = position,
                    Colour = getSnapColor(j % Snap, j)
                });
                position += increase;
                j++;
            }
        }
    }

    private void loadEvents()
    {
        foreach (var flashEvent in values.MapEvents.FlashEvents)
            EffectContainer.AddFlash(flashEvent);

        foreach (var laneSwitch in values.MapEvents.LaneSwitchEvents)
            LaneSwitchContainer.Add(new EditorLaneSwitchEvent { Event = laneSwitch, Map = Map });
    }

    public void RedrawLines()
    {
        FutureTimingLines.Clear();
        TimingLines.Clear();
        loadTimingLines();
    }

    private void onKeyModeChanged(int keys)
    {
        PlayfieldContainer.Width = COLUMN_WIDTH * keys;
        Waveform.Height = COLUMN_WIDTH * keys;
        EffectContainer.Clear();
        LaneSwitchContainer.Clear();
        loadEvents();

        ColumnDividerContainer.Clear();
        ColumnDividerContainer.Add(getColumnDividers());
    }

    protected override bool OnHover(HoverEvent e)
    {
        TimingLines.FadeIn(100);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        TimingLines.FadeOut(100);
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        string debug = "";
        debug += $"Time: {getTimeFromMouse(e.MousePosition)}ms\nLane: {getLaneFromMouse(e.ScreenSpaceMousePosition)}";
        debug += $"\nMouse Global: {e.MousePosition}";

        debugText.Text = debug;

        switch (Tool)
        {
            case EditorTool.Long when isDragging:
            {
                float holdTime = getTimeFromMouseSnapped(e.MousePosition) - ghostNote.Info.Time;
                if (holdTime < 0) holdTime = 0;
                ghostNote.Info.HoldTime = holdTime;
                break;
            }

            case EditorTool.Select when Selecting:
                SelectionNow = e.MousePosition;
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
                        SelectionStart = e.MousePosition;
                        SelectionStartTime = getTimeFromMouse(e.MousePosition);
                        SelectionStartLane = getLaneFromMouse(e.ScreenSpaceMousePosition);
                        SelectionContainer.FadeIn(SELECTION_FADE);
                        SelectionNow = e.MousePosition;
                        Selecting = true;
                        break;

                    case EditorTool.Single:
                        var hitObject = GetHitObjectAt(ghostNote.Info.Time, ghostNote.Info.Lane);
                        if (hitObject != null) return true;

                        if (!notePlacable) return true;

                        var copy = ghostNote.Info.Copy();
                        HitObjects.Add(new EditorHitObject(this) { Info = copy });
                        MapInfo.HitObjects.Add(copy);
                        break;

                    case EditorTool.Long:
                        isDragging = true;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return true;

            case MouseButton.Right when Tool is EditorTool.Single or EditorTool.Long:
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
                    HitObjects.Add(new EditorHitObject(this) { Info = copy });
                    MapInfo.HitObjects.Add(copy);

                    ghostNote.Info.HoldTime = 0; // reset hold time
                    break;

                case EditorTool.Select:
                    Selecting = false;
                    SelectionContainer.FadeOut(SELECTION_FADE);
                    selectHitObjects();
                    break;
            }
        }
    }

    private void updateSelection()
    {
        var width = Math.Abs(SelectionNow.X - SelectionStart.X);
        float timeEnd = getYFromTime(getTimeFromMouse(SelectionNow));

        SelectionContainer.Width = width;
        SelectionContainer.Height = Math.Abs(timeEnd - getYFromTime(SelectionStartTime));
        SelectionContainer.X = Math.Min(SelectionStart.X, SelectionNow.X);
        SelectionContainer.Y = Math.Min(getYFromTime(SelectionStartTime), timeEnd);
    }

    private void selectHitObjects()
    {
        SelectedHitObjects.ForEach(h => h.UpdateSelection(false));
        SelectedHitObjects.Clear();

        var timeEnd = getTimeFromMouseSnapped(SelectionNow);
        var laneEnd = getLaneFromMouse(SelectionNow);

        bool laneReversed = laneEnd < SelectionStartLane;
        bool timeReversed = timeEnd < SelectionStartTime;

        Logger.Log($"Selecting from {SelectionStartTime} to {timeEnd} and from {SelectionStartLane} to {laneEnd}");

        foreach (var hitObject in MapInfo.HitObjects)
        {
            bool inLane = hitObject.Lane >= SelectionStartLane && hitObject.Lane <= laneEnd;
            bool inTime = hitObject.Time >= SelectionStartTime && hitObject.Time <= timeEnd;

            if (laneReversed) inLane = hitObject.Lane <= SelectionStartLane && hitObject.Lane >= laneEnd;
            if (timeReversed) inTime = hitObject.Time <= SelectionStartTime && hitObject.Time >= timeEnd;

            if (inLane && inTime)
            {
                var editorHitObject = GetHitObjectAt(hitObject.Time, hitObject.Lane);
                if (editorHitObject == null) continue;

                editorHitObject.UpdateSelection(true);
                SelectedHitObjects.Add(editorHitObject);
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

        Waveform.Width = songLengthInPixels;
        Waveform.Y = songTimeInPixels;

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

                return val == 0 ? Colour4.White : Colour4.Purple;

            case 8:
            case 16:
                if (val == 0) return Colour4.White;
                if ((i - 1) % 2 == 0) return Colour4.Gold;

                return i % 4 == 0 ? Colour4.Red : Colour4.FromHex("#0085ff");

            default:
                if (val != 0) return Colour4.FromHex(i % 2 == 0 ? "#af4fb8" : "#4e94b7");

                Logger.Log($"Unknown snap value: {Snap}", LoggingTarget.Runtime, LogLevel.Important);
                return Colour4.White;
        }
    }
}
