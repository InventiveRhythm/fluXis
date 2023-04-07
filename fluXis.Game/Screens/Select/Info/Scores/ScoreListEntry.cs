using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Database.Score;
using fluXis.Game.Graphics;
using fluXis.Game.Map;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Scoring;
using fluXis.Game.Screens.Result;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Select.Info.Scores;

public partial class ScoreListEntry : Container
{
    [Resolved]
    private MapStore mapStore { get; set; }

    [Resolved]
    private Storage storage { get; set; }

    public ScoreList ScoreList { get; set; }

    private readonly RealmScore score;
    private readonly SpriteText timeText;

    public ScoreListEntry(RealmScore score, int index = -1)
    {
        this.score = score;

        RelativeSizeAxes = Axes.X;
        Height = 50;

        CornerRadius = 10;
        Masking = true;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Surface
            },
            new GridContainer
            {
                ColumnDimensions = new Dimension[]
                {
                    new(GridSizeMode.Absolute, 50),
                    new(),
                    new(GridSizeMode.Absolute, 125)
                },
                RelativeSizeAxes = Axes.Both,
                Content = new[]
                {
                    new Drawable[]
                    {
                        new SpriteText
                        {
                            Text = $"#{index}",
                            Font = FluXisFont.Default(32),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        },
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
                                new SpriteText
                                {
                                    Text = Fluxel.GetLoggedInUser()?.Username ?? "Player",
                                    Font = FluXisFont.Default(28),
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.BottomLeft,
                                    Padding = new MarginPadding { Left = 10 },
                                    Y = 5
                                },
                                timeText = new SpriteText
                                {
                                    Text = TimeUtils.Ago(score.Date),
                                    Font = FluXisFont.Default(),
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.TopLeft,
                                    Padding = new MarginPadding { Left = 10 }
                                },
                                new SpriteText
                                {
                                    Text = score.Accuracy.ToString("00.00").Replace(",", ".") + "%",
                                    Font = FluXisFont.Default(32),
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight,
                                    Padding = new MarginPadding { Right = 10 }
                                }
                            }
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Left = 10, Right = 10 },
                            Children = new Drawable[]
                            {
                                new SpriteText
                                {
                                    Text = score.Score.ToString("0000000"),
                                    Font = FluXisFont.Default(32),
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void Update()
    {
        timeText.Text = TimeUtils.Ago(score.Date);
        base.Update();
    }

    protected override bool OnClick(ClickEvent e)
    {
        RealmMap map = mapStore.CurrentMapSet.Maps.FirstOrDefault(m => m.ID == score.MapID);

        if (map == null)
        {
            Logger.Log("RealmMap is null", LoggingTarget.Runtime, LogLevel.Error);
            return false;
        }

        MapInfo mapInfo = MapUtils.LoadFromPath(storage.GetFullPath($"files/{PathUtils.HashToPath(map.Hash)}"));

        if (mapInfo == null)
        {
            Logger.Log("MapInfo is null", LoggingTarget.Runtime, LogLevel.Error);
            return false;
        }

        Performance performance = new Performance(mapInfo, map.OnlineID, map.Hash);

        // find a better way to do this
        for (int i = 0; i < score.Judgements.Flawless; i++) performance.AddJudgement(Judgement.Flawless);
        for (int i = 0; i < score.Judgements.Perfect; i++) performance.AddJudgement(Judgement.Perfect);
        for (int i = 0; i < score.Judgements.Great; i++) performance.AddJudgement(Judgement.Great);
        for (int i = 0; i < score.Judgements.Alright; i++) performance.AddJudgement(Judgement.Alright);
        for (int i = 0; i < score.Judgements.Okay; i++) performance.AddJudgement(Judgement.Okay);
        for (int i = 0; i < score.Judgements.Miss; i++) performance.AddJudgement(Judgement.Miss);
        for (int i = 0; i < score.MaxCombo; i++) performance.IncCombo();

        ScoreList.MapInfo.Screen.Push(new ResultsScreen(map, mapInfo, performance, false, false));

        return true;
    }
}
