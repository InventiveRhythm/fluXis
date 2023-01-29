using System.Collections.Generic;
using System.Globalization;
using fluXis.Game.Map;
using fluXis.Game.Utils;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Timing;

public class TimingCategoryList<T> : Container
{
    public TimingCategoryList(string title, List<T> list, Colour4 background)
    {
        RelativeSizeAxes = Axes.Both;
        Width = 1f / 3f;

        FillFlowContainer flow;

        AddRange(new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = background
            },
            new BasicScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    flow = new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Child = new SpriteText
                        {
                            Text = title,
                            Font = new FontUsage("Quicksand", 32, "Bold"),
                            Margin = new MarginPadding(10)
                        }
                    }
                }
            }
        });

        foreach (var item in list)
        {
            flow.Add(new ListEntry(item));
        }
    }

    private class ListEntry : Container
    {
        public ListEntry(T item)
        {
            RelativeSizeAxes = Axes.X;
            Height = 30;

            Drawable[] children;

            if (item is TimingPointInfo tp)
            {
                children = new Drawable[]
                {
                    new SpriteText
                    {
                        Text = TimeUtils.Format(tp.Time),
                        Font = new FontUsage("Quicksand", 24, "Bold", false, true),
                        Width = 100
                    },
                    new SpriteText
                    {
                        Text = tp.BPM + "bpm",
                        Font = new FontUsage("Quicksand", 24, "Bold"),
                        Width = 70
                    },
                    new SpriteText
                    {
                        Text = tp.Signature + "/4",
                        Font = new FontUsage("Quicksand", 24, "Bold")
                    },
                };
            }
            else if (item is ScrollVelocityInfo sv)
            {
                children = new Drawable[]
                {
                    new SpriteText
                    {
                        Text = TimeUtils.Format(sv.Time),
                        Font = new FontUsage("Quicksand", 24, "Bold", false, true),
                        Width = 100
                    },
                    new SpriteText
                    {
                        Text = sv.Multiplier.ToString(CultureInfo.InvariantCulture) + "x",
                        Font = new FontUsage("Quicksand", 24, "Bold")
                    }
                };
            }
            else
            {
                children = new Drawable[]
                {
                    new SpriteText
                    {
                        Text = item.ToString(),
                        Font = new FontUsage("Quicksand", 24, "Bold")
                    }
                };
            }

            AddRange(new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.White,
                    Alpha = 0
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Margin = new MarginPadding { Left = 10 },
                    Spacing = new Vector2(10, 0),
                    Children = children
                }
            });
        }
    }
}
