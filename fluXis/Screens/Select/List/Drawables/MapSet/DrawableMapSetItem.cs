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

    private bool alreadyLoadedOnce;
    private SelectedState selectedState = SelectedState.Deselected;

    private DrawableMapSetHeader header = null!;
    private Container<DrawableMapSetDifficulty>? difficultyFlow;
    private ExpandedLoadWrapper difficultyWrapper = null!;

    private const float unselected_pos = 20f;
    private const float selected_pos = 85f;
    private const float pos_velocity = 48 + 5;

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
            difficultyWrapper = new ExpandedLoadWrapper(() =>
            {
                var flow = new Container<DrawableMapSetDifficulty>
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Padding = new MarginPadding { Horizontal = 10 }
                };

                var isSelected = item.State.Value == SelectedState.Selected;
                float initialPos = selected_pos;

                foreach (var map in maps)
                {
                    flow.Add(new DrawableMapSetDifficulty(this, map)
                    {
                        Y = alreadyLoadedOnce ? (isSelected ? initialPos : unselected_pos) : 0,
                        RequestedResort = () =>
                        {
                            var children = flow.Children.ToList();
                            flow.Clear(false);
                            children.Sort();
                            flow.Children = children;
                        }
                    });

                    if (alreadyLoadedOnce && isSelected) initialPos += pos_velocity;
                }

                alreadyLoadedOnce = true;
                difficultyFlow = flow;
                return flow;
            }, 0, 250),
            new ExpandedLoadWrapper(() => header = new DrawableMapSetHeader(this, set), 0)
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
        var pos = selected ? selected_pos : unselected_pos;

        foreach (var difficulty in difficultyFlow)
        {
            difficulty.TargetY = pos;
            if (selected) pos += pos_velocity;

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
        difficultyWrapper.AllowUnloading = false;
        selectedState = SelectedState.Selected;
    }

    private void deselect()
    {
        if (selectedState == SelectedState.Deselected)
            return;

        header?.Hide();
        difficultyWrapper.AllowUnloading = true;
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
    private partial class ExpandedLoadWrapper : DelayedLoadUnloadWrapper
    {
        public float Pad { get; set; } = 250f;

        public ExpandedLoadWrapper(Func<Drawable> createContentAction, double timeBeforeLoad = 0, double timeBeforeUnload = 1000)
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
