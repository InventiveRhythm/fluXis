using System;
using System.Collections.Generic;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Screens.Edit.Input;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Shared.Points;

public abstract partial class PointsSidebar : ExpandingContainer, IKeyBindingHandler<FluXisGlobalKeybind>, IKeyBindingHandler<EditorKeybinding>
{
    private const int size_closed = 190;
    private const int size_open = 420;

    protected override double HoverDelay => 250;
    protected override bool CloseOnHoverLost => false;

    public Action OnWrapperClick { get; set; }

    [Resolved]
    protected EditorMap Map { get; private set; }

    private bool showingSettings;

    private Drawable closedContent;
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

        InternalChildren = new[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            closedContent = CreateClosedContent(),
            pointsList = CreatePointsList().With(l =>
            {
                l.Alpha = 0;
                l.ShowSettings = showPointSettings;
                l.RequestClose = close;
            }),
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

        Expanded.BindValueChanged(v =>
        {
            this.ResizeWidthTo(v.NewValue ? size_open : size_closed, 500, Easing.OutQuart);

            if (v.NewValue)
            {
                closedContent.FadeOut(200);
                pointsList.Delay(200).FadeIn(200);
            }
            else
            {
                closedContent.Delay(200).FadeIn(200);
                pointsList.FadeOut(200);
            }
        }, true);
    }

    protected abstract PointsList CreatePointsList();
    protected virtual Drawable CreateClosedContent() => Empty();

    public void ShowPoint(ITimedObject obj)
    {
        Expanded.Value = true;
        pointsList.ShowPoint(obj);
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
            Locked.Value = false;
            return;
        }

        showingSettings = false;
        Locked.Value = false;

        settingsWrapper.FadeOut(200).ScaleTo(1.2f, 400, Easing.OutQuint);
        pointsList.FadeIn(200).ScaleTo(1, 400, Easing.OutQuint);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        // prevent sidebar from closing when using context menus
        if (e.Button == MouseButton.Right)
            Locked.Value = true;

        return false;
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

    public bool OnPressed(KeyBindingPressEvent<EditorKeybinding> e)
    {
        if (e.Action == EditorKeybinding.ToggleSidebar)
        {
            if (showingSettings)
                return false;

            if (Expanded.Value)
                close();
            else
                Expanded.Value = true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<EditorKeybinding> e) { }
    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
