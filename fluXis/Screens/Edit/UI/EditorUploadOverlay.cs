using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Text;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osuTK;

namespace fluXis.Screens.Edit.UI;

public partial class EditorUploadOverlay : FillFlowContainer, IHasCursorType
{
    CursorType IHasCursorType.Cursor => CursorType.Wait;

    public string Text
    {
        get => text;
        set
        {
            text = value;

            if (topText == null)
                return;

            Scheduler.ScheduleIfNeeded(() => topText.Text = value);
        }
    }

    public string SubText
    {
        get => subText;
        set
        {
            subText = value;

            if (bottomText == null)
                return;

            Scheduler.ScheduleIfNeeded(() => bottomText.Text = value);
        }
    }

    private string text = "Uploading...";
    private string subText = "please wait";

    private FluXisSpriteText topText;
    private FluXisSpriteText bottomText;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Direction = FillDirection.Vertical;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        InternalChildren = new Drawable[]
        {
            new LoadingIcon
            {
                Size = new Vector2(48),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Margin = new MarginPadding { Bottom = 16 }
            },
            topText = new FluXisSpriteText
            {
                Text = Text,
                WebFontSize = 24,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            bottomText = new FluXisSpriteText
            {
                Text = SubText,
                WebFontSize = 20,
                Alpha = .8f,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        this.FadeInFromZero(400);
    }

    public override void Show() => this.FadeIn(400);
    public override void Hide() => this.FadeOut(400);

    protected override bool OnHover(HoverEvent e) => true;
}
