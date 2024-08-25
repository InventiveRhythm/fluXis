using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Skinning;
using fluXis.Game.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Overlay;

public partial class FullComboOverlay : CompositeDrawable
{
    private Box background;
    private OutlinedSquare square;
    private OutlinedSquare diamond;
    private FluXisSpriteText text;

    private Sample sampleFullCombo;
    private Sample sampleAllFlawless;

    [BackgroundDependencyLoader]
    private void load(SkinManager skins)
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        sampleFullCombo = skins.GetFullComboSample();
        sampleAllFlawless = skins.GetAllFlawlessSample();

        InternalChildren = new Drawable[]
        {
            background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2,
                Alpha = 0
            },
            square = new OutlinedSquare
            {
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0, 4),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                BorderThickness = 400
            },
            diamond = new OutlinedSquare
            {
                Size = new Vector2(800),
                Scale = new Vector2(0),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                BorderThickness = 400,
                Rotation = -45
            },
            text = new FluXisSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Scale = new Vector2(0),
                FontSize = 100,
                Shadow = true
            }
        };
    }

    public void Show(FullComboType type)
    {
        this.FadeIn();

        square.ResizeWidthTo(1, 1200, Easing.OutQuint)
              .BorderTo(0, 1200, Easing.OutQuint);

        if (type == FullComboType.AllFlawless)
        {
            sampleAllFlawless?.Play();

            diamond.ScaleTo(1, 1200, Easing.OutQuint)
                   .RotateTo(-45).RotateTo(45, 1200, Easing.OutQuint)
                   .BorderTo(0, 1200, Easing.OutQuint);
        }
        else
            sampleFullCombo?.Play();

        background.FadeTo(.4f, 400);
        text.ScaleTo(1.2f, 1000, Easing.OutQuint);

        text.Text = type switch
        {
            FullComboType.FullCombo => "FULL COMBO",
            FullComboType.AllFlawless => "ALL FLAWLESS",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public override void Hide()
    {
        this.Delay(1200).FadeOut();

        square.ResizeWidthTo(0, 1200, Easing.InQuint)
              .BorderTo(400f, 1200, Easing.InQuint);

        diamond.ScaleTo(0, 1200, Easing.InQuint)
               .RotateTo(135, 1200, Easing.InQuint)
               .BorderTo(400f, 1200, Easing.InQuint);

        background.Delay(800).FadeOut(400);
        text.Delay(200).ScaleTo(0, 1000, Easing.InQuint);
    }

    public enum FullComboType
    {
        FullCombo,
        AllFlawless
    }
}
