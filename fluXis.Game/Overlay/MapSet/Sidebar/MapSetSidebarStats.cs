using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Online.API.Models.Maps;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Overlay.MapSet.Sidebar;

public partial class MapSetSidebarStats : FillFlowContainer
{
    private Bindable<APIMap> mapBind { get; }

    private StatNumber length;
    private StatNumber bpm;
    private StatNumber hits;
    private StatNumber lns;
    private StatBar rating;
    private StatBar accuracy;
    private StatBar health;

    public MapSetSidebarStats(Bindable<APIMap> mapBind)
    {
        this.mapBind = mapBind;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(12);
        InternalChildren = new Drawable[]
        {
            new ForcedHeightText
            {
                Text = "Statistics",
                WebFontSize = 20,
                Height = 28
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(16),
                Children = new Drawable[]
                {
                    new GridContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        RowDimensions = new Dimension[] { new(GridSizeMode.AutoSize) },
                        Content = new[]
                        {
                            new Drawable[]
                            {
                                length = new StatNumber("Length", v => TimeUtils.Format(v, false)),
                                bpm = new StatNumber("BPM", v => $"{(int)v}"),
                                hits = new StatNumber("Hits", v => $"{(int)v}"),
                                lns = new StatNumber("LNs", v => $"{(int)v}"),
                            }
                        }
                    },
                    rating = new StatBar("Rating", 30, v => v.ToStringInvariant("0.00")),
                    accuracy = new StatBar("Accuracy", 10, v => v.ToStringInvariant("0.0")),
                    health = new StatBar("Health", 10, v => v.ToStringInvariant("0.0")),
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        mapBind.BindValueChanged(e =>
        {
            var map = e.NewValue;
            length.SetValue(map.Length);
            bpm.SetValue((float)map.BPM);
            hits.SetValue(map.NoteCount);
            lns.SetValue(map.LongNoteCount);
            rating.SetValue((float)map.NotesPerSecond);
            accuracy.SetValue(0);
            health.SetValue(0);
        }, true);

        FinishTransforms(true);
    }

    private partial class StatNumber : FillFlowContainer
    {
        private Func<float, string> format { get; }
        private ForcedHeightText valueText { get; }

        private float val;

        private float valTransform
        {
            get => val;
            set
            {
                val = value;
                valueText.Text = format?.Invoke(val);
            }
        }

        public StatNumber(string label, Func<float, string> format)
        {
            this.format = format;

            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Direction = FillDirection.Vertical;
            Spacing = new Vector2(6);

            InternalChildren = new Drawable[]
            {
                new ForcedHeightText
                {
                    Text = label,
                    WebFontSize = 12,
                    Height = 12,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = .8f
                },
                valueText = new ForcedHeightText
                {
                    Text = "wah",
                    WebFontSize = 14,
                    Height = 14,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            };
        }

        public void SetValue(float v) => this.TransformTo(nameof(valTransform), v, 400, Easing.OutQuint);
    }

    private partial class StatBar : GridContainer
    {
        private float max { get; }
        private Func<float, string> format { get; }
        private FluXisSpriteText valueText { get; }

        private float val;
        private Box background { get; }
        private Circle bar { get; }

        private float valTransform
        {
            get => val;
            set
            {
                val = value;
                valueText.Text = format?.Invoke(val);

                var progress = Math.Clamp(val / max, 0, 1);
                var col = FluXisColors.GetDifficultyColor(progress * 30);
                background.Colour = col.Darken(.8f);
                bar.Colour = col;
                bar.Width = progress;

                var inside = progress >= .2;
                valueText.X = progress;
                valueText.Colour = inside ? Colour4.Black : Colour4.White;
                valueText.Origin = inside ? Anchor.CentreRight : Anchor.CentreLeft;
            }
        }

        public StatBar(string label, float max, Func<float, string> format)
        {
            this.max = max;
            this.format = format;

            RelativeSizeAxes = Axes.X;
            Height = 16;
            ColumnDimensions = new Dimension[]
            {
                new(GridSizeMode.Absolute, 80),
                new()
            };

            Content = new[]
            {
                new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Text = label,
                        WebFontSize = 12,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Margin = new MarginPadding { Horizontal = 4 }
                    },
                    new CircularContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                        Children = new Drawable[]
                        {
                            background = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                            },
                            bar = new Circle
                            {
                                RelativeSizeAxes = Axes.Both,
                            },
                            valueText = new FluXisSpriteText
                            {
                                RelativePositionAxes = Axes.X,
                                Anchor = Anchor.CentreLeft,
                                Text = "wah",
                                Alpha = .75f,
                                WebFontSize = 10,
                                Margin = new MarginPadding { Horizontal = 8 }
                            }
                        }
                    }
                }
            };
        }

        public void SetValue(float v) => this.TransformTo(nameof(valTransform), v, 400, Easing.OutQuint);
    }
}
