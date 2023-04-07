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
}
