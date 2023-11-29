using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Online.Drawables;

public partial class DrawableCountry : Container
{
    public override float Width
    {
        get => base.Width;
        set
        {
            base.Width = value;
            base.Height = value * 3 / 4f;
        }
    }

    public override float Height
    {
        get => base.Height;
        set
        {
            base.Height = value;
            base.Width = value * 4 / 3f;
        }
    }

    public CountryCode Code { get; }

    public DrawableCountry(CountryCode code)
    {
        Code = code;
        CornerRadius = 5;
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Texture = textures.Get($"Flags/{Code}"),
                FillMode = FillMode.Fit
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = Code == CountryCode.Unknown ? 1 : 0,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2
                    },
                    new FluXisSpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Text = "??"
                    }
                }
            }
        };
    }
}
