using System.Collections.Generic;
using System.Collections.Specialized;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Screens.Gameplay.HUD;
using fluXis.Screens.Layout.Blueprints;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Layout.Settings;

public partial class ComponentsSettings : FluXisScrollContainer
{
    private static List<Anchor> anchors => new()
    {
        Anchor.TopLeft, Anchor.TopCentre, Anchor.TopRight,
        Anchor.CentreLeft, Anchor.Centre, Anchor.CentreRight,
        Anchor.BottomLeft, Anchor.BottomCentre, Anchor.BottomRight
    };

    private LayoutBlueprintContainer blueprints { get; }
    private BindableList<GameplayHUDComponent> selected => blueprints.SelectionHandler.SelectedObjects;

    private FillFlowContainer flow;

    public ComponentsSettings(LayoutBlueprintContainer blueprints)
    {
        this.blueprints = blueprints;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = flow = new FillFlowContainer()
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Padding = new MarginPadding(20),
            Spacing = new Vector2(16)
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        selected.BindCollectionChanged(selectionChanged, true);
    }

    private void selectionChanged(object _, NotifyCollectionChangedEventArgs __)
    {
        flow.Clear();

        switch (selected.Count)
        {
            case 1:
                var comp = selected[0];

                flow.AddRange(new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Text = comp.GetType().Name,
                        WebFontSize = 20
                    },
                    new PointSettingsTextBox
                    {
                        Text = "X Position",
                        DefaultText = comp.Settings.Position.X.ToStringInvariant(),
                        OnTextChanged = box =>
                        {
                            if (box.Text.TryParseFloatInvariant(out var v))
                            {
                                comp.Settings.Position = new Vector2(v, comp.Settings.Position.Y);
                                update(comp);
                            }
                            else
                                box.NotifyError();
                        }
                    },
                    new PointSettingsTextBox
                    {
                        Text = "Y Position",
                        DefaultText = comp.Settings.Position.Y.ToStringInvariant(),
                        OnTextChanged = box =>
                        {
                            if (box.Text.TryParseFloatInvariant(out var v))
                            {
                                comp.Settings.Position = new Vector2(comp.Settings.Position.X, v);
                                update(comp);
                            }
                            else
                                box.NotifyError();
                        }
                    },
                    new PointSettingsDropdown<Anchor>
                    {
                        Text = "Anchor",
                        CurrentValue = comp.Settings.Anchor,
                        Items = anchors,
                        OnValueChanged = v =>
                        {
                            comp.Settings.Anchor = v;
                            update(comp);
                        }
                    },
                    new PointSettingsDropdown<Anchor>
                    {
                        Text = "Origin",
                        CurrentValue = comp.Settings.Origin,
                        Items = anchors,
                        OnValueChanged = v =>
                        {
                            comp.Settings.Origin = v;
                            update(comp);
                        }
                    },
                    new PointSettingsSlider<float>()
                    {
                        Text = "Scale",
                        CurrentValue = comp.Settings.Scale,
                        Min = .5f,
                        Max = 4f,
                        OnValueChanged = v =>
                        {
                            comp.Settings.Scale = v;
                            update(comp);
                        }
                    },
                    new PointSettingsToggle
                    {
                        Text = "Anchor to Playfield",
                        CurrentValue = comp.Settings.AnchorToPlayfield,
                        OnStateChanged = v =>
                        {
                            comp.Settings.AnchorToPlayfield = v;
                            update(comp);
                        }
                    }
                });
                return;

            case > 1:
                flow.Add(new FluXisSpriteText
                {
                    Text = "Too many selections.",
                    WebFontSize = 14
                });
                return;

            default:
                flow.Add(new FluXisSpriteText
                {
                    Text = "Nothing selected.",
                    WebFontSize = 14
                });
                return;
        }

        void update(GameplayHUDComponent comp) => comp.Settings.ApplyTo(comp);
    }
}
