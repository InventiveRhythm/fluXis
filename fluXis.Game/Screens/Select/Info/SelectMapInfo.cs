using System;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Screens.Select.Info.Scores;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Select.Info;

public partial class SelectMapInfo : GridContainer
{
    [Resolved]
    private AudioClock clock { get; set; }

    public SelectScreen Screen { get; set; }

    public ScoreList ScoreList;

    private BackgroundStack backgroundStack;
    private FluXisSpriteText titleText;
    private FluXisSpriteText artistText;
    private FluXisSpriteText bpmText;
    private FluXisSpriteText lengthText;
    private FluXisSpriteText npsText;
    private FluXisSpriteText lnpText;

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.CentreRight;
        Origin = Anchor.CentreRight;
        RelativeSizeAxes = Axes.Both;
        Width = .5f;
        Padding = new MarginPadding { Vertical = 10 };
        Margin = new MarginPadding { Right = -10 };
        RowDimensions = new Dimension[]
        {
            new(GridSizeMode.Relative, .4f),
            new(GridSizeMode.Absolute, 10),
            new()
        };

        Content = new[]
        {
            new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    CornerRadius = 10,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FluXisColors.Background2
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Bottom = 80 },
                            Child = new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                CornerRadius = 10,
                                Masking = true,
                                Children = new Drawable[]
                                {
                                    backgroundStack = new BackgroundStack(),
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = Colour4.Black,
                                        Alpha = .4f
                                    },
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Padding = new MarginPadding(20),
                                        Children = new Drawable[]
                                        {
                                            titleText = new FluXisSpriteText
                                            {
                                                Text = "No Map Selected",
                                                FontSize = 60,
                                                RelativeSizeAxes = Axes.X,
                                                Truncate = true,
                                                Anchor = Anchor.CentreLeft,
                                                Origin = Anchor.BottomLeft,
                                                Y = 10
                                            },
                                            artistText = new FluXisSpriteText
                                            {
                                                Text = "Please select a map to view info",
                                                FontSize = 32,
                                                RelativeSizeAxes = Axes.X,
                                                Truncate = true,
                                                Anchor = Anchor.CentreLeft,
                                                Origin = Anchor.TopLeft
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            Height = 80,
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                            Padding = new MarginPadding { Right = 10 },
                            Child = new GridContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                ColumnDimensions = new Dimension[]
                                {
                                    new(),
                                    new(),
                                    new(),
                                    new()
                                },
                                Content = new[]
                                {
                                    new Drawable[]
                                    {
                                        new Container
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Children = new Drawable[]
                                            {
                                                new FluXisSpriteText
                                                {
                                                    Text = "BPM",
                                                    FontSize = 18,
                                                    Colour = FluXisColors.Text2,
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.BottomCentre
                                                },
                                                bpmText = new FluXisSpriteText
                                                {
                                                    Text = "",
                                                    FontSize = 24,
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.TopCentre,
                                                    Y = -4
                                                }
                                            }
                                        },
                                        new Container
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Children = new Drawable[]
                                            {
                                                new FluXisSpriteText
                                                {
                                                    Text = "Length",
                                                    FontSize = 18,
                                                    Colour = FluXisColors.Text2,
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.BottomCentre
                                                },
                                                lengthText = new FluXisSpriteText
                                                {
                                                    Text = "",
                                                    FontSize = 24,
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.TopCentre,
                                                    Y = -4
                                                }
                                            }
                                        },
                                        new Container
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Children = new Drawable[]
                                            {
                                                new FluXisSpriteText
                                                {
                                                    Text = "NPS",
                                                    FontSize = 18,
                                                    Colour = FluXisColors.Text2,
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.BottomCentre
                                                },
                                                npsText = new FluXisSpriteText
                                                {
                                                    Text = "",
                                                    FontSize = 24,
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.TopCentre,
                                                    Y = -4
                                                }
                                            }
                                        },
                                        new Container
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Children = new Drawable[]
                                            {
                                                new FluXisSpriteText
                                                {
                                                    Text = "LN%",
                                                    FontSize = 18,
                                                    Colour = FluXisColors.Text2,
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.BottomCentre
                                                },
                                                lnpText = new FluXisSpriteText
                                                {
                                                    Text = "",
                                                    FontSize = 24,
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.TopCentre,
                                                    Y = -4
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            Array.Empty<Drawable>(), // Spacer
            new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Bottom = 20, Right = 20 },
                    Child = ScoreList = new ScoreList
                    {
                        MapInfo = this
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        clock.OnBeat += OnBeat;

        base.LoadComplete();
    }

    public void ChangeMap(RealmMap map)
    {
        backgroundStack.ChangeMap(map);
        ScoreList.SetMap(map);
        titleText.Text = map.Metadata.Title;
        artistText.Text = map.Metadata.Artist;
        // difficultyText.Text = map.Difficulty;
        // mapperText.Text = map.Metadata.Mapper;

        lengthText.Text = TimeUtils.Format(map.Filters.Length, false);
        bpmText.Text = map.Filters.BPMMin == map.Filters.BPMMax ? $"{map.Filters.BPMMin}" : $"{map.Filters.BPMMin}-{map.Filters.BPMMax}";
        npsText.Text = $"{map.Filters.NotesPerSecond:F}".Replace(",", ".");
        lnpText.Text = $"{map.Filters.LongNotePercentage:P2}".Replace(",", ".").Replace(" %", "%");
    }

    private void OnBeat(int beat)
    {
        bpmText.FadeIn().FadeTo(.6f, clock.BeatTime);
    }

    protected override void Dispose(bool isDisposing)
    {
        clock.OnBeat -= OnBeat;
        base.Dispose(isDisposing);
    }

    private partial class BackgroundStack : Container
    {
        public BackgroundStack()
        {
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }

        public void ChangeMap(RealmMap map)
        {
            var background = new MapBackground
            {
                Map = map,
                RelativeSizeAxes = Axes.Both,
                FillMode = FillMode.Fill,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            };

            Add(background);
            background.FadeInFromZero(300);
        }

        protected override void Update()
        {
            base.Update();

            while (Children.Count > 1)
            {
                if (Children[1].Alpha == 1)
                    Remove(Children[0], false);
                else
                    break;
            }
        }
    }
}
