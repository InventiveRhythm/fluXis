using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Scoring.Structs;
using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.HUD;

public partial class HitErrorBar : GameplayHUDElement
{
    [Resolved]
    private SkinManager skinManager { get; set; }

    private Bindable<float> scaleBind;

    private SpriteIcon icon;
    private Container hits;
    private CircularContainer average;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        scaleBind = config.GetBindable<float>(FluXisSetting.HitErrorScale);

        Anchor = Anchor.Centre;
        Origin = Anchor.TopCentre;
        AutoSizeAxes = Axes.Y;
        Width = Screen.HitWindows.TimingFor(Screen.HitWindows.LowestHitable) * 2f / Screen.Rate;
        Y = 50;

        Screen.JudgementProcessor.ResultAdded += addHit;

        Container colors;

        InternalChildren = new Drawable[]
        {
            icon = new SpriteIcon
            {
                Icon = FontAwesome.Solid.ChevronDown,
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
                    Colour = skinManager.Skin.GetColorForJudgement(timing.Judgement)
                }
            });
        }
    }

    protected override void LoadComplete()
    {
        scaleBind.BindValueChanged(e => Scale = new Vector2(e.NewValue), true);
    }

    private void addHit(HitResult result)
    {
        float time = -result.Difference;
        var judgement = Screen.HitWindows.JudgementFor(time);
        time /= Screen.Rate;

        icon.MoveToX(time, 300, Easing.OutQuint);

        CircularContainer hit = new CircularContainer
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Size = new Vector2(5, 10),
            Masking = true,
            X = time,
            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = skinManager.Skin.GetColorForJudgement(judgement)
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
        float avg = Screen.JudgementProcessor.Results.Average(h => h.Difference);
        var judgement = Screen.HitWindows.JudgementFor(avg);
        avg /= Screen.Rate;

        average.MoveToX(-avg, 100, Easing.OutQuint);
        average.Colour = skinManager.Skin.GetColorForJudgement(judgement);
    }
}
