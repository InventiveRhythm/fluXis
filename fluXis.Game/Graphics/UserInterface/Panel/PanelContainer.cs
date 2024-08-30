using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Input;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Graphics.UserInterface.Panel;

public partial class PanelContainer : CompositeDrawable, IKeyBindingHandler<FluXisGlobalKeybind>
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

    private Drawable dim;
    private Container contentContainer;

    private AudioFilter lowPass;

    [BackgroundDependencyLoader]
    private void load(AudioManager audio)
    {
        RelativeSizeAxes = Axes.Both;

        InternalChild = new PopoverContainer
        {
            RelativeSizeAxes = Axes.Both,
            Children = new[]
            {
                lowPass = new AudioFilter(audio.TrackMixer),
                dim = new FullInputBlockingContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0,
                    OnClickAction = tryClose,
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
            setCutOff(AudioFilter.MAX);
        }
        else if (Content is { IsLoaded: true })
        {
            dim.Alpha = Content.Alpha;
            setBlur(Content.Alpha);
            setCutOff((int)(AudioFilter.MAX - (AudioFilter.MAX - AudioFilter.MIN) * Content.Alpha));
        }
        else if (BlurContainer?.BlurSigma != Vector2.Zero || dim.Alpha != 0)
        {
            dim.Alpha = 0;
            setBlur(0);
        }
    }

    private void tryClose()
    {
        if (Content is ICloseable closeable)
            closeable.Close();
    }

    private void setBlur(float blur)
    {
        if (BlurContainer == null)
            return;

        BlurContainer.BlurSigma = new Vector2(blur * 10);
    }

    private void setCutOff(int cutoff)
    {
        if (lowPass == null)
            return;

        lowPass.Cutoff = cutoff;
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (Content == null)
            return false;

        if (e.Action == FluXisGlobalKeybind.Back)
            tryClose();

        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
