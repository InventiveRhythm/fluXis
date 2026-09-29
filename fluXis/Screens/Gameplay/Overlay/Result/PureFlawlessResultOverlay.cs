using fluXis.Skinning;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace fluXis.Screens.Gameplay.Overlay.Result;

public partial class PureFlawlessResultOverlay : CompositeDrawable
{
    private ResultShearContainer fillLeft;
    private ResultShearContainer fillRight;
    private ResultShearContainer hollow;

    private Box line1;
    private Box line2;

    private Sample sample;

    [BackgroundDependencyLoader]
    private void load(TextureStore textures, ISkin skin)
    {
        sample = skin.GetAllFlawlessSample();

        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        InternalChildren =
        [
            fillLeft = new ResultShearContainer
            {
                X = -180,
                RelativeSizeAxes = Axes.Both,
                Shear = new Vector2(0.25f, 0),
                Anchor = Anchor.Centre,
                Origin = Anchor.CentreRight,
                Children =
                [
                    new Sprite
                    {
                        Texture = textures.Get("Gameplay/Overlay/pure"),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.BottomCentre
                    },
                    new Sprite
                    {
                        Texture = textures.Get("Gameplay/Overlay/flawless"),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.TopCentre
                    }
                ]
            },
            fillRight = new ResultShearContainer
            {
                X = 180,
                RelativeSizeAxes = Axes.Both,
                Shear = new Vector2(0.25f, 0),
                Anchor = Anchor.Centre,
                Origin = Anchor.CentreLeft,
                Children =
                [
                    new Sprite
                    {
                        Texture = textures.Get("Gameplay/Overlay/pure"),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.BottomCentre
                    },
                    new Sprite
                    {
                        Texture = textures.Get("Gameplay/Overlay/flawless"),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.TopCentre
                    }
                ]
            },
            hollow = new ResultShearContainer
            {
                Width = 360,
                RelativeSizeAxes = Axes.Y,
                Shear = new Vector2(0.25f, 0),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children =
                [
                    new Sprite
                    {
                        Texture = textures.Get("Gameplay/Overlay/pure-hollow"),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.BottomCentre
                    },
                    new Sprite
                    {
                        Texture = textures.Get("Gameplay/Overlay/flawless-hollow"),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.TopCentre
                    }
                ]
            },
            new Container
            {
                X = -180,
                Size = new Vector2(6, 1.4f),
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Rotation = 180 / 13f,
                Child = line1 = new Box { RelativePositionAxes = Axes.Both }.WithRelativeSize(Axes.Both)
            },
            new Container
            {
                X = 180,
                Size = new Vector2(6, 1.4f),
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Rotation = 180 / 13f,
                Child = line2 = new Box { RelativePositionAxes = Axes.Both }.WithRelativeSize(Axes.Both)
            }
        ];
    }

    public override void Show()
    {
        base.Show();
        sample?.Play();

        line1.MoveToY(0).ResizeHeightTo(0)
             .ResizeHeightTo(1, 1200, Easing.OutQuint);
        line2.MoveToY(1).ResizeHeightTo(1)
             .MoveToY(0, 1200, Easing.OutQuint);

        ResultShearContainer[] all = [fillLeft, hollow, fillRight];

        for (var i = 0; i < all.Length; i++)
        {
            var drawable = all[i];
            var pure = drawable[0];
            var flawless = drawable[1];
            var mult = i == 1 ? -1 : 1;

            pure.MoveTo(new Vector2(1600 * mult, -24)).MoveToX(-200, 1200, Easing.OutQuint);
            flawless.MoveTo(new Vector2(-1600 * mult, 24)).MoveToX(0, 1200, Easing.OutQuint);
        }
    }

    public override void Hide()
    {
        this.Delay(1200).FadeOut();

        line1.ResizeHeightTo(0, 1200, Easing.InQuint)
             .MoveToY(1, 1200, Easing.InQuint);
        line2.ResizeHeightTo(0, 1200, Easing.InQuint);

        ResultShearContainer[] all = [fillLeft, hollow, fillRight];

        for (var i = 0; i < all.Length; i++)
        {
            var drawable = all[i];
            var pure = drawable[0];
            var flawless = drawable[1];

            var mult = i == 1 ? -1 : 1;

            pure.MoveTo(new Vector2(-1600 * mult, -24), 1200, Easing.InQuint);
            flawless.MoveTo(new Vector2(1600 * mult, 24), 1200, Easing.InQuint);
        }
    }
}
