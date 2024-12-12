using System;
using System.Collections.Generic;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Menus;
using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Screens.Edit.Actions;
using fluXis.Game.Screens.Edit.Actions.Events;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Game.UI;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;

public abstract partial class PointListEntry : Container, IHasContextMenu
{
    protected abstract string Text { get; }
    protected abstract Colour4 Color { get; }

    public MenuItem[] ContextMenuItems => new MenuItem[]
    {
        new FluXisMenuItem("Clone to current time", FontAwesome6.Solid.Clone, clone),
        new FluXisMenuItem("Go to time", FontAwesome6.Solid.ArrowRight, goTo),
        new FluXisMenuItem("Edit", FontAwesome6.Solid.PenRuler, OpenSettings),
        new FluXisMenuItem("Delete", FontAwesome6.Solid.Trash, MenuItemType.Dangerous, () => delete(false))
    };

    public event Action<PointListEntry> Selected;
    public event Action<PointListEntry> Deselected;
    public Bindable<PointListEntry> CurrentEvent { get; set; }

    private SelectedState state;

    public SelectedState State
    {
        get => state;
        set
        {
            if (state == value)
                return;

            state = value;

            if (IsLoaded)
                updateState();
        }
    }

    public Action<IEnumerable<Drawable>> ShowSettings { get; set; }
    public Action RequestClose { get; set; }
    public Action DeleteSelected { get; set; }
    public Action CloneSelected { get; set; }
    public Action<ITimedObject> OnClone { get; set; }
    public ITimedObject Object { get; }

    protected float BeatLength => Map.MapInfo.GetTimingPoint(Object.Time).MsPerBeat;

    [Resolved]
    protected EditorSettings Settings { get; private set; }

    [Resolved]
    protected EditorMap Map { get; private set; }

    [Resolved]
    protected EditorActionStack ActionStack { get; private set; }

    [Resolved]
    private UISamples samples { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    private HoverLayer hover;
    private Circle indicator;
    private FluXisSpriteText timeText;
    private FillFlowContainer valueFlow;

    protected PointListEntry(ITimedObject obj)
    {
        Object = obj;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 32;
        Masking = true;
        CornerRadius = 5;
        BorderColour = Color;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4.White,
                Alpha = 0,
                AlwaysPresent = true
            },
            hover = new HoverLayer { Colour = Color },
            indicator = new Circle
            {
                Size = new Vector2(4, 0),
                Position = new Vector2(3, 0),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Colour = Color
            },
            timeText = new FluXisSpriteText
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Colour = Color,
                X = 10
            },
            valueFlow = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Direction = FillDirection.Horizontal,
                X = -10
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        UpdateValues();

        CurrentEvent.BindValueChanged(currentEventChange, true);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        CurrentEvent.ValueChanged -= currentEventChange;
    }

    private void currentEventChange(ValueChangedEvent<PointListEntry> e)
        => indicator.ResizeHeightTo(e.NewValue == this ? 16 : 0, 600, Easing.OutQuint);

    private void updateState()
    {
        switch (state)
        {
            case SelectedState.Selected:
                BorderThickness = 2;
                Selected?.Invoke(this);
                break;

            case SelectedState.Deselected:
                BorderThickness = 0;
                Deselected?.Invoke(this);
                break;
        }
    }

    public void OpenSettings()
    {
        ShowSettings?.Invoke(CreateSettings());
    }

    public abstract ITimedObject CreateClone();
    protected abstract Drawable[] CreateValueContent();

    protected virtual IEnumerable<Drawable> CreateSettings()
    {
        return new Drawable[]
        {
            new PointSettingsTitle(Text, () => delete()),
            new PointSettingsTime(Map, Object)
        };
    }

    private void clone()
    {
        // let the parent handle the cloning
        if (State == SelectedState.Selected)
        {
            CloneSelected?.Invoke();
            return;
        }

        var clone = CreateClone();
        OnClone?.Invoke(clone);
    }

    private void goTo() => clock.SeekSmoothly(Object.Time);

    private void delete(bool close = true)
    {
        // let the parent handle the deletion
        if (State == SelectedState.Selected)
        {
            DeleteSelected?.Invoke();
            return;
        }

        ActionStack.Add(new EventRemoveAction(Object));

        if (close)
            RequestClose?.Invoke();
    }

    public void UpdateValues()
    {
        timeText.Text = TimeUtils.Format(Object.Time);

        valueFlow.Clear();
        valueFlow.AddRange(CreateValueContent());
        OnValueUpdate();
    }

    protected virtual void OnValueUpdate() { }

    protected override bool OnHover(HoverEvent e)
    {
        samples.Hover();
        hover.Show();
        return false;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
    }

    protected override bool OnClick(ClickEvent e)
    {
        samples.Click();

        if (e.ControlPressed)
            State = State == SelectedState.Selected ? SelectedState.Deselected : SelectedState.Selected;
        else
            OpenSettings();

        return true;
    }
}
