using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Input;
using fluXis.Game.Map.Structures;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Screens.Edit.Actions;
using fluXis.Game.Screens.Edit.Actions.Notes;
using fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Game.Screens.Edit.Tabs.Charting.Points;
using fluXis.Game.Screens.Edit.Tabs.Charting.Tools;
using fluXis.Game.Screens.Edit.Tabs.Charting.Tools.Effects;
using fluXis.Game.Screens.Gameplay.Audio.Hitsounds;
using fluXis.Shared.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Charting;

public partial class ChartingContainer : Container, IKeyBindingHandler<PlatformAction>, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public const float WAVEFORM_OFFSET = 20;

    public IReadOnlyList<ChartingTool> Tools { get; } = new ChartingTool[]
    {
        new SelectTool(),
        new SingleNoteTool(),
        new LongNoteTool(),
        new TickNoteTool()
    };

    public IReadOnlyList<EffectTool> EffectTools { get; } = new EffectTool[]
    {
        new LaneSwitchTool(),
        new FlashTool(),
        new ShakeTool()
    };

    public static readonly int[] SNAP_DIVISORS = { 1, 2, 3, 4, 6, 8, 12, 16 };

    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private Editor editor { get; set; }

    [Resolved]
    private EditorActionStack actions { get; set; }

    [Resolved]
    private EditorSettings settings { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private Clipboard clipboard { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    public Bindable<string> CurrentHitSound { get; } = new($"{Hitsounding.DEFAULT_PREFIX}normal");

    private DependencyContainer dependencies;
    private InputManager inputManager;
    private double scrollAccumulation;
    private bool recordingInput;

    private Container playfieldContainer;
    private Box dim;
    private Toolbox.Toolbox toolbox;

    private ClickableContainer sidebarClickHandler;
    private PointsSidebar sidebar;

    public EditorPlayfield Playfield { get; private set; }
    public BlueprintContainer BlueprintContainer { get; private set; }
    public IEnumerable<EditorHitObject> HitObjects => Playfield.HitObjectContainer.HitObjects;
    public bool CursorInPlacementArea => Playfield.ReceivePositionalInputAt(inputManager.CurrentState.Mouse.Position);

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        editor.ChartingContainer = this;

        dependencies.Cache(this);
        dependencies.CacheAs(Playfield = new EditorPlayfield());

        InternalChildren = new Drawable[]
        {
            playfieldContainer = new Container
            {
                Name = "Playfield",
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    Playfield,
                    BlueprintContainer = new BlueprintContainer { ChartingContainer = this }
                }
            },
            dim = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black.Opacity(.4f),
                Alpha = 0
            },
            toolbox = new Toolbox.Toolbox(),
            sidebarClickHandler = new ClickableContainer
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
                Action = () => sidebar.OnWrapperClick?.Invoke()
            },
            sidebar = new PointsSidebar()
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        inputManager = GetContainingInputManager();

        toolbox.Expanded.BindValueChanged(e => Schedule(() => onSidebarExpand(e)));
        sidebar.Expanded.BindValueChanged(e => Schedule(() => onSidebarExpand(e)));

        void onSidebarExpand(ValueChangedEvent<bool> e)
        {
            var showDim = toolbox.Expanded.Value || sidebar.Expanded.Value;
            dim.FadeTo(showDim ? 1 : 0, 400, Easing.OutCubic);

            var leftSide = toolbox.Expanded.Value;
            var rightSide = sidebar.Expanded.Value;
            var bothSides = leftSide && rightSide;

            var offset = bothSides switch
            {
                false when leftSide => 40,
                false when rightSide => -40,
                _ => 0
            };

            playfieldContainer.MoveToX(offset, 500, Easing.OutCubic);
            sidebarClickHandler.FadeTo(rightSide ? 1 : 0);
        }
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        switch (e.Key)
        {
            case >= Key.Number1 and <= Key.Number9 when recordingInput && !e.ControlPressed:
            {
                var index = e.Key - Key.Number1;
                placeNote(index + 1);
                return true;
            }

            case >= Key.Number1 and <= Key.Number9 when !e.ControlPressed:
            {
                var index = e.Key - Key.Number1;

                if (index >= Tools.Count) return false;

                BlueprintContainer.CurrentTool = Tools[index];
                return true;
            }

            case Key.R when e.ControlPressed:
            {
                recordingInput = !recordingInput;

                if (recordingInput)
                    notifications.SendSmallText("Recording input.", FontAwesome6.Solid.Check);
                else
                    notifications.SendSmallText("Stopped recording input.", FontAwesome6.Solid.XMark);

                return true;
            }

            case >= Key.A and <= Key.Z when !e.ControlPressed:
            {
                var letter = e.Key.ToString();
                var tool = EffectTools.FirstOrDefault(t => t.Letter == letter);

                if (tool == null) return false;

                BlueprintContainer.CurrentTool = tool;
                return true;
            }

            case Key.Space:
            {
                if (clock.IsRunning)
                    clock.Stop();
                else
                    clock.Start();

                return true;
            }

            default:
                return false;
        }
    }

    protected override bool OnScroll(ScrollEvent e)
    {
        var scroll = e.ShiftPressed ? e.ScrollDelta.X : e.ScrollDelta.Y;
        int delta = scroll > 0 ? 1 : -1;

        if (e.ControlPressed)
        {
            settings.Zoom += delta * .1f;
            settings.Zoom = Math.Clamp(settings.Zoom, .5f, 5f);
        }
        else if (e.ShiftPressed)
        {
            var snaps = SNAP_DIVISORS;
            var index = Array.IndexOf(snaps, settings.SnapDivisor);
            index += delta;

            if (index < 0)
                index = snaps.Length - 1;
            else if (index >= snaps.Length)
                index = 0;

            settings.SnapDivisor = snaps[index];
        }
        else
        {
            if (scrollAccumulation != 0 && Math.Sign(scrollAccumulation) != delta)
                scrollAccumulation = delta * (1 - Math.Abs(scrollAccumulation));

            scrollAccumulation += e.ScrollDelta.Y;

            while (Math.Abs(scrollAccumulation) >= 1)
            {
                seek(scrollAccumulation < 0 ? 1 : -1);
                scrollAccumulation = scrollAccumulation < 0 ? Math.Min(0, scrollAccumulation + 1) : Math.Max(0, scrollAccumulation - 1);
            }
        }

        return true;
    }

    private void seek(int direction)
    {
        double amount = 1;

        if (clock.IsRunning)
        {
            var tp = map.MapInfo.GetTimingPoint(clock.CurrentTime);
            amount *= 4 * (tp.BPM / 120);
        }

        if (direction < 1)
            clock.SeekBackward(amount);
        else
            clock.SeekForward(amount);
    }

    private void placeNote(int lane)
    {
        if (lane > map.RealmMap.KeyCount)
            return;

        var time = (float)clock.CurrentTime;
        var snapped = Playfield.HitObjectContainer.SnapTime(time);

        var tp = map.MapInfo.GetTimingPoint(time);
        float increase = tp.Signature * tp.MsPerBeat / (4 * settings.SnapDivisor);
        var next = Playfield.HitObjectContainer.SnapTime(time + increase);

        // take the closest snap
        time = Math.Abs(time - snapped) < Math.Abs(time - next) ? snapped : next;

        var note = new HitObject
        {
            Time = time,
            HitSound = CurrentHitSound.Value,
            Lane = lane
        };

        actions.Add(new NotePlaceAction(note, map));
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
    {
        return dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
    }

    public bool OnPressed(KeyBindingPressEvent<PlatformAction> e)
    {
        switch (e.Action)
        {
            case PlatformAction.Copy:
                Copy();
                return true;

            case PlatformAction.Paste:
                Paste();
                return true;

            case PlatformAction.Cut:
                Copy(true);
                return true;

            case PlatformAction.SelectAll:
                BlueprintContainer.SelectAll();
                return true;

            case PlatformAction.Delete:
                BlueprintContainer.SelectionHandler.DeleteSelected();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<PlatformAction> e) { }

    public void Copy(bool deleteAfter = false)
    {
        var hits = BlueprintContainer.SelectionHandler.SelectedObjects.OfType<HitObject>().Select(h => h.Copy()).ToList();

        if (!hits.Any())
        {
            notifications.SendSmallText("Nothing selected.", FontAwesome6.Solid.XMark);
            return;
        }

        var minTime = hits.Min(x => x.Time);

        foreach (var hit in hits)
            hit.Time -= minTime;

        var content = new EditorClipboardContent { HitObjects = hits };
        clipboard.SetText(content.Serialize());

        if (deleteAfter)
        {
            notifications.SendSmallText($"Cut {content.HitObjects.Count} hit objects.", FontAwesome6.Solid.Check);
            BlueprintContainer.SelectionHandler.DeleteSelected();
        }
        else
            notifications.SendSmallText($"Copied {content.HitObjects.Count} hit objects.", FontAwesome6.Solid.Check);
    }

    public void Paste()
    {
        var content = clipboard.GetText()?.Deserialize<EditorClipboardContent>();

        if (content == null)
        {
            notifications.SendSmallText("Clipboard is empty.", FontAwesome6.Solid.XMark);
            return;
        }

        BlueprintContainer.SelectionHandler.DeselectAll();

        foreach (var hitObject in content.HitObjects)
        {
            hitObject.Time += (float)clock.CurrentTime;
        }

        actions.Add(new NotePasteAction(content.HitObjects.ToArray(), map));

        notifications.SendSmallText($"Pasted {content.HitObjects.Count} hit objects.", FontAwesome6.Solid.Check);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Undo:
                actions.Undo();
                return true;

            case FluXisGlobalKeybind.Redo:
                actions.Redo();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
