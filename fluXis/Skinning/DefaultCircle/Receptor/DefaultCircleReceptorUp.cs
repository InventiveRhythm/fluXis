using fluXis.Audio;
using fluXis.Graphics.Sprites.Outline;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Skinning.Bases;
using fluXis.Skinning.Json;
using fluXis.Utils.Extensions;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osuTK;

namespace fluXis.Skinning.DefaultCircle.Receptor;

public partial class DefaultCircleReceptorUp : ColorableSkinDrawable
{
    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private IBeatSyncProvider beatSync { get; set; }

    protected OutlinedCircle Circle { get; }
    private Colour4 colour { get; set; }

    public DefaultCircleReceptorUp(SkinJson skinJson, MapColor index)
        : base(skinJson, index)
    {
        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        AlwaysPresent = true;
        Masking = true;
        Alpha = .25f;

        InternalChild = Circle = new OutlinedCircle
        {
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(DefaultCircleSkin.SCALE),
            BorderColour = Colour4.White,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            BorderThickness = 8
        };
    }

    public override void SetColor(Colour4 color)
    {
        colour = color;
        Circle.Colour = color.Lighten(0.4f);
    }

    protected override void Update()
    {
        base.Update();

        Height = DrawWidth;
    }

    public override void Show()
    {
        var duration = beatSync?.BeatTime ?? 200;
        this.FadeInFromZero().FadeTo(.25f, duration / 2);

        Circle.BorderTo(16).FadeColour(colour)
              .BorderTo(8, duration, Easing.OutQuint).FadeColour(colour.Lighten(.4f), duration / 2);
    }
}
