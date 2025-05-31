using System;
using fluXis.Database.Maps;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Scoring;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Result.Center;

public partial class ResultsCenterStats : CompositeDrawable
{
    [BackgroundDependencyLoader]
    private void load(RealmMap map, ScoreInfo score)
    {
        RelativeSizeAxes = Axes.X;
        Height = 38;

        InternalChild = new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            Content = new[]
            {
                new Drawable[]
                {
                    new Statistic("Accuracy", $"{score.Accuracy.ToStringInvariant("00.00")}%"),
                    new Statistic("Combo", "", flow =>
                    {
                        flow.AddText($"{score.MaxCombo}x");
                        flow.AddText<FluXisSpriteText>($"/{map.Filters.NoteCount + map.Filters.LongNoteCount * 2}x", text =>
                        {
                            text.WebFontSize = 14;
                            text.Alpha = .6f;
                        });
                    }),
                    new Statistic("Performance", $"{score.PerformanceRating.ToStringInvariant("0.00")}"),
                }
            }
        };
    }

    private partial class Statistic : FillFlowContainer
    {
        public Statistic(string title, string value, Action<FluXisTextFlow> create = null)
        {
            RelativeSizeAxes = Axes.Both;
            Direction = FillDirection.Vertical;
            Spacing = new Vector2(8);

            create ??= textFlow => textFlow.AddText(value);

            var flow = new FluXisTextFlow
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                WebFontSize = 24,
                Shadow = true
            };

            create.Invoke(flow);

            InternalChildren = new Drawable[]
            {
                new ForcedHeightText
                {
                    Text = title,
                    WebFontSize = 16,
                    Height = 12,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = .8f,
                    Shadow = true
                },
                new Container
                {
                    AutoSizeAxes = Axes.X,
                    Height = 18,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Child = flow
                }
            };
        }
    }
}
