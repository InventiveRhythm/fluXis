using System;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Shared.Points;
using osu.Framework.Bindables;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Tags;

public partial class EditorTagDependencies
{
    private Action<int> changeTabAction;

    public PointsSidebar ChartingPoints;
    public PointsSidebar DesignPoints;

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

    public void ShowPointInCharting(ITimedObject obj)
    {
        ChangeTab(1);
        ChartingPoints.ShowPoint(obj);
    }
    
    public void ShowPointInDesign(ITimedObject obj)
    {
        ChangeTab(2);
        DesignPoints.ShowPoint(obj);
    }
}