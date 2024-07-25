using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map;
using fluXis.Game.Map.Drawables;
using fluXis.Game.Scoring;
using fluXis.Game.Screens.Select.Mods;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Game.Screens.Select.Info.Header;

public partial class SelectMapInfoHeader : CompositeDrawable
{
    [Resolved]
    private MapStore maps { get; set; }

    [Resolved]
    private ModSelector mods { get; set; }

    private SpriteStack<MapBackground> backgrounds;
    private SectionedGradient gradient;
    private SpriteStack<MapCover> covers;
    private TruncatingText title;
    private TruncatingText artist;
    private DifficultyChip difficulty;
    private FluXisSpriteText difficultyText;
    private FluXisSpriteText mapper;

    private BpmDisplay bpm;
    private StatDisplay length;
    private StatDisplay notesPerSecond;
    private StatDisplay longNotePercentage;

    private StatDisplay accuracy;
    private StatDisplay health;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        CornerRadius = 20;
        Masking = true;
        EdgeEffect = FluXisStyles.ShadowMedium;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding { Bottom = 10 },
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 180,
                        CornerRadius = 20,
                        Masking = true,
                        Children = new Drawable[]
                        {
                            backgrounds = new SpriteStack<MapBackground>(),
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
                                        new Container
                                        {
                                            Size = new Vector2(180),
                                            CornerRadius = 20,
                                            Masking = true,
                                            Child = covers = new SpriteStack<MapCover>()
                                        },
                                        new FillFlowContainer
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
                                                title = new TruncatingText
                                                {
                                                    RelativeSizeAxes = Axes.X,
                                                    WebFontSize = 36,
                                                    Text = "no map selected",
                                                    Shadow = true
                                                },
                                                artist = new TruncatingText
                                                {
                                                    RelativeSizeAxes = Axes.X,
                                                    WebFontSize = 24,
                                                    Text = "select a map to view info",
                                                    Shadow = true
                                                }
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
                        length = new StatDisplay("Length", v => TimeUtils.Format(v, false)),
                        notesPerSecond = new StatDisplay("NPS", v => v.ToStringInvariant("0.00")),
                        longNotePercentage = new StatDisplay("LN%", v => v.ToStringInvariant("0.00") + "%")
                    }),
                    new StatsRow(new[]
                    {
                        accuracy = new StatDisplay("Accuracy", v => v.ToStringInvariant("0.0")),
                        health = new StatDisplay("Health", v => v.ToStringInvariant("0.0"))
                    })
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        maps.MapBindable.BindValueChanged(mapChanged, true);
        mods.RateMod.RateBindable.BindValueChanged(rateChanged, true);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        maps.MapBindable.ValueChanged -= mapChanged;
        mods.RateMod.RateBindable.ValueChanged -= rateChanged;
    }

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
            title.Text = "no map selected";
            artist.Text = "select a map to view info";
            return;
        }

        var map = e.NewValue;

        title.Text = map.Metadata.Title;
        artist.Text = map.Metadata.Artist;
        gradient.FadeColour(map.Metadata.Color, 400, Easing.OutQuint);
        difficulty.Rating = map.Filters.NotesPerSecond;
        difficultyText.Text = map.Difficulty;
        mapper.Text = $"mapped by {map.Metadata.Mapper}";

        updateValues(map, mods.RateMod.RateBindable.Value);

        longNotePercentage.SetValue(map.Filters.LongNotePercentage * 100);
        longNotePercentage.TooltipText = $"{map.Filters.NoteCount} / {map.Filters.LongNoteCount}";

        accuracy.SetValue(map.AccuracyDifficulty);

        // fallback for older maps
        health.SetValue(map.HealthDifficulty == 0 ? 8 : map.HealthDifficulty);

        var windows = new HitWindows(map.AccuracyDifficulty, 1);
        var timingsStr = "";

        foreach (var timing in windows.GetTimings())
            timingsStr += $"{timing.Judgement}: {timing.Milliseconds.ToStringInvariant()}ms\n";

        accuracy.TooltipText = timingsStr.Trim();

        var background = new MapBackground(map);
        backgrounds.Add(background, 400);
        background.FadeInFromZero(400, Easing.OutQuint);

        var cover = new MapCover(map.MapSet);
        covers.Add(cover, 400);
        cover.FadeInFromZero(400, Easing.OutQuint);
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
            : base("BPM", _ => "")
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

        private void onBeat(int beat)
        {
            ValueText.FadeIn().FadeTo(min_alpha, beatSync.BeatTime);

            if (!IsHovered)
                return;

            if (beat % 4 == 0)
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

    private partial class StatDisplay : CompositeDrawable, IHasTooltip
    {
        public LocalisableString TooltipText { get; set; } = "";

        private string title { get; }
        private Func<float, string> format { get; }

        private float value = 0;
        protected FluXisSpriteText ValueText { get; private set; }

        public StatDisplay(string title, Func<float, string> format)
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
