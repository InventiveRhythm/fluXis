using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Charting.Points.List;
using fluXis.Game.Screens.Edit.Tabs.Charting.Selection;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Extensions.TypeExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Points;

public partial class PointsSidebar : ExpandingContainer, IKeyBindingHandler<FluXisGlobalKeybind>
{
    private const int size_closed = 190;
    private const int size_open = 420;

    protected override double HoverDelay => 500;

    public Action OnWrapperClick { get; set; }

    [Resolved]
    private ChartingContainer chartingContainer { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    private SelectionHandler selectionHandler => chartingContainer.BlueprintContainer.SelectionHandler;

    private bool showingSettings;

    private SelectionInspector inspector;
    private PointsList pointsList;
    private ClickableContainer settingsWrapper;
    private FillFlowContainer settingsFlow;

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 320;
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.TopRight;
        Origin = Anchor.TopRight;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            inspector = new SelectionInspector(),
            pointsList = new PointsList
            {
                Alpha = 0,
                ShowSettings = showPointSettings,
                RequestClose = close
            },
            settingsWrapper = new ClickableContainer
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2,
                        Size = new Vector2(1.5f)
                    },
                    new FluXisScrollContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        ScrollbarVisible = false,
                        Child = settingsFlow = new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Spacing = new Vector2(20),
                            Padding = new MarginPadding(20)
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        OnWrapperClick = close;

        selectionHandler.SelectedObjects.CollectionChanged += (_, _) => updateSelection();
        updateSelection();

        Expanded.BindValueChanged(v =>
        {
            this.ResizeWidthTo(v.NewValue ? size_open : size_closed, 500, Easing.OutQuart);

            if (v.NewValue)
            {
                inspector.FadeOut(200);
                pointsList.Delay(200).FadeIn(200);
            }
            else
            {
                inspector.Delay(200).FadeIn(200);
                pointsList.FadeOut(200);
            }
        }, true);
    }

    public void ShowPoint(ITimedObject obj)
    {
        Expanded.Value = true;
        pointsList.ShowPoint(obj);
    }

    private void updateSelection()
    {
        inspector.Clear();

        switch (selectionHandler.SelectedObjects.Count)
        {
            case 0:
                inspector.AddSection("Nothing selected", "");
                break;

            case 1:
            {
                var selected = selectionHandler.SelectedObjects.Single();
                inspector.AddSection("Type", selected.GetType().ReadableName());
                inspector.AddSection("Time", TimeUtils.Format(selected.Time));

                if (selected is HitObject hit)
                {
                    inspector.AddSection("Hit Type", hit.Type switch
                    {
                        0 when !hit.LongNote => "Single",
                        0 when hit.LongNote => "Long",
                        1 => "Tick",
                        _ => "Unknown"
                    });

                    inspector.AddSection("Lane", $"{hit.Lane}");

                    switch (hit.Type)
                    {
                        case 0 when hit.LongNote:
                            inspector.AddSection("Length", $"{hit.HoldTime:#,0.##}ms");
                            inspector.AddSection("End Time", TimeUtils.Format(hit.EndTime));
                            break;
                    }
                }

                break;
            }

            default:
                inspector.AddSection("Selected", $"{selectionHandler.SelectedObjects.Count} objects");
                inspector.AddSection("Start", TimeUtils.Format(selectionHandler.SelectedObjects.Min(o => o.Time)));
                inspector.AddSection("End", TimeUtils.Format(selectionHandler.SelectedObjects.Max(o => o.Time)));

                if (selectionHandler.SelectedObjects.All(o => o is HitObject))
                {
                    var evenCount = map.RealmMap.KeyCount % 2 == 0;
                    var laneCount = map.RealmMap.KeyCount / 2;
                    var middleLane = laneCount + 1;

                    var leftCount = selectionHandler.SelectedObjects.Count(o => ((HitObject)o).Lane <= laneCount);
                    var middleCount = selectionHandler.SelectedObjects.Count(o => ((HitObject)o).Lane == middleLane);
                    var rightCount = evenCount
                        ? selectionHandler.SelectedObjects.Count(o => ((HitObject)o).Lane > laneCount)
                        : selectionHandler.SelectedObjects.Count(o => ((HitObject)o).Lane > middleLane);

                    var leftPercentage = (float)leftCount / selectionHandler.SelectedObjects.Count * 100;
                    var middlePercentage = (float)middleCount / selectionHandler.SelectedObjects.Count * 100;
                    var rightPercentage = (float)rightCount / selectionHandler.SelectedObjects.Count * 100;

                    inspector.AddSection("Side Balance - Left", $"{leftCount} ({leftPercentage:0.##}%)");

                    if (!evenCount)
                        inspector.AddSection("Side Balance - Middle", $"{middleCount} ({middlePercentage:0.##}%)");

                    inspector.AddSection("Side Balance - Right", $"{rightCount} ({rightPercentage:0.##}%)");
                }

                break;
        }
    }

    private void showPointSettings(IEnumerable<Drawable> drawables)
    {
        if (showingSettings) return;

        showingSettings = true;
        Locked.Value = true;

        settingsFlow.Clear();
        settingsFlow.AddRange(drawables);

        pointsList.FadeOut(200).ScaleTo(.9f, 400, Easing.OutQuint);
        settingsWrapper.ScaleTo(1.1f).FadeInFromZero(200).ScaleTo(1, 400, Easing.OutQuint);
    }

    private void close()
    {
        if (!showingSettings)
        {
            Expanded.Value = false;
            return;
        }

        showingSettings = false;
        Locked.Value = false;

        settingsWrapper.FadeOut(200).ScaleTo(1.2f, 400, Easing.OutQuint);
        pointsList.FadeIn(200).ScaleTo(1, 400, Easing.OutQuint);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Action != FluXisGlobalKeybind.Back)
            return false;

        if (!showingSettings && !Expanded.Value)
            return false;

        close();
        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
