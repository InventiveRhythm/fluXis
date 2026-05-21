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
using osu.Framework.Graphics.Primitives;
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
    private Container<DrawableMapSetDifficulty>? difficultyFlow;

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
            new DelayedLoadUnloadWrapper(() =>
            {
                var flow = new Container<DrawableMapSetDifficulty>
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Padding = new MarginPadding { Horizontal = 10 }
                };

                foreach (var map in maps)
                {
                    flow.Add(new DrawableMapSetDifficulty(this, map)
                    {
                        RequestedResort = () =>
                        {
                            var children = flow.Children.ToList();
                            flow.Clear(false);
                            children.Sort();
                            flow.Children = children;
                        }
                    });
                }

                difficultyFlow = flow;
                return flow;
            }, 0, 250),
            new MapSetLoadWrapper(() => header = new DrawableMapSetHeader(this, set), 0)
            {
                RelativeSizeAxes = Axes.X,
                Height = DrawableMapSetHeader.HEIGHT
            }
        };
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

        if (difficultyFlow is not { IsAlive: true }) return;

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

        header?.Show();
        selectedState = SelectedState.Selected;
    }

    private void deselect()
    {
        if (selectedState == SelectedState.Deselected)
            return;

        header?.Hide();
        selectedState = SelectedState.Deselected;
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (item.State.Value == SelectedState.Selected)
            return false; // dont handle clicks when we already selected this mapset

        item.Select();
        return true;
    }

    // a trick so it doesn't unload early if it's at the bottom or top of the scroll container
    private partial class MapSetLoadWrapper : DelayedLoadUnloadWrapper
    {
        public float Pad { get; set; } = 250f;

        public MapSetLoadWrapper(Func<Drawable> createContentAction, double timeBeforeLoad = 0, double timeBeforeUnload = 1000)
            : base(createContentAction, timeBeforeLoad, timeBeforeUnload)
        {
        }

        public override Quad ScreenSpaceDrawQuad
        {
            get
            {
                var rect = base.ScreenSpaceDrawQuad.AABBFloat;

                return new RectangleF(
                    rect.X,
                    rect.Y - Pad,
                    rect.Width,
                    rect.Height + (Pad * 2)
                );
            }
        }
    }
}
