using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Skinning;
using fluXis.Shared.Scoring.Structs;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.HUD.Components;

public partial class HitErrorBar : GameplayHUDComponent
{
    [Resolved]
    private SkinManager skinManager { get; set; }

    private SpriteIcon icon;
    private Container hits;
    private CircularContainer average;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Y;
        Width = Screen.HitWindows.TimingFor(Screen.HitWindows.LowestHitable) * 2f / Screen.Rate;

        Container colors;

        InternalChildren = new Drawable[]
        {
            icon = new SpriteIcon
            {
                Icon = FontAwesome6.Solid.ChevronDown,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Size = new Vector2(16)
            },
            colors = new Container
            {
                Margin = new MarginPadding { Top = 20 },
                Height = 5,
                RelativeSizeAxes = Axes.X,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                CornerRadius = 2.5f,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.White
                    }
                }
            },
            hits = new Container
            {
                Margin = new MarginPadding { Top = 22.5f },
                RelativeSizeAxes = Axes.X,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre
            },
            average = new CircularContainer
            {
                Size = new Vector2(7, 7),
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Margin = new MarginPadding { Top = 30 },
                Masking = true,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both
                }
            }
        };

        foreach (var timing in Screen.HitWindows.GetTimings().Reverse())
        {
            if (timing.Judgement < Screen.HitWindows.LowestHitable)
                continue;

            colors.Add(new Container
            {
                RelativeSizeAxes = Axes.Y,
                Width = timing.Milliseconds * 2f / Screen.Rate,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                CornerRadius = 2.5f,
                Masking = true,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = skinManager.SkinJson.GetColorForJudgement(timing.Judgement)
                }
            });
        }
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Screen.JudgementProcessor.ResultAdded += addHit;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        Screen.JudgementProcessor.ResultAdded -= addHit;
    }

    private void addHit(HitResult result)
    {
        var time = -result.Difference;
        var judgement = Screen.HitWindows.JudgementFor(time);
        time /= Screen.Rate;

        icon.MoveToX((float)time, 300, Easing.OutQuint);

        CircularContainer hit = new CircularContainer
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Size = new Vector2(5, 10),
            Masking = true,
            X = (float)time,
            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = skinManager.SkinJson.GetColorForJudgement(judgement)
            }
        };

        hits.Add(hit);
        hit.ScaleTo(0)
           .ScaleTo(1f, 200, Easing.OutQuint)
           .Then()
           .FadeOut(300)
           .Expire();

        updateAverage();
    }

    private void updateAverage()
    {
        var avg = Screen.JudgementProcessor.Results.Average(h => h.Difference);
        var judgement = Screen.HitWindows.JudgementFor(avg);
        avg /= Screen.Rate;

        average.MoveToX((float)-avg, 100, Easing.OutQuint);
        average.Colour = skinManager.SkinJson.GetColorForJudgement(judgement);
    }
}
