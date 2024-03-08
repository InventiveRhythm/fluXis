using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Shared.Components.Other;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace fluXis.Game.Overlay.Achievements;

public partial class AchievementOverlay : CompositeDrawable, ICloseable
{
    private const float initial_delay = 455;
    private const float square_duration = 2000;

    private Achievement achievement { get; }

    private Sample sample;

    private Square backgroundSquare;
    private Container speedySquares;
    private Square mainSquare;
    private Box miniSquare;
    private Square bigSquare;
    private FillFlowContainer textContainer;

    public AchievementOverlay(Achievement achievement)
    {
        this.achievement = achievement;
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures, ISampleStore samples)
    {
        sample = samples.Get($"Achievement/{achievement.Level}");

        RelativeSizeAxes = Axes.Both;
        Anchor = Origin = Anchor.Centre;

        var color = getColor();

        InternalChildren = new Drawable[]
        {
            backgroundSquare = new Square(color)
            {
                Alpha = .6f
            },
            speedySquares = new Container
            {
                RelativeSizeAxes = Axes.Both,
                ChildrenEnumerable = Enumerable.Range(0, achievement.Level switch
                {
                    2 => 4,
                    3 => 8,
                    _ => 1
                }).Select(_ => new Square(color))
            },
            mainSquare = new Square(color, 50)
            {
                Size = new Vector2(0)
            },
            miniSquare = new Box
            {
                Size = new Vector2(0),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = color,
                Rotation = 45
            },
            new Sprite
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Texture = textures.Get($"Achievements/{achievement.ID}"),
                Size = new Vector2(200)
            },
            textContainer = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.Centre,
                Origin = Anchor.TopCentre,
                Y = 340,
                Alpha = 0,
                Children = new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Text = achievement.Name,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Shadow = true,
                        FontSize = 38
                    },
                    new FluXisSpriteText
                    {
                        Text = achievement.Description,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        FontSize = 20,
                        Shadow = true,
                        Alpha = .8f
                    }
                }
            },
            bigSquare = new Square(color, 50)
            {
                Size = new Vector2(2000)
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        this.FadeInFromZero(200);

        bigSquare.ResizeTo(0, initial_delay, Easing.OutQuint);

        float delay = initial_delay;

        foreach (var square in speedySquares)
        {
            square.Delay(delay).ResizeTo(2000, square_duration, Easing.OutQuint).RotateTo(90, square_duration, Easing.OutQuint).Delay(square_duration - 200).FadeOut(200);
            delay += achievement.Level switch
            {
                1 => 400,
                2 => 200,
                3 => 100,
                _ => 0
            };
        }

        if (achievement.Level == 3)
            backgroundSquare.Delay(initial_delay).ResizeTo(2000, square_duration * 2, Easing.OutQuint);

        mainSquare.BorderTo(initial_delay, 10, square_duration, Easing.OutQuint);
        mainSquare.Delay(initial_delay).ResizeTo(400, square_duration, Easing.OutQuint);
        miniSquare.Delay(initial_delay).ResizeTo(100, square_duration, Easing.OutQuint);
        textContainer.Delay(initial_delay + 600).FadeIn(200);

        sample?.Play();
    }

    public void Close() => this.ScaleTo(.9f, 400, Easing.OutQuint).FadeOut(200);

    private ColourInfo getColor()
    {
        if (achievement.Level == 1)
            return Colour4.FromHex("#bf8970");
        if (achievement.Level == 2)
            return Colour4.FromHex("#d4af37");
        if (achievement.Level == 3)
            return getHighestColor();

        return FluXisColors.Highlight;
    }

    private static ColourInfo getHighestColor() => new()
    {
        TopLeft = Colour4.FromHex("#c2752c"),
        TopRight = Colour4.FromHex("#4cb5d6"),
        BottomLeft = Colour4.FromHex("#c37182"),
        BottomRight = Colour4.FromHex("#b570e8")
    };

    private partial class Square : CompositeDrawable
    {
        public Square(ColourInfo color, float border = 10)
        {
            BorderColour = color;
            BorderThickness = border;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Anchor = Origin = Anchor.Centre;
            Rotation = 45;
            Masking = true;

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    AlwaysPresent = true,
                    Alpha = 0
                }
            };
        }

        public void BorderTo(float delay, float thickness, float duration, Easing easing) => this.Delay(delay).TransformTo(nameof(BorderThickness), thickness, duration, easing);
    }
}
