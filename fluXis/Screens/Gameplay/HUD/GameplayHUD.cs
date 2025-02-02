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

    private Container components;
    private PlayfieldHUD[] playfields;

    public GameplayHUD(RulesetContainer ruleset)
    {
        this.ruleset = ruleset;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            components = new Container
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

        loadLayout();
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        layouts.Reloaded += refreshLayout;
        layouts.Layout.ValueChanged += bindableRefreshLayout;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        layouts.Reloaded -= refreshLayout;
        layouts.Layout.ValueChanged -= bindableRefreshLayout;
    }

    private void bindableRefreshLayout(ValueChangedEvent<HUDLayout> _)
    {
        refreshLayout();
    }

    private void refreshLayout()
    {
        components.Clear();
        playfields.ForEach(hud => hud.Clear());

        loadLayout();
    }

    private void loadLayout()
    {
        if (layouts.Layout.Value?.Gameplay == null)
            return;

        foreach (var (key, settings) in layouts.Layout.Value.Gameplay)
        {
            try
            {
                var loop = settings.AnchorToPlayfield ? playfields.Length : 1;

                for (int i = 0; i < loop; i++)
                {
                    var manager = playfields[i].Player;
                    var component = layouts.CreateComponent(key.Split('#')[0], settings, manager.JudgementProcessor, manager.HealthProcessor, manager.ScoreProcessor, ruleset.HitWindows);

                    if (settings.AnchorToPlayfield)
                        playfields[i].Add(component);
                    else
                        components.Add(component);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to load component {key}.");
            }
        }
    }

    private partial class PlayfieldHUD : Container
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
