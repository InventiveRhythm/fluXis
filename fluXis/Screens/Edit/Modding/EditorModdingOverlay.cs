using fluXis.Graphics;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Screens.Edit.Modding;

public partial class EditorModdingOverlay : CompositeDrawable
{
    [Resolved]
    private EditorModding modding { get; set; }

    private const float top = 45;
    private const float bottom = 60;
    private const float margin = 24;

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(200);
        CornerRadius = 8;
        Masking = true;
        Alpha = 0;

        modding.OnEnable += Show;
        modding.OnDisable += Hide;
    }

    private void createContent()
    {
        EdgeEffect = Styling.ShadowMedium;
        Position = new Vector2(margin, top + margin);

        BorderThickness = 4;
        BorderColour = Theme.Highlight;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background2
            }
        };
    }

    protected override bool OnDragStart(DragStartEvent e) => e.Button == MouseButton.Left;

    protected override void OnDrag(DragEvent e)
    {
        this.MoveTo(e.MousePosition - DrawSize / 2, 200, Easing.OutQuint);
    }

    protected override void OnDragEnd(DragEndEvent e)
    {
        var full = Parent!.DrawSize;
        var center = full / 2;
        float x, y;

        if (e.MousePosition.X > center.X)
            x = full.X - DrawWidth - margin;
        else
            x = margin;

        if (e.MousePosition.Y > center.Y)
            y = full.Y - DrawHeight - margin - bottom;
        else
            y = margin + top;

        this.MoveTo(new Vector2(x, y), 600, Easing.OutElasticQuarter);
    }

    public override void Show()
    {
        if (modding.ViewType != EditorModding.Type.Purifier)
            return;

        createContent();
        this.FadeInFromZero(200).ScaleTo(.9f).ScaleTo(1f, 400, Easing.OutQuint);
    }

    public override void Hide()
    {
        this.FadeOut(200).ScaleTo(.9f, 400, Easing.OutQuint);
    }
}
