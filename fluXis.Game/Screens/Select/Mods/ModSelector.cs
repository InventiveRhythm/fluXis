using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Localization;
using fluXis.Game.Mods;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Select.Mods;

public partial class ModSelector : Container
{
    public BindableBool IsOpen = new();

    private FluXisSpriteText maxScoreText;
    private float scoreMultiplier = 1;

    private readonly Dictionary<IMod, ModEntry> mappings = new();

    public List<IMod> SelectedMods { get; } = new();
    public ModSelectRate RateMod { get; private set; }

    private ClickableContainer background;
    private ClickableContainer content;

    private Sample sampleOpen;
    private Sample sampleClose;
    private Sample sampleSelect;
    private Sample sampleDeselect;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        sampleOpen = samples.Get("UI/Select/ModSelect-Open");
        sampleClose = samples.Get("UI/Select/ModSelect-Close");
        sampleSelect = samples.Get("UI/Select/ModSelect-Select");
        sampleDeselect = samples.Get("UI/Select/ModSelect-Deselect");

        RelativeSizeAxes = Axes.Both;
        Padding = new MarginPadding { Top = -10 };
        Alpha = 0;

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
                    Alpha = 0.5f
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
                            RelativeSizeAxes = Axes.Both
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
                                                new FluXisSpriteText
                                                {
                                                    Text = LocalizationStrings.ModSelect.Title,
                                                    FontSize = 40,
                                                    Y = -10
                                                },
                                                new FluXisSpriteText
                                                {
                                                    Text = LocalizationStrings.ModSelect.Description,
                                                    Margin = new MarginPadding { Top = 25 },
                                                    Colour = FluXisColors.Text2
                                                },
                                                maxScoreText = new FluXisSpriteText
                                                {
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
                                        new FluXisScrollContainer
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Masking = true,
                                            ScrollbarAnchor = Anchor.TopRight,
                                            Child = new FillFlowContainer
                                            {
                                                RelativeSizeAxes = Axes.X,
                                                AutoSizeAxes = Axes.Y,
                                                Direction = FillDirection.Vertical,
                                                Children = new Drawable[]
                                                {
                                                    new Container
                                                    {
                                                        RelativeSizeAxes = Axes.X,
                                                        AutoSizeAxes = Axes.Y,
                                                        Padding = new MarginPadding { Horizontal = 5 },
                                                        Margin = new MarginPadding { Bottom = 10 },
                                                        Child = RateMod = new ModSelectRate { Selector = this }
                                                    },
                                                    new FillFlowContainer<FillFlowContainer<ModCategory>>
                                                    {
                                                        Direction = FillDirection.Horizontal,
                                                        RelativeSizeAxes = Axes.X,
                                                        AutoSizeAxes = Axes.Y,
                                                        Children = new[]
                                                        {
                                                            new FillFlowContainer<ModCategory>
                                                            {
                                                                AutoSizeAxes = Axes.Y,
                                                                RelativeSizeAxes = Axes.X,
                                                                Width = 0.5f,
                                                                Direction = FillDirection.Vertical,
                                                                Spacing = new Vector2(0, 10),
                                                                Children = new ModCategory[]
                                                                {
                                                                    new()
                                                                    {
                                                                        Label = LocalizationStrings.ModSelect.DifficultyDecreaseSection,
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
                                                                        Label = LocalizationStrings.ModSelect.MiscSection,
                                                                        HexColour = "#8866ff",
                                                                        Selector = this,
                                                                        Mods = new IMod[]
                                                                        {
                                                                            new NoSvMod(),
                                                                            new NoLnMod()
                                                                        }
                                                                    }
                                                                }
                                                            },
                                                            new FillFlowContainer<ModCategory>
                                                            {
                                                                AutoSizeAxes = Axes.Y,
                                                                RelativeSizeAxes = Axes.X,
                                                                Width = 0.5f,
                                                                Direction = FillDirection.Vertical,
                                                                Spacing = new Vector2(0, 10),
                                                                Children = new ModCategory[]
                                                                {
                                                                    new()
                                                                    {
                                                                        Label = LocalizationStrings.ModSelect.DifficultyIncreaseSection,
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
                                                                        Label = LocalizationStrings.ModSelect.AutomationSection,
                                                                        HexColour = "#66b3ff",
                                                                        Selector = this,
                                                                        Mods = new IMod[]
                                                                        {
                                                                            new AutoPlayMod(),
                                                                            new AutoPlayV2Mod()
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
                        }
                    }
                }
            }
        };

        IsOpen.BindValueChanged(_ => updateVisibility());
    }

    public void AddMapping(IMod mod, ModEntry entry)
    {
        mappings.Add(mod, entry);
    }

    public void Select(IMod mod)
    {
        foreach (var selectedMod in SelectedMods)
        {
            if (mod.IncompatibleMods.Contains(selectedMod.Acronym) || selectedMod.IncompatibleMods.Contains(mod.Acronym))
            {
                var entry = mappings[selectedMod];

                if (entry != null)
                {
                    entry.Selected = false;
                    entry.UpdateSelected();

                    Schedule(() => SelectedMods.Remove(selectedMod));
                }
            }
        }

        sampleSelect?.Play();
        SelectedMods.Add(mod);
        UpdateTotalMultiplier();
    }

    public void Deselect(IMod mod)
    {
        sampleDeselect?.Play();
        SelectedMods.Remove(mod);
        UpdateTotalMultiplier();
    }

    public void UpdateTotalMultiplier() => this.TransformTo(nameof(scoreMultiplier), 1f + SelectedMods.Sum(mod => mod.ScoreMultiplier - 1f), 400, Easing.OutQuint);

    private void updateVisibility()
    {
        content.MoveToX(IsOpen.Value ? 0 : .5f, 800, Easing.OutQuint);
        background.FadeTo(IsOpen.Value ? 1 : 0, 400).OnComplete(_ =>
        {
            if (!IsOpen.Value) Hide();
        });

        if (IsOpen.Value)
        {
            sampleOpen?.Play();
            Show();
        }
        else sampleClose?.Play();
    }

    protected override void Update()
    {
        maxScoreText.Text = LocalizationStrings.ModSelect.MaxScore((int)Math.Round(scoreMultiplier * 100));
    }

    protected override bool OnHover(HoverEvent e) => true;
    protected override bool OnDragStart(DragStartEvent e) => true;
    protected override bool OnScroll(ScrollEvent e) => true;
}
