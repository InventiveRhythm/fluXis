using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Scoring.Structs;
using fluXis.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Screens.Gameplay.HUD.Components;

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
        Width = HitWindows.TimingFor(HitWindows.LowestHitable) * 2f / Deps.PlaybackRate;

        Container colors;

        InternalChildren = new Drawable[]
        {
            icon = new FluXisSpriteIcon
            {
                Icon = FontAwesome6.Solid.AngleDown,
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
                        Colour = Theme.Text
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

        foreach (var timing in HitWindows.GetTimings().Reverse())
        {
            if (timing.Judgement < HitWindows.LowestHitable)
                continue;

            colors.Add(new Container
            {
                RelativeSizeAxes = Axes.Y,
                Width = timing.Milliseconds * 2f / Deps.PlaybackRate,
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

        JudgementProcessor.ResultAdded += addHit;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        JudgementProcessor.ResultAdded -= addHit;
    }

    private void addHit(HitResult result)
    {
        var time = -result.Difference;
        var judgement = HitWindows.JudgementFor(time);
        time /= Deps.PlaybackRate;

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
        var avg = JudgementProcessor.Results.Average(h => h.Difference);
        var judgement = HitWindows.JudgementFor(avg);
        avg /= Deps.PlaybackRate;

        average.MoveToX((float)-avg, 100, Easing.OutQuint);
        average.Colour = skinManager.SkinJson.GetColorForJudgement(judgement);
    }
}
