using System;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Layout;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Multiplayer.SubScreens.Open.Lobby.UI;

public partial class MultiLobbyMarquee : CompositeDrawable
{
    private LocalisableString text;

    public LocalisableString Text
    {
        get => text;
        set
        {
            text = value;
            if (IsLoaded) refreshContent();
        }
    }

    private readonly LayoutValue sizeLayout;
    private bool needsUpdate;

    private readonly FillFlowContainer flow;

    public MultiLobbyMarquee()
    {
        Masking = true;

        InternalChild = flow = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal
        };

        AddLayout(sizeLayout = new LayoutValue(Invalidation.DrawSize));
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        refreshContent();
    }

    private void refreshContent()
    {
        flow.Clear();
        flow.Spacing = new Vector2(Height);

        flow.Add(new FluXisSpriteText
        {
            Text = Text,
            FontSize = Height
        });

        needsUpdate = true;
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        if (!needsUpdate && sizeLayout.IsValid)
            return;

        if (flow.Count < 1)
            return;

        var first = flow.First();
        var count = (int)Math.Ceiling(DrawWidth / first.DrawWidth);

        if (flow.Count >= count)
            return;

        for (int i = 0; i < count; i++)
        {
            flow.Add(new FluXisSpriteText
            {
                Text = Text,
                FontSize = Height
            });
        }

        var size = first.DrawWidth + flow.Spacing.X;
        flow.MoveToX(0).MoveToX(-size, size / Height * 1000).Loop();

        sizeLayout.Validate();
        needsUpdate = false;
    }
}
