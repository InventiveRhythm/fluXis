using System;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Shared.Points;
using osu.Framework.Bindables;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Tags;

public partial class EditorTagDependencies
{
    private Action<int> changeTabAction;

    public PointsSidebar ChartingPointsSidebar;
    public PointsSidebar DesignPointsSidebar;

    public Bindable<int> CurrentTab { get; }

    public EditorTagDependencies(Bindable<int> currentTab, Action<int> changeTab)
    {
        CurrentTab = currentTab;
        changeTabAction = changeTab;
    }

    public void ChangeTab(int tab)
    {
        changeTabAction?.Invoke(tab);
    }

    public void ShowPointIn(ITimedObject obj, PointsSidebar inSidebar, PointsSidebar fromSidebar)
        => inSidebar.ShowPoint(obj, fromSidebar);
}