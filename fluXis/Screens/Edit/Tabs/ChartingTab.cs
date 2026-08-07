using System;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Charting;
using Humanizer;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Screens.Edit.Tabs;

public partial class ChartingTab : EditorTab
{
    public override IconUsage Icon => Phosphor.Bold.PencilRuler;
    public override string TabName => "Charting";
    public override bool HasLoading => true;

    private LoadingIcon loadingIcon;
    public ChartingContainer Container { get; private set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Container = new ChartingContainer();
        Child = loadingIcon = new LoadingIcon
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Size = new Vector2(50)
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        LoadComponentAsync(Container, container =>
        {
            loadingIcon.FadeOut(200);

            AddInternal(container);
            container.FadeInFromZero(200);
        });
    }

    public override void ScheduleAfterLoad(Action act)
    {
        if (Container.IsLoaded)
            act.Invoke();
        else
            Container.OnLoadComplete += _ => act();
    }

    public static string FormatTypeName<T>(bool multiple = false, bool title = false) where T : ITimedObject
    {
        var type = typeof(T);
        var desc = type.Name.Replace("Event", "").Titleize();
        desc = desc.Replace("Hit Object", "HitObject");

        if (type == typeof(ITimedObject))
            desc = "Object";

        if (!title) desc = desc.ToLower();

        if (multiple)
            desc = desc.Pluralize();

        return desc;
    }
}
