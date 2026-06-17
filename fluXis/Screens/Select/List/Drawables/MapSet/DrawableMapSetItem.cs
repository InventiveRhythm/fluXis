using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Screens.Select.List.Items;
using fluXis.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Utils;

namespace fluXis.Screens.Select.List.Drawables.MapSet;

#nullable enable

public partial class DrawableMapSetItem : CompositeDrawable
{
    public Action? SelectAction;
    public Action<RealmMap>? EditAction;
    public Action<RealmMapSet>? ExportAction;
    public Action<RealmMapSet>? DeleteAction;

    private readonly MapSetItem item;
    private readonly RealmMapSet set;
    private readonly List<RealmMap> maps;

    private SelectedState selectedState = SelectedState.Deselected;

    private DrawableMapSetHeader header = null!;
    private Container<DrawableMapSetDifficulty> difficultyFlow = null!;

    public DrawableMapSetItem(MapSetItem item, RealmMapSet set, List<RealmMap> maps)
    {
        this.item = item;
        this.set = set;
        this.maps = maps;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        AutoSizeDuration = 300;
        AutoSizeEasing = Easing.OutQuint;

        InternalChildren = new Drawable[]
        {
            difficultyFlow = new Container<DrawableMapSetDifficulty>
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding { Horizontal = 10 }
            },
            header = new DrawableMapSetHeader(this, set)
        };

        foreach (var map in maps)
        {
            difficultyFlow.Add(new DrawableMapSetDifficulty(this, map)
            {
                RequestedResort = () =>
                {
                    var children = difficultyFlow.Children.ToList();
                    difficultyFlow.Clear(false);
                    children.Sort();
                    difficultyFlow.Children = children;
                }
            });
        }
    }

    protected override void LoadComplete()
    {
        item.State.BindValueChanged(updateSelected, true);

        base.LoadComplete();
    }

    protected override void Update()
    {
        base.Update();

        if (Precision.AlmostEquals(item.Position, Y))
            Y = item.Position;
        else
            Y = (float)Interpolation.Lerp(item.Position, Y, Math.Exp(-0.01 * Time.Elapsed));

        var selected = item.State.Value == SelectedState.Selected;
        var pos = selected ? 85f : 20;

        foreach (var difficulty in difficultyFlow)
        {
            difficulty.TargetY = pos;
            if (selected) pos += 48 + 5;

            difficulty.UpdatePosition(Time.Elapsed);
            difficulty.Alpha = difficulty.Y <= 30 ? 0 : 1;
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        item.State.ValueChanged -= updateSelected;

        SelectAction = null;
        EditAction = null;
        ExportAction = null;
        DeleteAction = null;
    }

    private void updateSelected(ValueChangedEvent<SelectedState> v)
    {
        if (v.NewValue == SelectedState.Selected)
            select();
        else
            deselect();
    }

    private void select()
    {
        if (selectedState == SelectedState.Selected)
            return;

        header.Show();
        selectedState = SelectedState.Selected;
    }

    private void deselect()
    {
        if (selectedState == SelectedState.Deselected)
            return;

        header.Hide();
        selectedState = SelectedState.Deselected;
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (item.State.Value == SelectedState.Selected)
            return false; // dont handle clicks when we already selected this mapset

        item.Select();
        return true;
    }
}
