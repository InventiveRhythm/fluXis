using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings;

public partial class PointSettingsTitle : CompositeDrawable
{
    private string title { get; }
    private Action deleteAction { get; }

    public PointSettingsTitle(string title, Action deleteAction)
    {
        this.title = title;
        this.deleteAction = deleteAction;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 30;

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                WebFontSize = 20,
                Text = title,
            },
            new DeleteButton(deleteAction)
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight
            }
        };
    }

    private partial class DeleteButton : PointsList.PointsListIconButton
    {
        protected override IconUsage ButtonIcon => FontAwesome.Solid.Trash;

        public DeleteButton(Action action)
            : base(action)
        {
        }

        protected override void UpdateColors(bool hovered)
        {
            Background.Colour = hovered ? FluXisColors.Red : FluXisColors.Background3;
            Icon.Colour = hovered ? FluXisColors.Background3 : FluXisColors.Text;
        }
    }
}
