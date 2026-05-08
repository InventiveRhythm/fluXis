using System;
using fluXis.Database.Maps;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Scoring;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Result.Center;

public partial class ResultsCenterStats : CompositeDrawable
{
    [Resolved]
    private Bindable<ScoreInfo> score { get; set; }

    [Resolved]
    private RealmMap map { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 38;

        setContent();
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        score.BindValueChanged(_ => setContent());
    }

    private void setContent()
    {
        int noteCount = map.Filters.NoteCount + (score.Value.Mods.Contains("NLN") ? map.Filters.LongNoteCount : 0);
        int longNoteCount = score.Value.Mods.Contains("NLN") ? 0 : map.Filters.LongNoteCount;
        int landmineCount = score.Value.Mods.Contains("NMN") ? 0 : map.Filters.LandmineCount;
        int mapMaxCombo = noteCount + longNoteCount * 2 + landmineCount;

        InternalChild = new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            Content = new[]
            {
                new Drawable[]
                {
                    new Statistic("Accuracy", $"{score.Value.Accuracy.ToStringInvariant("00.00")}%"),
                    new Statistic("Combo", "", flow =>
                    {
                        flow.AddText($"{score.Value.MaxCombo}x");
                        flow.AddText<FluXisSpriteText>($"/{mapMaxCombo}x", text =>
                        {
                            text.WebFontSize = 14;
                            text.Alpha = .6f;
                        });
                    }),
                    new Statistic("Performance", $"{score.Value.PerformanceRating.ToStringInvariant("0.00")}"),
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
