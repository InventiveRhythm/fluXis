using System;
using System.Linq;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osuTK;

namespace fluXis.Screens.Gameplay.HUD;

public partial class GameplayHUD : Container
{
    private RulesetContainer ruleset { get; }

    [Resolved]
    private LayoutManager layouts { get; set; }

    public GameplayHUDComponent[] Components => components.Concat(playfields[0].Children).ToArray();

    public bool AutoRefresh { get; init; } = true;

    private HUDLayout layout;

    private Container<GameplayHUDComponent> components;
    private PlayfieldHUD[] playfields;

    public GameplayHUD(RulesetContainer ruleset, HUDLayout layout = null)
    {
        this.ruleset = ruleset;
        this.layout = layout;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            components = new Container<GameplayHUDComponent>
            {
                RelativeSizeAxes = Axes.Both
            },
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                Content = new[]
                {
                    playfields = ruleset.PlayfieldManager.Players.Select(x => new PlayfieldHUD(x)).ToArray()
                }
            }
        };

        layout ??= layouts.Layout.Value;
        loadLayout();
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (!AutoRefresh) return;

        layouts.Reloaded += refreshLayout;
        layouts.Layout.ValueChanged += bindableRefreshLayout;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        layouts.Reloaded -= refreshLayout;
        layouts.Layout.ValueChanged -= bindableRefreshLayout;
    }

    private void bindableRefreshLayout(ValueChangedEvent<HUDLayout> _) => refreshLayout();

    private void refreshLayout()
    {
        components.Clear();
        playfields.ForEach(hud => hud.Clear());

        layout = layouts.Layout.Value;
        loadLayout();
    }

    private void loadLayout()
    {
        if (layout?.Gameplay == null)
            return;

        foreach (var (key, settings) in layout.Gameplay)
        {
            try
            {
                AddComponent(key, settings);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to load component {key}.");
            }
        }
    }

    public GameplayHUDComponent AddComponent(string key, HUDComponentSettings settings)
    {
        var loop = settings.AnchorToPlayfield ? playfields.Length : 1;
        GameplayHUDComponent component = null;

        for (int i = 0; i < loop; i++)
        {
            var manager = playfields[i].Player;
            var split = key.Split('#');
            component = layouts.CreateComponent(split[0], settings, manager);
            component.Name = key;

            if (split.Length > 1)
            {
                var subKey = split[1];
                settings.Key = subKey;
            }

            if (settings.AnchorToPlayfield)
                playfields[i].Add(component);
            else
                components.Add(component);
        }

        settings.Drawable = component;
        return component;
    }

    public void RemoveComponent(GameplayHUDComponent component)
    {
        var key = component.Name;
        components.RemoveAll(c => c.Name == key, true);
        playfields.ForEach(hud => hud.RemoveAll(c => c.Name == key, true));
    }

    private partial class PlayfieldHUD : Container<GameplayHUDComponent>
    {
        public PlayfieldPlayer Player { get; }
        public Playfield Playfield => Player.MainPlayfield;

        public PlayfieldHUD(PlayfieldPlayer player)
        {
            Player = player;

            AlwaysPresent = true;
            RelativeSizeAxes = Axes.Y;
            Anchor = Origin = Anchor.Centre;
        }

        protected override void Update()
        {
            base.Update();

            Position = Playfield.Position;
            Width = Playfield.DrawWidth;
            Rotation = Playfield.Rotation;
            Alpha = Playfield.Alpha;

            var scale = Playfield.Scale;

            if (Playfield.IsUpScroll)
                scale *= new Vector2(1, -1);

            Scale = scale;
        }
    }
}
