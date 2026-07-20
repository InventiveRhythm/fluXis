using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Audio;
using fluXis.Configuration;
using fluXis.Database.Maps;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Localization;
using fluXis.Map;
using fluXis.Map.Drawables;
using fluXis.Mods;
using fluXis.Overlay.Mouse;
using fluXis.Scoring;
using fluXis.Screens.Select.Mods;
using fluXis.Skinning;
using fluXis.Utils;
using JetBrains.Annotations;
using Midori.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Select.Info.Header;

public partial class SelectMapInfoHeader : CompositeDrawable
{
    [Resolved]
    private MapStore maps { get; set; }

    [Resolved]
    private SkinManager skins { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private ModsOverlay mods { get; set; }

    private SpriteStack<LoadWrapper<MapBackground>> backgrounds;
    private SectionedGradient gradient;
    private SpriteStack<LoadWrapper<MapCover>> covers;

    private Marquee title;
    private float titleSize;
    private Marquee artist;
    private float artistSize;

    private DifficultyChip difficulty;
    private FluXisSpriteText difficultyText;
    private FluXisSpriteText mapper;

    private BpmDisplay bpm;
    private StatDisplay length;
    private StatDisplay notesPerSecond;
    private TooltipStatDisplay longNotePercentage;

    private TooltipStatDisplay accuracy;
    private StatDisplay health;

    private Container headerTop;
    private FillFlowContainer topFlow;
    private Container coversContainer;
    private Bindable<float> sizeBind;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        sizeBind = config.GetBindable<float>(FluXisSetting.InfoHeaderSize);

        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        CornerRadius = 20;
        Masking = true;
        EdgeEffect = Styling.ShadowMedium;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background2
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding { Bottom = 10 },
                Children = new Drawable[]
                {
                    headerTop = new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = sizeBind.Value,
                        CornerRadius = 20,
                        Masking = true,
                        Children = new Drawable[]
                        {
                            backgrounds = new SpriteStack<LoadWrapper<MapBackground>> { AutoFill = false },
                            new SectionedGradient
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Colour4.Black,
                                Alpha = .5f
                            },
                            gradient = new SectionedGradient
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Colour4.Black,
                                Alpha = .5f
                            },
                            new GridContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                ColumnDimensions = new Dimension[]
                                {
                                    new(GridSizeMode.AutoSize),
                                    new()
                                },
                                Content = new[]
                                {
                                    new Drawable[]
                                    {
                                        coversContainer = new Container
                                        {
                                            Size = new Vector2(sizeBind.Value),
                                            CornerRadius = 20,
                                            Masking = true,
                                            Child = covers = new SpriteStack<LoadWrapper<MapCover>>()
                                        },
                                        topFlow = new FillFlowContainer
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Direction = FillDirection.Vertical,
                                            Spacing = new Vector2(-5),
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            Padding = new MarginPadding { Horizontal = 20 },
                                            Children = new Drawable[]
                                            {
                                                title = new Marquee { RelativeSizeAxes = Axes.X },
                                                artist = new Marquee { RelativeSizeAxes = Axes.X }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 44,
                        Padding = new MarginPadding { Horizontal = 20 },
                        Spacing = new Vector2(5),
                        Direction = FillDirection.Horizontal,
                        Children = new Drawable[]
                        {
                            difficulty = new DifficultyChip
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Size = new Vector2(80, 20),
                                Margin = new MarginPadding { Right = 5 }
                            },
                            difficultyText = new FluXisSpriteText
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                WebFontSize = 16,
                                Text = "difficulty"
                            },
                            mapper = new FluXisSpriteText
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                WebFontSize = 16,
                                Alpha = .8f,
                                Text = "mapped by nobody"
                            }
                        }
                    },
                    new StatsRow(new[]
                    {
                        bpm = new BpmDisplay(),
                        length = new StatDisplay(LocalizationStrings.General.MapLength, v => TimeUtils.Format(v, false)),
                        notesPerSecond = new StatDisplay(LocalizationStrings.General.MapNotesPerSecond, v => v.ToStringInvariant("0.00")),
                        longNotePercentage = new TooltipStatDisplay(LocalizationStrings.General.MapLongNotePercent, v => v.ToStringInvariant("0.00") + "%")
                    }),
                    new StatsRow(new[]
                    {
                        accuracy = new TooltipStatDisplay(LocalizationStrings.General.MapAccuracy, v => v.ToStringInvariant("0.0")),
                        health = new StatDisplay(LocalizationStrings.General.MapHealth, v => v.ToStringInvariant("0.0"))
                    })
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        maps.MapBindable.BindValueChanged(mapChanged, true);
        sizeBind.BindValueChanged(v =>
        {
            const float dur = 400;
            const Easing ease = Easing.OutQuint;

            switch (v.NewValue)
            {
                case <= 132:
                    titleSize = 24;
                    artistSize = 16;
                    topFlow.Padding = new MarginPadding { Left = 12, Right = 32 };
                    break;

                default:
                    titleSize = 36;
                    artistSize = 24;
                    topFlow.Padding = new MarginPadding { Left = 20, Right = 40 };
                    break;
            }

            headerTop.ResizeHeightTo(v.NewValue, dur, ease);
            coversContainer.ResizeTo(new Vector2(v.NewValue), dur, ease);

            if (maps.CurrentMap is null) return;

            updateText(title, maps.CurrentMap.Metadata.LocalizedTitle, titleSize);
            updateText(artist, maps.CurrentMap.Metadata.LocalizedArtist, artistSize);
        }, true);

        if (mods is null) return;

        mods.SelectedMods.BindCollectionChanged((_, _) => updateDifficultyValues(maps.MapBindable.Value, mods.SelectedMods));
        mods.RateMod.RateBindable.BindValueChanged(rateChanged, true);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        maps.MapBindable.ValueChanged -= mapChanged;

        if (mods is null) return;

        mods.RateMod.RateBindable.ValueChanged -= rateChanged;
    }

    private void updateText(Marquee target, LocalisableString text, float size) => target.CreateFunc = () => new FluXisSpriteText
    {
        Text = text,
        WebFontSize = size,
        Shadow = true
    };

    private void rateChanged(ValueChangedEvent<float> e)
    {
        if (maps.MapBindable.Value == null)
            return;

        updateValues(maps.MapBindable.Value, e.NewValue);
    }

    private void mapChanged(ValueChangedEvent<RealmMap> e)
    {
        if (e.NewValue == null)
        {
            updateText(title, "no map selected", titleSize);
            updateText(artist, "select a map to view info", artistSize);
            return;
        }

        var map = e.NewValue;

        updateText(title, map.Metadata.LocalizedTitle, titleSize);
        updateText(artist, map.Metadata.LocalizedArtist, artistSize);
        gradient.FadeColour(map.Metadata.Color, 400, Easing.OutQuint);
        difficulty.RealmMap = map;
        difficultyText.Text = map.Difficulty;
        mapper.Text = $"mapped by {map.Metadata.Mapper}";

        updateValues(map, mods?.RateMod.RateBindable.Value ?? 1);

        longNotePercentage.SetValue(map.Filters.LongNotePercentage * 100);
        longNotePercentage.TooltipContent =
        [
            ("Normal/Tick", $"{map.Filters.NoteCount}", Theme.Text),
            ("Long Notes", $"{map.Filters.LongNoteCount}", Theme.Text),
            ("Mines", $"{map.Filters.LandmineCount}", Theme.Text)
        ];

        updateDifficultyValues(map, mods?.SelectedMods ?? new BindableList<IMod>());

        var background = new LoadWrapper<MapBackground>
        {
            RelativeSizeAxes = Axes.Both,
            LoadContent = () => new MapBackground(map)
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            OnComplete = b => b.FadeInFromZero(Styling.TRANSITION_FADE)
        };
        LoadComponent(background);
        backgrounds.Add(background);

        var cover = new LoadWrapper<MapCover>
        {
            RelativeSizeAxes = Axes.Both,
            LoadContent = () => new MapCover(map.MapSet)
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            OnComplete = c => c.FadeInFromZero(Styling.TRANSITION_FADE)
        };
        LoadComponent(cover);
        covers.Add(cover);
    }

    private void updateDifficultyValues(RealmMap map, IList<IMod> list)
    {
        var multiplier = list.Any(m => m is HardMod) ? 1.5f : 1;

        var acc = Math.Clamp(map.AccuracyDifficulty, 1, 10) * multiplier;
        var hp = Math.Clamp(map.HealthDifficulty == 0 ? 8 : map.HealthDifficulty, 1, 10) * multiplier;

        accuracy.SetValue(acc);
        health.SetValue(hp);

        var windows = new HitWindows(acc, 1);
        accuracy.TooltipContent = windows.GetTimings()
                                         .Select(x => (
                                             x.Judgement.ToString(),
                                             $"{x.Milliseconds:0.##}ms",
                                             skins.SkinJson.GetColorForJudgement(x.Judgement))
                                         )
                                         .ToArray();
    }

    private void updateValues(RealmMap map, float rate)
    {
        bpm.SetValue(map.Filters.BPMMin * rate, map.Filters.BPMMax * rate);
        length.SetValue(map.Filters.Length / rate);
        notesPerSecond.SetValue(map.Filters.NotesPerSecond * rate);
    }

    private partial class StatsRow : GridContainer
    {
        public StatsRow(IEnumerable<StatDisplay> displays)
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            ColumnDimensions = new Dimension[] { new(), new(), new(), new() };
            RowDimensions = new Dimension[] { new(GridSizeMode.AutoSize) };

            Content = new[] { displays.ToArray() };
        }
    }

    private partial class BpmDisplay : StatDisplay
    {
        [Resolved]
        private IBeatSyncProvider beatSync { get; set; }

        private const float min_alpha = .6f;

        private float min = 0;
        private float max = 0;

        private Sample metronomeSample;
        private Sample metronomeEndSample;

        public BpmDisplay()
            : base(LocalizationStrings.General.MapBPM, _ => "")
        {
        }

        [BackgroundDependencyLoader]
        private void load(ISampleStore samples)
        {
            metronomeSample = samples.Get("UI/metronome");
            metronomeEndSample = samples.Get("UI/metronome-end");
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            ValueText.Alpha = min_alpha;
            beatSync.OnBeat += onBeat;
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            beatSync.OnBeat -= onBeat;
        }

        private void onBeat(int beat, bool finish)
        {
            ValueText.FadeIn().FadeTo(min_alpha, beatSync.BeatTime);

            if (!IsHovered)
                return;

            if (finish)
                metronomeEndSample?.Play();
            else
                metronomeSample?.Play();
        }

        public void SetValue(float mi, float ma)
        {
            if (!float.IsFinite(mi))
                mi = 0;
            if (!float.IsFinite(ma))
                ma = 0;

            this.TransformTo(nameof(min), mi, 400, Easing.OutQuint);
            this.TransformTo(nameof(max), ma, 400, Easing.OutQuint);
        }

        protected override void Update()
        {
            ValueText.Text = formatBpm();
        }

        private string formatBpm()
        {
            if (Math.Abs(min - max) < 1)
                return $"{(int)min}";

            return $"{(int)min}-{(int)max}";
        }
    }

    private partial class TooltipStatDisplay : StatDisplay, IHasCustomTooltip<(string l, string r, Colour4 c)[]>
    {
        public (string l, string r, Colour4 c)[] TooltipContent { get; set; } = [];

        public TooltipStatDisplay(LocalisableString title, Func<float, string> format)
            : base(title, format)
        {
        }

        public ITooltip<(string l, string r, Colour4 c)[]> GetCustomTooltip() => new Tooltip();

        private partial class Tooltip : CustomTooltipContainer<(string l, string r, Colour4 c)[]>
        {
            private readonly FillFlowContainer flow;

            public Tooltip()
            {
                Child = flow = new FillFlowContainer
                {
                    Width = 240,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(6),
                    Padding = new MarginPadding(12)
                };
            }

            public override void SetContent((string l, string r, Colour4 c)[] content)
            {
                flow.Clear();
                flow.AddRange(content.Select(x => new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 20,
                    Colour = x.c,
                    Children =
                    [
                        new FluXisSpriteText
                        {
                            Text = x.l,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            WebFontSize = 14
                        },
                        new FluXisSpriteText
                        {
                            Text = x.r,
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            WebFontSize = 14
                        }
                    ]
                }));
            }
        }
    }

    private partial class StatDisplay : CompositeDrawable
    {
        private LocalisableString title { get; }
        private Func<float, string> format { get; }

        private float value = 0;
        protected FluXisSpriteText ValueText { get; private set; }

        public StatDisplay(LocalisableString title, Func<float, string> format)
        {
            this.title = title;
            this.format = format;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            Height = 60;

            InternalChildren = new Drawable[]
            {
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Direction = FillDirection.Vertical,
                    Children = new Drawable[]
                    {
                        new FluXisSpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            WebFontSize = 14,
                            Alpha = .8f,
                            Text = title
                        },
                        ValueText = new FluXisSpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            WebFontSize = 20
                        }
                    }
                }
            };
        }

        public void SetValue(float v)
        {
            if (!float.IsFinite(v))
                v = 0;

            this.TransformTo(nameof(value), v, 400, Easing.OutQuint);
        }

        protected override void Update()
        {
            base.Update();

            ValueText.Text = format(value);
        }
    }
}
