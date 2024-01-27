using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Containers;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Graphics.UserInterface.Panel;

public partial class PanelContainer : CompositeDrawable
{
    public BufferedContainer BlurContainer { get; init; }

    public Drawable Content
    {
        get => contentContainer.Count == 0 ? null : contentContainer[0];
        set
        {
            contentContainer.Clear();
            if (value != null) contentContainer.Add(value);
        }
    }

    [Resolved]
    private GlobalClock clock { get; set; }

    private Drawable dim;
    private Container contentContainer;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        InternalChild = new PopoverContainer
        {
            RelativeSizeAxes = Axes.Both,
            Children = new[]
            {
                dim = new FullInputBlockingContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Black,
                        Alpha = .5f
                    }
                },
                contentContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both
                }
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        if (Content is { IsLoaded: true, IsPresent: false } && !Content.Transforms.Any())
        {
            Schedule(() => contentContainer.Remove(Content, false));
            dim.Alpha = 0;
            setBlur(0);
            clock.LowPassFilter.Cutoff = LowPassFilter.MAX;
        }
        else if (Content is { IsLoaded: true })
        {
            dim.Alpha = Content.Alpha;
            setBlur(Content.Alpha);

            var lowpass = (LowPassFilter.MAX - LowPassFilter.MIN) * Content.Alpha;
            lowpass = LowPassFilter.MAX - lowpass;
            clock.LowPassFilter.Cutoff = (int)lowpass;
        }
        else if (BlurContainer?.BlurSigma != Vector2.Zero || dim.Alpha != 0)
        {
            dim.Alpha = 0;
            setBlur(0);
        }
    }

    private void setBlur(float blur)
    {
        if (BlurContainer == null)
            return;

        BlurContainer.BlurSigma = new Vector2(blur * 10);
    }
}
