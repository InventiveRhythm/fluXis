using System;
using System.Collections.Generic;
using fluXis.Game.Screens.Gameplay.HUD.Components;
using fluXis.Game.Screens.Gameplay.HUD.Components.Judgement;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.HUD;

public partial class GameplayHUD : Container
{
    [Resolved]
    private GameplayScreen screen { get; set; }

    [Resolved]
    private LayoutManager layouts { get; set; }

    private readonly Dictionary<string, Type> componentLookup = new();

    private Container components;
    private Container playfieldComponents;

    public GameplayHUD()
    {
        componentLookup.Add("Accuracy", typeof(AccuracyDisplay));
        componentLookup.Add("AttributeText", typeof(AttributeText));
        componentLookup.Add("Combo", typeof(ComboCounter));
        componentLookup.Add("Health", typeof(HealthBar));
        componentLookup.Add("HitError", typeof(HitErrorBar));
        componentLookup.Add("Judgement", typeof(JudgementDisplay));
        componentLookup.Add("JudgementCounter", typeof(JudgementCounter));
        componentLookup.Add("Progress", typeof(Progressbar));
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
            playfieldComponents = new Container
            {
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
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

    protected override void Update()
    {
        base.Update();

        playfieldComponents.Position = screen.Playfield.Position;
        playfieldComponents.Width = screen.Playfield.DrawWidth;

        var scale = screen.Playfield.Scale;

        if (screen.Playfield.IsUpScroll)
            scale *= new Vector2(0, -1);

        playfieldComponents.Scale = scale;
    }

    private void bindableRefreshLayout(ValueChangedEvent<HUDLayout> _)
    {
        refreshLayout();
    }

    private void refreshLayout()
    {
        components.Clear();
        playfieldComponents.Clear();

        loadLayout();
    }

    private void loadLayout()
    {
        if (layouts.Layout.Value?.Gameplay == null)
            return;

        foreach (var (key, value) in layouts.Layout.Value.Gameplay)
        {
            var typeName = key.Split('#')[0];

            if (!componentLookup.ContainsKey(typeName))
                continue;

            try
            {
                var type = componentLookup[typeName];
                var component = (GameplayHUDComponent)Activator.CreateInstance(type);

                if (component == null)
                    throw new Exception($"Failed to create instance of {type}.");

                component.Settings = value;
                LoadComponent(component);
                value.ApplyTo(component);

                if (value.AnchorToPlayfield)
                    playfieldComponents.Add(component);
                else
                    components.Add(component);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to load component {key}.");
            }
        }
    }
}
