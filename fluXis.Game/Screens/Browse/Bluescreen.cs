using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Input;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Browse;

public partial class Bluescreen : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override bool ShowToolbar => false;
    public override bool AllowMusicControl => false;
    public override bool ShowCursor => false;
    public override bool AllowExit => false;

    private BindableDouble progress { get; } = new();
    private FluXisSpriteText loadingText;

    [BackgroundDependencyLoader]
    private void load(TextureStore textures, GlobalClock clock)
    {
        clock.Stop();

        InternalChildren = new Drawable[]
        {
            new Box
            {
                Colour = Colour4.FromHex("0079d8"),
                RelativeSizeAxes = Axes.Both
            },
            new FluXisSpriteText
            {
                Text = ":(",
                Shadow = false,
                Position = new Vector2(175),
                FontSize = 200
            },
            new FluXisTextFlow
            {
                Text = "Your PC ran into a problem and needs to restart. We're\njust collecting some error info, and then we'll restart for\nyou.",
                Position = new Vector2(175, 400),
                FontSize = 60,
                Shadow = false,
                RelativeSizeAxes = Axes.Both
            },
            loadingText = new FluXisSpriteText
            {
                Text = "complete",
                Position = new Vector2(175, 600),
                FontSize = 60
            },
            new Sprite
            {
                Texture = textures.Get("QR"),
                Size = new Vector2(125),
                Position = new Vector2(175, 700)
            },
            new FluXisSpriteText
            {
                Text = "For more information about this issue and possible fixes, visit https://www.windows.com/stopcode",
                Shadow = false,
                FontSize = 25,
                Position = new Vector2(320, 700)
            },
            new FluXisTextFlow
            {
                Text = "If you call a support person, give them this info:\nStop code: CRITICAL_PROCESS_DIED",
                Shadow = false,
                FontSize = 20,
                Position = new Vector2(320, 780),
                RelativeSizeAxes = Axes.Both
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        progress.BindValueChanged(e => loadingText.Text = $"{e.NewValue * 100:0}%", true);
    }

    protected override void Update()
    {
        progress.Value += Clock.ElapsedFrameTime / 20000;
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back:
                this.Exit();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
