using System;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Game.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;

public partial class SelectionBlueprint : Container
{
    [Resolved]
    private EditorPlayfield playfield { get; set; }

    private EditorHitObjectContainer hitObjectContainer => playfield.HitObjectContainer;

    public HitObjectInfo HitObject { get; }
    public EditorHitObject Drawable { get; internal set; }

    public event Action<SelectionBlueprint> Selected;
    public event Action<SelectionBlueprint> Deselected;

    private SelectedState state;
    public event Action<SelectedState> StateChanged;

    public bool IsSelected => State == SelectedState.Selected;
    public virtual Vector2 ScreenSpaceSelectionPoint => ScreenSpaceDrawQuad.Centre;

    public override bool HandlePositionalInput => ShouldBeAlive;
    public override bool RemoveWhenNotAlive => false;

    protected SelectionBlueprint(HitObjectInfo info)
    {
        HitObject = info;
        AlwaysPresent = true;
        Width = 80;
    }

    protected override void Update()
    {
        base.Update();

        Anchor = Origin = Anchor.BottomCentre;
        foreach (var child in InternalChildren)
            child.Anchor = child.Origin = Anchor.BottomCentre;

        Position = Parent.ToLocalSpace(hitObjectContainer.ScreenSpacePositionAtTime(HitObject.Time, HitObject.Lane)) - AnchorPosition;
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

    protected virtual void OnDeselected()
    {
        foreach (var drawable in InternalChildren)
            drawable.Hide();
    }

    protected virtual void OnSelected()
    {
        foreach (var drawable in InternalChildren)
            drawable.Show();
    }

    protected override bool ShouldBeConsideredForInput(Drawable child) => State == SelectedState.Selected;
    public void Select() => State = SelectedState.Selected;
    public void Deselect() => State = SelectedState.Deselected;
}
