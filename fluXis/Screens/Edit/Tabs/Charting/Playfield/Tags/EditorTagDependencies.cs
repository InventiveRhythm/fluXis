using System;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Charting.Points;
using fluXis.Screens.Edit.Tabs.Design.Points;
using fluXis.Screens.Edit.Tabs.Shared.Points;
using osu.Framework.Bindables;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Tags;

public partial class EditorTagDependencies
{
    private Action<EditorTabType> changeTabAction;

    public ChartingSidebar ChartingPointsSidebar;
    public DesignSidebar DesignPointsSidebar;

    public Bindable<EditorTabType> CurrentTab { get; }

    public EditorTagDependencies(Bindable<EditorTabType> currentTab, Action<EditorTabType> changeTab)
    {
        CurrentTab = currentTab;
        changeTabAction = changeTab;
    }

    public void ChangeTab(EditorTabType tab)
    {
        changeTabAction?.Invoke(tab);
    }

    public void ShowPointFrom(PointsSidebar sidebar, ITimedObject obj)
    {
        switch (sidebar)
        {
            case ChartingSidebar:
                if (CurrentTab.Value is EditorTabType.Charting)
                    ChartingPointsSidebar.ShowPoint(obj);
                else
                    DesignPointsSidebar.ShowPoint(obj, sidebar);
                break;

            case DesignSidebar:
                if (CurrentTab.Value is EditorTabType.Design)
                    DesignPointsSidebar.ShowPoint(obj);
                else
                    ChartingPointsSidebar.ShowPoint(obj, sidebar);
                break;
        }
    }
}
