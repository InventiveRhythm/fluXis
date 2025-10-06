using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Select.Search.Dropdown;

public abstract partial class FluXisFilterButtonsBase<T> : CompositeDrawable where T : struct
{
    private readonly InputManager input;
    public System.Action OnFilterChanged;

    protected FillFlowContainer ButtonFlow;
    protected FluXisSpriteText Text;

    protected abstract T[] Values { get; }
    protected abstract string Label { get; }
    protected abstract float FontSize { get; set; }
    protected abstract List<T> FilterList { get; }
    public abstract T[] DefaultFilter { get; set; }

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

        if (FilterList.Count == 0)
            FilterList.AddRange(DefaultFilter);
        
        OnFilterChanged?.Invoke();
        
        UpdateAllButtons();
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (e.Button == osuTK.Input.MouseButton.Right)
        {
            ResetState();
            foreach (var button in ButtonFlow.Children.OfType<ISelectableButton<T>>())
                button.Flash(); 
        }

        return base.OnMouseDown(e);
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
        void Flash() {}
    }
}