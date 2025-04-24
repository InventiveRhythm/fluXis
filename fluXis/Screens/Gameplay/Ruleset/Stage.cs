using fluXis.Skinning;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Extensions.EnumExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Gameplay.Ruleset;

public partial class Stage : Container
{
    [Resolved]
    private ISkin skin { get; set; }

    private Container center;
    private Container left;
    private Container right;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        InternalChildren = new Drawable[]
        {
            center = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            left = new Container
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreRight
            },
            right = new Container
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreLeft
            }
        };

        Anchor[] parts =
        {
            Anchor.TopCentre,
            Anchor.Centre,
            Anchor.BottomCentre,
            Anchor.TopLeft,
            Anchor.CentreLeft,
            Anchor.BottomLeft,
            Anchor.TopRight,
            Anchor.CentreRight,
            Anchor.BottomRight
        };

        foreach (var part in parts)
        {
            var drawable = skin.GetStageBackgroundPart(part);

            if (drawable is null)
                return;

            if (part == Anchor.Centre)
            {
                drawable.RelativeSizeAxes = Axes.Both;
                drawable.Size = Vector2.One;
            }

            drawable.Anchor = part;
            drawable.Origin = part.Opposite();

            if (part.HasFlagFast(Anchor.x0) || part.HasFlagFast(Anchor.x2))
                drawable.Anchor = part.OppositeHorizontal();

            if (part.HasFlagFast(Anchor.x0))
                left.Add(drawable);
            else if (part.HasFlagFast(Anchor.x2))
                right.Add(drawable);
            else
                center.Add(drawable);
        }
    }

    protected override void Update()
    {
        base.Update();

        fixHeight(center[0], left[0]);
        fixHeight(center[^1], left[^1]);
    }

    private void fixHeight(Drawable target, Drawable reference) => target.Height = reference.DrawHeight;
}
