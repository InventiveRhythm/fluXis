using fluXis.Game.Database.Score;
using fluXis.Game.Graphics;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Utils;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Select.Info.Scores;

public partial class ScoreListEntry : Container
{
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
                    new(GridSizeMode.Absolute, 100),
                },
                RelativeSizeAxes = Axes.Both,
                Content = new[]
                {
                    new Drawable[]
                    {
                        new SpriteText
                        {
                            Text = $"#{index}",
                            Font = new FontUsage("Quicksand", 32, "Bold"),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Y = -2
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
                                    Font = new FontUsage("Quicksand", 28, "Bold"),
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.BottomLeft,
                                    Padding = new MarginPadding { Left = 10 },
                                    Y = 4
                                },
                                timeText = new SpriteText
                                {
                                    Text = TimeUtils.Ago(score.Date),
                                    Font = new FontUsage("Quicksand", 20, "Bold"),
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.TopLeft,
                                    Padding = new MarginPadding { Left = 10 },
                                    Y = -1
                                },
                                new SpriteText
                                {
                                    Text = score.Accuracy.ToString("00.00").Replace(",", ".") + "%",
                                    Font = new FontUsage("Quicksand", 32, "Bold"),
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight,
                                    Padding = new MarginPadding { Right = 10 },
                                    Y = -2
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
                                    Font = new FontUsage("Quicksand", 32, "Bold"),
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight,
                                    Y = -2
                                },
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
}
