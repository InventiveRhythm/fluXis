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

public partial class FullComboResultOverlay : CompositeDrawable
{
    private ResultShearContainer fill;
    private ResultShearContainer hollow;
    private Box line;

    private Sample sample;

    [BackgroundDependencyLoader]
    private void load(TextureStore textures, ISkin skin)
    {
        sample = skin.GetFullComboSample();

        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        InternalChildren =
        [
            fill = new ResultShearContainer
            {
                RelativeSizeAxes = Axes.Both,
                Shear = new Vector2(0.25f, 0),
                Anchor = Anchor.Centre,
                Origin = Anchor.CentreRight,
                Children =
                [
                    new Sprite
                    {
                        Texture = textures.Get("Gameplay/Overlay/full"),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.BottomCentre
                    },
                    new Sprite
                    {
                        Texture = textures.Get("Gameplay/Overlay/combo"),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.TopCentre
                    }
                ]
            },
            hollow = new ResultShearContainer
            {
                RelativeSizeAxes = Axes.Both,
                Shear = new Vector2(0.25f, 0),
                Anchor = Anchor.Centre,
                Origin = Anchor.CentreLeft,
                Children =
                [
                    new Sprite
                    {
                        Texture = textures.Get("Gameplay/Overlay/full-hollow"),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.BottomCentre
                    },
                    new Sprite
                    {
                        Texture = textures.Get("Gameplay/Overlay/combo-hollow"),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.TopCentre
                    }
                ]
            },
            new Container
            {
                Size = new Vector2(6, 1.4f),
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Rotation = 180 / 13f,
                Child = line = new Box { RelativePositionAxes = Axes.Both }.WithRelativeSize(Axes.Both)
            }
        ];
    }

    public override void Show()
    {
        base.Show();
        sample?.Play();

        line.MoveToY(0).ResizeHeightTo(0)
            .ResizeHeightTo(1.2f, 1200, Easing.OutQuint);

        ResultShearContainer[] all = [fill, hollow];

        foreach (var drawable in all)
        {
            var full = drawable[0];
            var combo = drawable[1];

            full.MoveTo(new Vector2(1600, -24)).MoveToX(-290, 1200, Easing.OutQuint);
            combo.MoveTo(new Vector2(-1600, 24)).MoveToX(140, 1200, Easing.OutQuint);
        }
    }

    public override void Hide()
    {
        this.Delay(1200).FadeOut();

        line.ResizeHeightTo(0, 1200, Easing.InQuint)
            .MoveToY(1.2f, 1200, Easing.InQuint);

        ResultShearContainer[] all = [fill, hollow];

        foreach (var drawable in all)
        {
            var full = drawable[0];
            var combo = drawable[1];

            full.MoveTo(new Vector2(-1600, -24), 1200, Easing.InQuint);
            combo.MoveTo(new Vector2(1600, 24), 1200, Easing.InQuint);
        }
    }
}
