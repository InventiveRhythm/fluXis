using System;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Screens.Select.List.Items;
using fluXis.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Utils;

namespace fluXis.Screens.Select.List.Drawables.MapSet;

public partial class DrawableMapSetItem : CompositeDrawable
{
    [Resolved]
    private MapStore maps { get; set; }

    public Action SelectAction;
    public Action<RealmMap> EditAction;
    public Action<RealmMapSet> ExportAction;
    public Action<RealmMapSet> DeleteAction;

    private MapSetItem item { get; }
    public RealmMapSet MapSet { get; private set; }

    private SelectedState selectedState = SelectedState.Deselected;

    public bool Selected => MapSet.ID == maps.MapSetBindable.Value?.ID;

    private DrawableMapSetHeader header;
    private Container<DrawableMapSetDifficulty> difficultyFlow;

    public DrawableMapSetItem(MapSetItem item, RealmMapSet mapSet)
    {
        MapSet = mapSet;
        this.item = item;
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
            header = new DrawableMapSetHeader(this, MapSet)
        };

        foreach (var map in MapSet.MapsSorted)
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
        maps.MapSetBindable.BindValueChanged(updateSelected, true);

        base.LoadComplete();
    }

    protected override void Update()
    {
        base.Update();

        if (Precision.AlmostEquals(item.Position, Y))
            Y = item.Position;
        else
            Y = (float)Interpolation.Lerp(item.Position, Y, Math.Exp(-0.01 * Time.Elapsed));

        if (!Selected)
        {
            difficultyFlow.ForEach(d => d.TargetY = 30);
            return;
        }

        var pos = 85f;

        foreach (var difficulty in difficultyFlow)
        {
            difficulty.TargetY = pos;
            pos += 48 + 5;
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        if (maps is not null)
            maps.MapSetBindable.ValueChanged -= updateSelected;

        SelectAction = null;
        EditAction = null;
        ExportAction = null;
        DeleteAction = null;

        MapSet = null;
    }

    private void updateSelected(ValueChangedEvent<RealmMapSet> set)
    {
        if (Selected)
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
        if (Selected)
            return false; // dont handle clicks when we already selected this mapset

        maps.Select(MapSet.LowestDifficulty, true);
        return true;
    }
}
