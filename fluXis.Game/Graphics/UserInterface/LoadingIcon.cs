using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace fluXis.Game.Graphics.UserInterface;

public partial class LoadingIcon : Container
{
    private const int circle_count = 3;
    private const float max_fill = 0.25f;
    private const float factor = 1 / 3f;
    private const float rotation = 360 * factor;

    private Bindable<double> progress { get; } = new();
    private Container<CircularProgress> circles;

    public LoadingIcon()
    {
        Size = new Vector2(100);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Masking = true;
        CornerRadius = 10;

        InternalChildren = new Drawable[]
        {
            circles = new Container<CircularProgress>
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                ChildrenEnumerable = Enumerable.Range(0, circle_count).Select(i => new CircularProgress
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RoundedCaps = true,
                    InnerRadius = 0.3f,
                    Rotation = 360 * (factor * i)
                })
            }
        };
    }

    protected override void LoadComplete()
    {
        rotate();

        progress.BindValueChanged(e => circles.ForEach(c => c.Progress = e.NewValue));
    }

    private void rotate()
    {
        const float duration = 600;

        circles.RotateTo(circles.Rotation + rotation, duration)
               .TransformBindableTo(progress, max_fill, duration, Easing.InOutCubic)
               .Then()
               .RotateTo(circles.Rotation + rotation * 2.2f, duration / 2)
               .TransformBindableTo(progress, 0, duration / 2)
               .Then()
               .OnComplete(_ => rotate());
    }

    public override void Show() => this.FadeIn(200);
    public override void Hide() => this.FadeOut(200);
}
