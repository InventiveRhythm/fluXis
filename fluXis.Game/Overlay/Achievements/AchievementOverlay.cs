using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Online.API.Models.Other;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace fluXis.Game.Overlay.Achievements;

public partial class AchievementOverlay : CompositeDrawable, ICloseable
{
    private const int speedy_square_count = 4;
    private const float delay_per_square = 200;
    private const float square_duration = 2000;

    private Achievement achievement { get; }

    private Container speedySquares;
    private Square mainSquare;
    private Box miniSquare;

    public AchievementOverlay(Achievement achievement)
    {
        this.achievement = achievement;
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Origin = Anchor.Centre;

        InternalChildren = new Drawable[]
        {
            speedySquares = new Container
            {
                RelativeSizeAxes = Axes.Both,
                ChildrenEnumerable = Enumerable.Range(0, speedy_square_count).Select(_ => new Square(achievement.Color))
            },
            mainSquare = new Square(achievement.Color, 50)
            {
                Size = new Vector2(0)
            },
            miniSquare = new Box
            {
                Size = new Vector2(0),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = achievement.Color,
                Rotation = 45
            },
            new Sprite
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Texture = textures.Get($"Achievements/{achievement.ID}"),
                Size = new Vector2(200)
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.Centre,
                Origin = Anchor.TopCentre,
                Y = 340,
                Children = new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Text = achievement.Name,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        FontSize = 38
                    },
                    new FluXisSpriteText
                    {
                        Text = achievement.Description,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        FontSize = 20,
                        Alpha = .8f
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        this.FadeInFromZero(200);

        float delay = 0;

        foreach (var square in speedySquares)
        {
            square.Delay(delay).ResizeTo(2000, square_duration, Easing.OutQuint).RotateTo(90, square_duration, Easing.OutQuint).Delay(square_duration - 200).FadeOut(200);
            delay += delay_per_square;
        }

        mainSquare.BorderTo(10, square_duration, Easing.OutQuint);
        mainSquare.ResizeTo(400, square_duration, Easing.OutQuint);
        miniSquare.ResizeTo(100, square_duration, Easing.OutQuint);
    }

    public void Close() => this.ScaleTo(.9f, 400, Easing.OutQuint).FadeOut(200);

    private partial class Square : CompositeDrawable
    {
        public Square(Colour4 color, float border = 10)
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

        public void BorderTo(float thickness, float duration, Easing easing) => this.TransformTo(nameof(BorderThickness), thickness, duration, easing);
    }
}
