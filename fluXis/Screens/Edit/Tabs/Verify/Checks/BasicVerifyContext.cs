using System;
using fluXis.Database.Maps;
using fluXis.Map;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Verify.Checks;

public class BasicVerifyContext : IVerifyContext
{
    public MapInfo MapInfo { get; }
    public MapEvents MapEvents { get; }
    public RealmMap RealmMap { get; }

    private readonly Action<Drawable> loadComponent;

    public BasicVerifyContext(RealmMap map, Action<Drawable> loadComponent)
    {
        RealmMap = map;
        this.loadComponent = loadComponent;

        MapInfo = map.GetMapInfo() ?? throw new InvalidOperationException($"Could not load map file from {map.FileName}!");
        MapEvents = MapInfo.GetMapEvents();
    }

    public void LoadComponent(Drawable drawable) => loadComponent.Invoke(drawable);
}
