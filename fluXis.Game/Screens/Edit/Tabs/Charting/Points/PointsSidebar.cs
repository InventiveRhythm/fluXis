using System;
using System.Collections.Generic;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using fluXis.Game.Screens.Edit.Tabs.Charting.Points.List;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Points;

public partial class PointsSidebar : ExpandingContainer, IKeyBindingHandler<FluXisGlobalKeybind>
{
    private const int size_closed = 120;
    private const int size_open = 420;

    protected override double HoverDelay => 500;

    public Action OnWrapperClick { get; set; }

    private bool showingSettings;

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
            pointsList = new PointsList
            {
                Alpha = 0,
                ShowSettings = showPointSettings
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

        OnWrapperClick = closeSettings;

        Expanded.BindValueChanged(v =>
        {
            this.ResizeWidthTo(v.NewValue ? size_open : size_closed, 500, Easing.OutQuart);

            if (v.NewValue)
                pointsList.Delay(200).FadeIn(200);
            else
                pointsList.FadeOut(200);
        }, true);
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

    private void closeSettings()
    {
        if (!showingSettings) return;

        showingSettings = false;
        Locked.Value = false;

        settingsWrapper.FadeOut(200).ScaleTo(1.2f, 400, Easing.OutQuint);
        pointsList.FadeIn(200).ScaleTo(1, 400, Easing.OutQuint);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Action != FluXisGlobalKeybind.Back || !showingSettings)
            return false;

        closeSettings();
        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
