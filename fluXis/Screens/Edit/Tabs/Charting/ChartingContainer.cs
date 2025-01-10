using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites;
using fluXis.Map.Structures;
using fluXis.Overlay.Notifications;
using fluXis.Screens.Edit.Actions.Notes;
using fluXis.Screens.Edit.Actions.Notes.Shortcuts;
using fluXis.Screens.Edit.Input;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Screens.Edit.Tabs.Charting.Points;
using fluXis.Screens.Edit.Tabs.Charting.Toolbox;
using fluXis.Screens.Edit.Tabs.Charting.Tools;
using fluXis.Screens.Edit.Tabs.Charting.Tools.Effects;
using fluXis.Screens.Edit.Tabs.Shared;
using fluXis.Screens.Edit.Tabs.Shared.Points;
using fluXis.Screens.Edit.Tabs.Shared.Toolbox;
using fluXis.Screens.Gameplay.Audio.Hitsounds;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Charting;

public partial class ChartingContainer : EditorTabContainer, IKeyBindingHandler<PlatformAction>
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
    public static readonly Key[] TOP_ROW_KEYS = { Key.Q, Key.W, Key.E, Key.R, Key.T, Key.Y, Key.U, Key.I, Key.O, Key.P };

    [Resolved]
    private EditorSettings settings { get; set; }

    [Resolved]
    private Clipboard clipboard { get; set; }

    [Resolved]
    private EditorSnapProvider snaps { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    public Bindable<string> CurrentHitSound { get; } = new($"{Hitsounding.DEFAULT_PREFIX}normal");

    private DependencyContainer dependencies;
    private InputManager inputManager;
    private bool recordingInput;

    private ToolboxHitsoundCategory toolboxHitsounds;
    private PointsSidebar sidebar;

    public EditorPlayfield Playfield { get; private set; }
    public ChartingBlueprintContainer BlueprintContainer { get; private set; }
    public IEnumerable<EditorHitObject> HitObjects => Playfield.HitObjectContainer.HitObjects;
    public bool CursorInPlacementArea => Playfield.ReceivePositionalInputAt(inputManager.CurrentState.Mouse.Position);

    public bool CanFlipSelection => BlueprintContainer.SelectionHandler.SelectedObjects.Any(x => x is HitObject);
    public bool CanShuffleSelection => BlueprintContainer.SelectionHandler.SelectedObjects.Any(x => x is HitObject);

    protected override void BeforeLoad()
    {
        Editor.ChartingContainer = this;

        dependencies.Cache(this);
        dependencies.CacheAs(Playfield = new EditorPlayfield());
        dependencies.CacheAs<ITimePositionProvider>(Playfield);
        dependencies.CacheAs(Playfield.HitObjectContainer);
        dependencies.CacheAs(sidebar = new ChartingSidebar());
    }

    protected override IEnumerable<Drawable> CreateContent()
    {
        return new Drawable[]
        {
            Playfield,
            BlueprintContainer = new ChartingBlueprintContainer { ChartingContainer = this }
        };
    }

    protected override EditorToolbox CreateToolbox() => new()
    {
        Categories = new ToolboxCategory[]
        {
            new()
            {
                Title = "Tools",
                ExtraTitle = "(1-4)",
                Icon = FontAwesome6.Solid.Pen,
                Tools = Tools
            },
            new()
            {
                Title = "Effects",
                Icon = FontAwesome6.Solid.WandMagicSparkles,
                Tools = EffectTools
            },
            toolboxHitsounds = new ToolboxHitsoundCategory()
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
        if (TOP_ROW_KEYS.Contains(e.Key))
        {
            var idx = Array.IndexOf(TOP_ROW_KEYS, e.Key);
            var items = toolboxHitsounds.Items;

            if (idx != -1 && idx < items.Count)
            {
                var sound = items[idx];
                sound.TriggerClick();
            }
        }

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

    private void placeNote(int lane)
    {
        if (lane > Map.RealmMap.KeyCount)
            return;

        var time = EditorClock.CurrentTime;
        var snapped = snaps.SnapTime(time);

        var tp = Map.MapInfo.GetTimingPoint(time);
        var increase = tp.Signature * tp.MsPerBeat / (4 * settings.SnapDivisor);
        var next = snaps.SnapTime(time + increase);

        // take the closest snap
        time = Math.Abs(time - snapped) < Math.Abs(time - next) ? snapped : next;

        var note = new HitObject
        {
            Time = time,
            HitSound = CurrentHitSound.Value,
            Lane = lane
        };

        ActionStack.Add(new NotePlaceAction(note));
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
    {
        return dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
    }

    public override bool OnPressed(KeyBindingPressEvent<EditorKeybinding> e)
    {
        switch (e.Action)
        {
            case EditorKeybinding.FlipSelection:
                FlipSelection();
                return true;

            case EditorKeybinding.ShuffleSelection:
                ShuffleSelection();
                return true;
        }

        return base.OnPressed(e);
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

    public void FlipSelection()
    {
        var objects = BlueprintContainer.SelectionHandler.SelectedObjects.OfType<HitObject>().ToList();

        if (!objects.Any())
        {
            notifications.SendSmallText("Nothing selected.", FontAwesome6.Solid.XMark);
            return;
        }

        ActionStack.Add(new NoteFlipAction(objects, Map.RealmMap.KeyCount));
    }

    public void ShuffleSelection()
    {
        var objects = BlueprintContainer.SelectionHandler.SelectedObjects.OfType<HitObject>().ToList();

        if (!objects.Any())
        {
            notifications.SendSmallText("Nothing selected.", FontAwesome6.Solid.XMark);
            return;
        }

        ActionStack.Add(new NoteShuffleAction(objects, Map.RealmMap.KeyCount));
    }

    public void ReSnapAll()
    {
        var objects = BlueprintContainer.SelectionHandler.SelectedObjects.OfType<HitObject>().ToList();

        if (!objects.Any())
            objects = HitObjects.Select(h => h.Data).ToList();

        ActionStack.Add(new NoteReSnapAction(objects, snaps.SnapTime, settings.SnapDivisor));
    }

    public void Copy(bool deleteAfter = false)
    {
        var hits = BlueprintContainer.SelectionHandler.SelectedObjects.OfType<HitObject>().Select(h => h.JsonCopy()).ToList();

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
        // ReSharper disable once RedundantAssignment
        EditorClipboardContent content = null;

        if ((!clipboard.GetText()?.TryDeserialize(out content) ?? true) || !content.HitObjects.Any())
        {
            notifications.SendSmallText("Clipboard is empty.", FontAwesome6.Solid.XMark);
            return;
        }

        BlueprintContainer.SelectionHandler.DeselectAll();

        foreach (var hitObject in content.HitObjects)
        {
            hitObject.Time += EditorClock.CurrentTime;
        }

        ActionStack.Add(new NotePasteAction(content.HitObjects.ToArray()));

        notifications.SendSmallText($"Pasted {content.HitObjects.Count} hit objects.", FontAwesome6.Solid.Check);
    }
}
