using System;
using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Modes;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Verify.Checks;

public class BasicVerifyContext : IVerifyContext
{
    public GameModeManager Modes { get; }
    public EditorConfiguration Configuration { get; }
    public PlayableMap Map { get; }
    public RealmMap RealmMap { get; }

    private readonly Action<Drawable> loadComponent;

    public BasicVerifyContext(RealmMap map, EditorConfiguration configuration, GameModeManager modes, Action<Drawable> loadComponent)
    {
        RealmMap = map;
        Modes = modes;
        this.loadComponent = loadComponent;
        Configuration = configuration;

        Map = map.GetPlayable(modes) ?? throw new InvalidOperationException($"Could not load map file from {map.FileName}!");
    }

    public void LoadComponent(Drawable drawable) => loadComponent.Invoke(drawable);
}
