using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osuTK;

namespace fluXis.Screens.Select.Search.Dropdown;

public abstract partial class FluXisFilterButtonsBase<T> : CompositeDrawable where T : struct
{
    private readonly InputManager input;
    public Action OnFilterChanged;

    protected FillFlowContainer ButtonFlow;
    protected FluXisSpriteText Text;

    protected abstract T[] Values { get; }
    protected abstract string Label { get; }
    protected abstract float FontSize { get; set; }
    protected abstract List<T> FilterList { get; }
    public abstract T[] DefaultFilter { get; set; }
    public abstract bool ResetWhenFull { get; set; }

    protected FluXisFilterButtonsBase(InputManager input, System.Action onFilterChanged = null)
    {
        this.input = input;
        OnFilterChanged = onFilterChanged;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 30;
        Padding = new MarginPadding { Horizontal = 5 };

        InternalChildren = new Drawable[]
        {
            Text = new FluXisSpriteText
            {
                Text = Label,
                FontSize = FontSize,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft
            },
            ButtonFlow = new FillFlowContainer
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(5),
                ChildrenEnumerable = Values.Select(CreateButton)
            }
        };

        if (DefaultFilter.Length != 0)
            ResetState();

        ButtonFlow.Children.OfType<ISelectableButton<T>>().ForEach(b => b.OnRightClick = ResetAllButtons);
    }

    protected abstract Drawable CreateButton(T value);

    protected void OnValueClick(T value)
    {
        bool isCtrlPressed = input.CurrentState.Keyboard.ControlPressed;

        if (FilterList.Count == Values.Length && DefaultFilter.Length == 0)
        {
            FilterList.Clear();
            FilterList.AddRange(Values);
        }

        if (isCtrlPressed && (FilterList.Count == 0 || FilterList.Count == Values.Length))
        {
            FilterList.Clear();
            FilterList.AddRange(Values);
        }

        if (!FilterList.Remove(value))
            FilterList.Add(value);

        if (FilterList.Count == 0 || (FilterList.Count == Values.Length && ResetWhenFull))
            ResetState();
        
        OnFilterChanged?.Invoke();
        
        UpdateAllButtons();
    }

    protected void ResetAllButtons(ISelectableButton<T> clickedButton)
    {
        if (FilterList.ToHashSet().SetEquals(DefaultFilter.ToHashSet()))
            return;

        ResetState();
        
        var buttons = ButtonFlow.Children.OfType<ISelectableButton<T>>().ToList();
        int clickedIndex = buttons.IndexOf(clickedButton);

        if (clickedIndex == -1)
            clickedIndex = buttons.Count / 2;
            
        double lastDelay = 0;

        for (int i = 0; i < buttons.Count; i++)
        {
            int distance = Math.Abs(i - clickedIndex);
            double delay = distance * 30;
            lastDelay = delay;
            int index = i;

            Scheduler.AddDelayed(() => buttons[index].Flash(), delay);
        }
    }

    protected void ResetState()
    {
        FilterList.Clear();
        FilterList.AddRange(DefaultFilter);
        OnFilterChanged?.Invoke();
        UpdateAllButtons();
    }

    protected void UpdateAllButtons()
    {
        foreach (var button in ButtonFlow.Children.OfType<ISelectableButton<T>>())
            button.UpdateSelection();
    }

    protected interface ISelectableButton<TValue>
    {
        void UpdateSelection();
        void Flash();
        Action<ISelectableButton<TValue>> OnRightClick { get; set; }
    }
}