using System;
using fluXis.UI;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Edit.Blueprints.Selection;

public partial class SelectionBlueprint<T> : Container
{
    public T Object { get; }
    public Drawable Drawable { get; internal set; }

    public event Action<SelectionBlueprint<T>> Selected;
    public event Action<SelectionBlueprint<T>> Deselected;

    public virtual double FirstComparer => 0;
    public virtual double SecondComparer => 0;

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

            StateChanged?.Invoke(state);
        }
    }

    private SelectedState state;
    public event Action<SelectedState> StateChanged;

    public bool IsSelected => State == SelectedState.Selected;
    public virtual Vector2 ScreenSpaceSelectionPoint => ScreenSpaceDrawQuad.Centre;

    public override bool HandlePositionalInput => ShouldBeAlive;
    public override bool RemoveWhenNotAlive => false;

    protected SelectionBlueprint(T info)
    {
        Object = info;
        AlwaysPresent = true;
        Anchor = Origin = Anchor.BottomLeft;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        updateState();
    }

    private void updateState()
    {
        switch (state)
        {
            case SelectedState.Selected:
                OnSelected();
                Selected?.Invoke(this);
                break;

            case SelectedState.Deselected:
                OnDeselected();
                Deselected?.Invoke(this);
                break;
        }
    }

    protected void OnDeselected()
    {
        foreach (var drawable in InternalChildren)
            drawable.Hide();
    }

    protected void OnSelected()
    {
        foreach (var drawable in InternalChildren)
            drawable.Show();
    }

    public void Select() => State = SelectedState.Selected;
    public void Deselect() => State = SelectedState.Deselected;

    protected override bool ShouldBeConsideredForInput(Drawable child) => State == SelectedState.Selected;
    protected override bool OnHover(HoverEvent e) => false;
}
