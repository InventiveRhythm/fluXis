using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics;
using fluXis.Game.Mods;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Select.Mods;

public partial class ModSelector : Container
{
    public BindableBool IsOpen = new();

    private FillFlowContainer<ModCategory> mods;
    private SpriteText maxScoreText;
    private float scoreMultiplier = 1;

    private List<ModEntry> selected = new();

    private ClickableContainer background;
    private ClickableContainer content;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            background = new ClickableContainer
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
                Action = () => IsOpen.Value = false,
                Child = new Box
                {
                    Colour = Colour4.Black,
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.5f,
                }
            },
            content = new ClickableContainer
            {
                RelativeSizeAxes = Axes.Both,
                RelativePositionAxes = Axes.X,
                X = 0.5f,
                Width = 0.5f,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Padding = new MarginPadding(20),
                Child = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    CornerRadius = 10,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            Colour = FluXisColors.Background2,
                            RelativeSizeAxes = Axes.Both,
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Vertical = 20, Horizontal = 10 },
                            Child = new GridContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                ColumnDimensions = new Dimension[] { new() },
                                RowDimensions = new Dimension[]
                                {
                                    new(GridSizeMode.AutoSize),
                                    new(GridSizeMode.Absolute, 10),
                                    new()
                                },
                                Content = new[]
                                {
                                    new Drawable[]
                                    {
                                        new Container
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Padding = new MarginPadding { Horizontal = 10 },
                                            Children = new Drawable[]
                                            {
                                                new SpriteText
                                                {
                                                    Text = "Gameplay Modifiers",
                                                    Font = FluXisFont.Default(40),
                                                    Y = -10
                                                },
                                                new SpriteText
                                                {
                                                    Text = "Make the game harder or easier for yourself.",
                                                    Font = FluXisFont.Default(),
                                                    Margin = new MarginPadding { Top = 25 },
                                                    Colour = FluXisColors.Text2
                                                },
                                                maxScoreText = new SpriteText
                                                {
                                                    Font = FluXisFont.Default(),
                                                    Anchor = Anchor.CentreRight,
                                                    Origin = Anchor.CentreRight,
                                                    Colour = FluXisColors.Text2
                                                }
                                            }
                                        }
                                    },
                                    new[] { Empty() },
                                    new Drawable[]
                                    {
                                        new BasicScrollContainer
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Masking = true,
                                            ScrollbarVisible = false,
                                            Child = mods = new FillFlowContainer<ModCategory>
                                            {
                                                Direction = FillDirection.Full,
                                                RelativeSizeAxes = Axes.X,
                                                AutoSizeAxes = Axes.Y,
                                                Spacing = new Vector2(0, 10),
                                                Children = new ModCategory[]
                                                {
                                                    new()
                                                    {
                                                        Label = "Difficulty Decrease",
                                                        HexColour = "#b2ff66",
                                                        Selector = this,
                                                        Mods = new IMod[]
                                                        {
                                                            new EasyMod(),
                                                            new NoFailMod()
                                                        }
                                                    },
                                                    new()
                                                    {
                                                        Label = "Difficulty Increase",
                                                        HexColour = "#ff6666",
                                                        Selector = this,
                                                        Mods = new IMod[]
                                                        {
                                                            new HardMod(),
                                                            new FragileMod(),
                                                            new FlawlessMod()
                                                        }
                                                    },
                                                    new()
                                                    {
                                                        Label = "Automation",
                                                        HexColour = "#66b3ff",
                                                        Selector = this,
                                                        Mods = new IMod[]
                                                        {
                                                            new AutoPlayMod(),
                                                        }
                                                    },
                                                    new()
                                                    {
                                                        Label = "Miscellaneous",
                                                        HexColour = "#8866ff",
                                                        Selector = this,
                                                        Mods = new IMod[]
                                                        {
                                                            new NoSvMod(),
                                                            new NoLnMod()
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        IsOpen.BindValueChanged(_ => updateVisibility());
    }

    public void Select(ModEntry mod)
    {
        foreach (var selectedMod in selected)
        {
            if (mod.Mod.IncompatibleMods.Contains(selectedMod.Mod.Acronym) || selectedMod.Mod.IncompatibleMods.Contains(mod.Mod.Acronym))
            {
                selectedMod.Selected = false;
                selectedMod.UpdateSelected();

                Schedule(() => selected.Remove(selectedMod));
            }
        }

        selected.Add(mod);
        updateTotalMultiplier();
    }

    public void Deselect(ModEntry mod)
    {
        selected.Remove(mod);
        updateTotalMultiplier();
    }

    private void updateTotalMultiplier()
    {
        this.TransformTo(nameof(scoreMultiplier), 1f + SelectedMods.Sum(mod => mod.ScoreMultiplier - 1f), 400, Easing.OutQuint);
    }

    public List<IMod> SelectedMods => selected.Select(mod => mod.Mod).ToList();

    private void updateVisibility()
    {
        background.FadeTo(IsOpen.Value ? 1 : 0, 400);
        content.MoveToX(IsOpen.Value ? 0 : .5f, 800, Easing.OutQuint);
    }

    protected override void Update()
    {
        maxScoreText.Text = $"Max Score: {(int)(scoreMultiplier * 100)}%";
    }

    protected override bool OnHover(HoverEvent e) => IsOpen.Value;
    protected override bool OnDragStart(DragStartEvent e) => IsOpen.Value;
    protected override bool OnScroll(ScrollEvent e) => IsOpen.Value;
}
