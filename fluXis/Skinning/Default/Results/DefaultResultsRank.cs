using fluXis.Scoring.Enums;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace fluXis.Skinning.Default.Results;

public partial class DefaultResultsRank : CompositeDrawable
{
    private ScoreRank rank { get; }

    private Sprite inner;
    private Sprite mid;
    private Sprite outer;
    private Sprite rankSpr;

    public DefaultResultsRank(ScoreRank rank)
    {
        this.rank = rank;
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        InternalChildren = new Drawable[]
        {
            outer = new Sprite
            {
                Size = new Vector2(1800),
                Texture = textures.Get("Results/outer-circle"),
                Origin = Anchor.Centre,
                Blending = BlendingParameters.Additive,
                Alpha = .1f
            },
            mid = new Sprite
            {
                Size = new Vector2(400),
                Texture = textures.Get("Results/circle"),
                Origin = Anchor.Centre,
                Blending = BlendingParameters.Additive,
                Alpha = .1f
            },
            inner = new Sprite
            {
                Size = new Vector2(320),
                Texture = textures.Get("Results/inner-circle"),
                Origin = Anchor.Centre,
                Blending = BlendingParameters.Additive,
                Alpha = .1f
            },
            rankSpr = new Sprite
            {
                Size = new Vector2(160),
                Texture = textures.Get($"Results/rank-{rank.ToString().ToLower()}"),
                Origin = Anchor.Centre
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        outer.Spin(20000, RotationDirection.Clockwise);
        mid.Spin(12000, RotationDirection.Clockwise);
        inner.Spin(10000, RotationDirection.Clockwise);

        if (rank == ScoreRank.X)
            rankSpr.Rainbow();
    }
}
