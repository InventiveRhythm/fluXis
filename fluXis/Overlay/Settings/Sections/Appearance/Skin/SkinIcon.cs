using fluXis.Audio;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Skinning;
using fluXis.Skinning.Json;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Overlay.Settings.Sections.Appearance.Skin;

public partial class SkinIcon : CompositeDrawable
{
    [Resolved]
    private SkinManager manager { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    private SkinInfo skin { get; }
    private bool isCurrent => manager.SkinInfo.Path == skin.Path;

    private Gradient background;
    private Gradient selected;

    public SkinIcon(SkinInfo skin)
    {
        this.skin = skin;
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        Size = new Vector2(128);
        CornerRadius = 8;
        Masking = true;

        var accent = FluXisColors.Highlight;

        if (!string.IsNullOrWhiteSpace(skin.AccentHex) && Colour4.TryParseHex(skin.AccentHex, out var acc))
            accent = acc;

        BorderColour = accent;
        BorderThickness = 0;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                Texture = skin.IconTexture ?? textures.Get("Skins/missing"),
                FillMode = FillMode.Fill
            },
            background = new Gradient
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = .75f
            },
            selected = new Gradient
            {
                RelativeSizeAxes = Axes.Both,
                Colour = accent,
                Alpha = .5f
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Padding = new MarginPadding(8),
                Spacing = new Vector2(2),
                Children = new Drawable[]
                {
                    new FluXisSpriteIcon
                    {
                        Icon = FontAwesome6.Brands.Steam,
                        Size = new Vector2(12),
                        Alpha = skin.SteamWorkshop ? 1 : 0,
                        Margin = new MarginPadding { Bottom = 4 },
                    },
                    new ForcedHeightText(true)
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 9,
                        WebFontSize = 12,
                        Text = skin.Name,
                    },
                    new ForcedHeightText(true)
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 7,
                        WebFontSize = 10,
                        Text = $"by {skin.Creator}",
                        Alpha = .8f
                    }
                }
            },
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        manager.SkinChanged += updateSelected;
        updateSelected();

        FinishTransforms(true);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        manager.SkinChanged -= updateSelected;
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (isCurrent)
            return false;

        manager.SetSkin(skin);
        samples.SkinSelectClick();
        return true;
    }

    private void updateSelected()
    {
        var state = isCurrent;
        this.BorderTo(state ? 2 : 0, 200);
        selected.FadeTo(state ? .25f : 0, 200);
    }

    protected override bool OnHover(HoverEvent e)
    {
        samples.Hover();
        background.FadeTo(.5f, 50);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        background.FadeTo(.75f, 200);
    }

    private partial class Gradient : CompositeDrawable
    {
        public Gradient()
        {
            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Height = .9f,
                    Colour = ColourInfo.GradientVertical(Colour4.White.Opacity(0), Colour4.White)
                },
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    RelativePositionAxes = Axes.Both,
                    Height = .1f,
                    Colour = Colour4.White,
                    Y = .9f
                }
            };
        }
    }
}
