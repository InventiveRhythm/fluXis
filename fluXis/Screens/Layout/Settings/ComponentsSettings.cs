using System.Collections.Generic;
using System.Collections.Specialized;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Text;
using fluXis.Screens.Edit.UI.Variable;
using fluXis.Screens.Gameplay.HUD;
using fluXis.Screens.Layout.Blueprints;
using fluXis.Utils;
using fluXis.Utils.Attributes;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Layout.Settings;

public partial class ComponentsSettings : FluXisScrollContainer
{
    [Resolved]
    private LayoutEditor editor { get; set; }

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
                    new EditorVariableTextBox
                    {
                        Text = "X Position",
                        CurrentValue = comp.Settings.Position.X.ToStringInvariant(),
                        OnValueChanged = box =>
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
                    new EditorVariableTextBox
                    {
                        Text = "Y Position",
                        CurrentValue = comp.Settings.Position.Y.ToStringInvariant(),
                        OnValueChanged = box =>
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
                    new EditorVariableDropdown<Anchor>
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
                    new EditorVariableDropdown<Anchor>
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
                    new EditorVariableSlider<float>()
                    {
                        Text = "Scale",
                        CurrentValue = comp.Settings.Scale,
                        Min = .5f,
                        Max = 4f,
                        Step = 0.02f,
                        OnValueChanged = v =>
                        {
                            comp.Settings.Scale = v;
                            update(comp);
                        }
                    },
                    new EditorVariableToggle
                    {
                        Text = "Anchor to Playfield",
                        CurrentValue = comp.Settings.AnchorToPlayfield,
                        OnValueChanged = v =>
                        {
                            comp.Settings.AnchorToPlayfield = v;
                            editor.UpdateAnchorToPlayfield(comp);
                            selectionChanged(null, null);
                        }
                    }
                });

                flow.AddRange(comp.CreatePointSettings());
                flow.FinishTransforms(true);
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
