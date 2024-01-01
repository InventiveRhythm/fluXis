using System;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Game.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Selection;

public partial class SelectionBlueprint : Container
{
    [Resolved]
    private EditorPlayfield playfield { get; set; }

    protected EditorHitObjectContainer HitObjectContainer => playfield.HitObjectContainer;

    public TimedObject Object { get; }
    public Drawable Drawable { get; internal set; }

    public event Action<SelectionBlueprint> Selected;
    public event Action<SelectionBlueprint> Deselected;

    public virtual float FirstComparer => Object.Time;
    public virtual float SecondComparer => Object.Time;

    private SelectedState state;
    public event Action<SelectedState> StateChanged;

    public bool IsSelected => State == SelectedState.Selected;
    public virtual Vector2 ScreenSpaceSelectionPoint => ScreenSpaceDrawQuad.Centre;

    public override bool HandlePositionalInput => ShouldBeAlive;
    public override bool RemoveWhenNotAlive => false;

    protected SelectionBlueprint(TimedObject info)
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

    protected override bool ShouldBeConsideredForInput(Drawable child) => State == SelectedState.Selected;
    public void Select() => State = SelectedState.Selected;
    public void Deselect() => State = SelectedState.Deselected;

    protected override bool OnHover(HoverEvent e) => true;
}
