using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Map.Structures;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Screens.Edit.Actions;
using fluXis.Game.Screens.Edit.Actions.Notes;
using fluXis.Game.Screens.Edit.Actions.Notes.Shortcuts;
using fluXis.Game.Screens.Edit.Input;
using fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Game.Screens.Edit.Tabs.Charting.Points;
using fluXis.Game.Screens.Edit.Tabs.Charting.Toolbox;
using fluXis.Game.Screens.Edit.Tabs.Charting.Tools;
using fluXis.Game.Screens.Edit.Tabs.Charting.Tools.Effects;
using fluXis.Game.Screens.Edit.Tabs.Shared;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points;
using fluXis.Game.Screens.Edit.Tabs.Shared.Toolbox;
using fluXis.Game.Screens.Edit.Tabs.Shared.Toolbox.Snap;
using fluXis.Game.Screens.Gameplay.Audio.Hitsounds;
using fluXis.Shared.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Charting;

public partial class ChartingContainer : EditorTabContainer, IKeyBindingHandler<PlatformAction>, IKeyBindingHandler<EditorKeybinding>
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
    private EditorActionStack actions { get; set; }

    [Resolved]
    private EditorSettings settings { get; set; }

    [Resolved]
    private Clipboard clipboard { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    public Bindable<string> CurrentHitSound { get; } = new($"{Hitsounding.DEFAULT_PREFIX}normal");

    private DependencyContainer dependencies;
    private InputManager inputManager;
    private bool recordingInput;

    private PointsSidebar sidebar;

    public EditorPlayfield Playfield { get; private set; }
    public BlueprintContainer BlueprintContainer { get; private set; }
    public IEnumerable<EditorHitObject> HitObjects => Playfield.HitObjectContainer.HitObjects;
    public bool CursorInPlacementArea => Playfield.ReceivePositionalInputAt(inputManager.CurrentState.Mouse.Position);

    public bool CanFlipSelection => BlueprintContainer.SelectionHandler.SelectedObjects.Any(x => x is HitObject);

    protected override void BeforeLoad()
    {
        Editor.ChartingContainer = this;

        dependencies.Cache(this);
        dependencies.CacheAs(Playfield = new EditorPlayfield());
        dependencies.CacheAs(sidebar = new ChartingSidebar());
    }

    protected override IEnumerable<Drawable> CreateContent()
    {
        return new Drawable[]
        {
            Playfield,
            BlueprintContainer = new BlueprintContainer { ChartingContainer = this }
        };
    }

    protected override EditorToolbox CreateToolbox() => new()
    {
        Categories = new ToolboxCategory[]
        {
            new()
            {
                Title = "Tools",
                Icon = FontAwesome6.Solid.Pen,
                Tools = Tools
            },
            new()
            {
                Title = "Effects",
                Icon = FontAwesome6.Solid.WandMagicSparkles,
                Tools = EffectTools
            },
            new ToolboxHitsoundCategory(),
            new ToolboxSnapCategory()
        }
    };

    protected override PointsSidebar CreatePointsSidebar() => sidebar;

    protected override void LoadComplete()
    {
        base.LoadComplete();

        inputManager = GetContainingInputManager();
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

            default:
                return base.OnKeyDown(e);
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
            base.OnScroll(e);

        return true;
    }

    private void placeNote(int lane)
    {
        if (lane > Map.RealmMap.KeyCount)
            return;

        var time = (float)EditorClock.CurrentTime;
        var snapped = Playfield.HitObjectContainer.SnapTime(time);

        var tp = Map.MapInfo.GetTimingPoint(time);
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

        actions.Add(new NotePlaceAction(note, Map));
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
    {
        return dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
    }

    public bool OnPressed(KeyBindingPressEvent<EditorKeybinding> e)
    {
        switch (e.Action)
        {
            case EditorKeybinding.FlipSelection:
                FlipSelection();
                return true;

            case EditorKeybinding.Undo:
                actions.Undo();
                return true;

            case EditorKeybinding.Redo:
                actions.Redo();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<EditorKeybinding> e) { }

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

    public void FlipSelection()
    {
        var objects = BlueprintContainer.SelectionHandler.SelectedObjects.OfType<HitObject>().ToList();

        if (!objects.Any())
        {
            notifications.SendSmallText("Nothing selected.", FontAwesome6.Solid.XMark);
            return;
        }

        actions.Add(new NoteFlipAction(objects, Map.RealmMap.KeyCount));
    }

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
            hitObject.Time += (float)EditorClock.CurrentTime;
        }

        actions.Add(new NotePasteAction(content.HitObjects.ToArray(), Map));

        notifications.SendSmallText($"Pasted {content.HitObjects.Count} hit objects.", FontAwesome6.Solid.Check);
    }
}
