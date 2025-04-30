using System;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Graphics.Containers;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Localization;
using fluXis.Map;
using fluXis.Mods;
using fluXis.Scoring.Processing;
using fluXis.Utils;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Screens.Select.Mods;

public partial class ModsOverlay : VisibilityContainer
{
    protected override bool StartHidden => true;

    public const double STAGGER_DURATION = 20;

    [Resolved]
    private MapStore maps { get; set; }

    public float ScoreMultiplier { get; private set; } = 1;

    public BindableList<IMod> SelectedMods { get; } = new();
    public ModSelectRate RateMod { get; private set; }
    public Action<float> RateChanged { get; set; }

    [CanBeNull]
    public BufferedContainer BackgroundBlur { get; set; }

    private float blur
    {
        get => BackgroundBlur?.BlurSigma.X / 8 ?? 0;
        set
        {
            if (BackgroundBlur != null)
            {
                BackgroundBlur.BlurSigma = new Vector2(value * 6);
                BackgroundBlur.FrameBufferScale = new Vector2(1 - value * .5f);
            }
        }
    }

    private FullInputBlockingContainer background;

    private ForcedHeightText maxScoreText;
    private ForcedHeightText maxPerformanceText;
    private float scoreMultiplier = 1;
    private double maxPerformance = 1;

    private Container header;
    private ModCategory diffDecrease;
    private ModCategory diffIncrease;
    private ModCategory miscellaneous;
    private ModCategory automation;

    private Sample sampleOpen;
    private Sample sampleClose;
    private Sample sampleSelect;
    private Sample sampleDeselect;

    private bool first = true;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        sampleOpen = samples.Get("UI/Select/mods-open");
        sampleClose = samples.Get("UI/Select/mods-close");
        sampleSelect = samples.Get("UI/Select/mods-select");
        sampleDeselect = samples.Get("UI/Select/mods-deselect");

        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            background = new FullInputBlockingContainer()
            {
                RelativeSizeAxes = Axes.Both,
                OnClickAction = Hide,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black,
                    Alpha = .75f
                }
            },
            new Container()
            {
                Width = 1200,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Child = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Vertical = 48 },
                    Child = new GridContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        ColumnDimensions = new Dimension[] { new() },
                        RowDimensions = new Dimension[]
                        {
                            new(GridSizeMode.Absolute, 46),
                            new(GridSizeMode.Absolute, 24),
                            new()
                        },
                        Content = new[]
                        {
                            new Drawable[]
                            {
                                header = new Container
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Padding = new MarginPadding { Horizontal = 10 },
                                    AlwaysPresent = true,
                                    Children = new Drawable[]
                                    {
                                        new FillFlowContainer
                                        {
                                            AutoSizeAxes = Axes.Both,
                                            Direction = FillDirection.Vertical,
                                            Spacing = new Vector2(2),
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            Children = new Drawable[]
                                            {
                                                new ForcedHeightText
                                                {
                                                    Height = 23,
                                                    Text = LocalizationStrings.ModSelect.Title,
                                                    WebFontSize = 32,
                                                },
                                                new ForcedHeightText
                                                {
                                                    Height = 15,
                                                    Text = LocalizationStrings.ModSelect.Description,
                                                    WebFontSize = 20,
                                                    Alpha = .8f
                                                }
                                            }
                                        },
                                        new FillFlowContainer
                                        {
                                            AutoSizeAxes = Axes.Both,
                                            Direction = FillDirection.Vertical,
                                            Spacing = new Vector2(2),
                                            Anchor = Anchor.CentreRight,
                                            Origin = Anchor.CentreRight,
                                            Children = new Drawable[]
                                            {
                                                maxScoreText = new ForcedHeightText
                                                {
                                                    Height = 15,
                                                    WebFontSize = 20,
                                                    Anchor = Anchor.CentreRight,
                                                    Origin = Anchor.CentreRight
                                                },
                                                maxPerformanceText = new ForcedHeightText
                                                {
                                                    Height = 10,
                                                    WebFontSize = 14,
                                                    Text = "0pr max",
                                                    Anchor = Anchor.CentreRight,
                                                    Origin = Anchor.CentreRight,
                                                    Alpha = .8f
                                                }
                                            }
                                        },
                                    }
                                }
                            },
                            new[] { Empty() },
                            new Drawable[]
                            {
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Direction = FillDirection.Vertical,
                                    Spacing = new Vector2(24),
                                    Children = new Drawable[]
                                    {
                                        new Container
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Child = RateMod = new ModSelectRate { Selector = this }
                                        },
                                        new GridContainer
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            RowDimensions = new Dimension[] { new(GridSizeMode.AutoSize) },
                                            ColumnDimensions = new Dimension[]
                                            {
                                                new(),
                                                new(GridSizeMode.Absolute, 24),
                                                new()
                                            },
                                            Content = new[]
                                            {
                                                new[]
                                                {
                                                    new FillFlowContainer<ModCategory>
                                                    {
                                                        RelativeSizeAxes = Axes.X,
                                                        AutoSizeAxes = Axes.Y,
                                                        Direction = FillDirection.Vertical,
                                                        Spacing = new Vector2(24),
                                                        Children = new[]
                                                        {
                                                            diffDecrease = new(this, LocalizationStrings.ModSelect.DifficultyDecreaseSection, FluXisColors.GetModTypeColor(ModType.DifficultyDecrease),
                                                                new IMod[]
                                                                {
                                                                    new EasyMod(),
                                                                    new NoFailMod()
                                                                }),
                                                            miscellaneous = new(this, LocalizationStrings.ModSelect.MiscSection, FluXisColors.GetModTypeColor(ModType.Misc), new IMod[]
                                                            {
                                                                new NoSvMod(),
                                                                new NoLnMod(),
                                                                new NoEventMod(),
                                                                new MirrorMod()
                                                            })
                                                        }
                                                    },
                                                    Empty(),
                                                    new FillFlowContainer<ModCategory>
                                                    {
                                                        RelativeSizeAxes = Axes.X,
                                                        AutoSizeAxes = Axes.Y,
                                                        Direction = FillDirection.Vertical,
                                                        Spacing = new Vector2(24),
                                                        Children = new[]
                                                        {
                                                            diffIncrease = new(this, LocalizationStrings.ModSelect.DifficultyIncreaseSection, FluXisColors.GetModTypeColor(ModType.DifficultyIncrease),
                                                                new IMod[]
                                                                {
                                                                    new HardMod(),
                                                                    new FragileMod(),
                                                                    new FlawlessMod()
                                                                }),
                                                            automation = new(this, LocalizationStrings.ModSelect.AutomationSection, FluXisColors.GetModTypeColor(ModType.Automation), new IMod[]
                                                            {
                                                                new AutoPlayMod()
                                                            })
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
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        SelectedMods.BindCollectionChanged((_, _) => UpdateTotalMultiplier(), true);
        maps.MapBindable.BindValueChanged(mapChanged, true);
    }

    protected override void PopIn()
    {
        background.FadeTo(1f, FluXisScreen.FADE_DURATION);

        header.MoveToX(50).MoveToX(0, 400, Easing.OutQuint).FadeIn(200);
        RateMod.Show();
        diffDecrease.Show(STAGGER_DURATION * 2);
        diffIncrease.Show((diffDecrease.ModCount + 3) * STAGGER_DURATION);
        miscellaneous.Show((diffDecrease.ModCount + 3) * STAGGER_DURATION);
        automation.Show((diffDecrease.ModCount + diffIncrease.ModCount + 4) * STAGGER_DURATION);
        this.TransformTo(nameof(blur), 1f, FluXisScreen.MOVE_DURATION, Easing.OutQuint);

        sampleOpen?.Play();
    }

    protected override void PopOut()
    {
        background.FadeTo(0f, FluXisScreen.FADE_DURATION);

        header.MoveToX(-50, 400, Easing.OutQuint).FadeOut(200);
        RateMod.Hide();
        diffDecrease.Hide(STAGGER_DURATION * 2);
        diffIncrease.Hide((diffDecrease.ModCount + 3) * STAGGER_DURATION);
        miscellaneous.Hide((diffDecrease.ModCount + 3) * STAGGER_DURATION);
        automation.Hide((diffDecrease.ModCount + diffIncrease.ModCount + 4) * STAGGER_DURATION);
        this.TransformTo(nameof(blur), 0f, FluXisScreen.MOVE_DURATION, Easing.OutQuint);

        if (!first)
            sampleClose?.Play();

        first = false;
    }

    public void Select(IMod mod)
    {
        foreach (var selectedMod in SelectedMods.Where(selectedMod => ModUtils.IsIncompatible(selectedMod, mod) || ModUtils.IsIncompatible(mod, selectedMod)))
            Schedule(() => SelectedMods.Remove(selectedMod));

        sampleSelect?.Play();
        SelectedMods.Add(mod);
    }

    public void Deselect(IMod mod)
    {
        sampleDeselect?.Play();
        SelectedMods.RemoveAll(m => m.GetType() == mod.GetType());
    }

    public void DeselectAll()
    {
        sampleDeselect?.Play();
        SelectedMods.Clear();
        RateMod.RateBindable.Value = 1f;
    }

    public void UpdateTotalMultiplier()
    {
        var multiplier = 1f + SelectedMods.Sum(mod => mod.ScoreMultiplier - 1f);

        ScoreMultiplier = multiplier;
        this.TransformTo(nameof(scoreMultiplier), multiplier, 400, Easing.OutQuint);

        var map = maps.CurrentMap;
        var combo = map.Filters.NoteCount + (map.Filters.LongNoteCount * 2);
        var pr = ScoreProcessor.CalculatePerformance(map.Filters.NotesPerSecond, 100, combo, 0, 0, 0, 0, 0, SelectedMods.ToList());
        this.TransformTo(nameof(maxPerformance), pr, 400, Easing.OutQuint);
    }

    private void mapChanged(ValueChangedEvent<RealmMap> _) => UpdateTotalMultiplier();

    protected override void Update()
    {
        maxScoreText.Text = LocalizationStrings.ModSelect.MaxScore((int)Math.Round(scoreMultiplier * 100));
        maxPerformanceText.Text = $"{maxPerformance:0}pr max";
    }

    protected override bool OnHover(HoverEvent e) => true;
    protected override bool OnDragStart(DragStartEvent e) => true;
    protected override bool OnScroll(ScrollEvent e) => true;

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        maps.MapBindable.ValueChanged -= mapChanged;
    }
}
