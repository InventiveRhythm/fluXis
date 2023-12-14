using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Select.List;

public partial class MapListEntry : Container
{
    [Resolved]
    private MapStore maps { get; set; }

    public Action SelectAction;
    public Action<RealmMap> EditAction;
    public Action<RealmMapSet> ExportAction;
    public Action<RealmMapSet> DeleteAction;

    public readonly RealmMapSet MapSet;

    public bool Selected => Equals(maps.CurrentMapSet, MapSet);

    public List<RealmMap> Maps
    {
        get
        {
            var diffs = MapSet.Maps.ToList();

            // sorting by nps until we have a proper difficulty system
            diffs.Sort((a, b) => a.Filters.NotesPerSecond.CompareTo(b.Filters.NotesPerSecond));
            return diffs;
        }
    }

    private MapSetHeader header;
    private Container difficultyContainer;
    private FillFlowContainer<MapDifficultyEntry> difficultyFlow;

    public MapListEntry(RealmMapSet mapSet)
    {
        MapSet = mapSet;
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
            difficultyContainer = new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Alpha = 0,
                Padding = new MarginPadding { Horizontal = 10, Top = 85 },
                Child = difficultyFlow = new FillFlowContainer<MapDifficultyEntry>
                {
                    Direction = FillDirection.Vertical,
                    AutoSizeAxes = Axes.Y,
                    RelativeSizeAxes = Axes.X,
                    Spacing = new Vector2(0, 5)
                }
            },
            header = new MapSetHeader(this, MapSet)
        };

        foreach (var map in Maps)
        {
            difficultyFlow.Add(new MapDifficultyEntry(this, map));
        }
    }

    protected override void LoadComplete()
    {
        maps.MapSetBindable.BindValueChanged(updateSelected, true);

        base.LoadComplete();
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        maps.MapSetBindable.ValueChanged -= updateSelected;
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
        header.Show();
        difficultyContainer.FadeIn(200);
    }

    private void deselect()
    {
        header.Hide();
        difficultyContainer.FadeOut(200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (Selected)
            return false; // dont handle clicks when we already selected this mapset

        maps.Select(MapSet.LowestDifficulty, true);
        return true;
    }
}
