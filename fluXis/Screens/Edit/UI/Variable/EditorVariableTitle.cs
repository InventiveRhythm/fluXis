using System;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Overlay.Wiki;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using Humanizer;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Screens.Edit.UI.Variable;

public partial class EditorVariableTitle : CompositeDrawable
{
    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private WikiOverlay wiki { get; set; }

    private string title { get; }
    private Action deleteAction { get; }
    private bool showWiki { get; }

    public EditorVariableTitle(string title, Action deleteAction, bool showWiki = true)
    {
        this.showWiki = showWiki;
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
            new PointsList.PointsListIconButton(() => wiki?.NavigateTo($"/editor/events/{title.Kebaberize()}"))
            {
                ButtonIcon = FontAwesome.Solid.Book,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                X = -40,
                Alpha = showWiki ? 1 : 0
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
        public DeleteButton(Action action)
            : base(action)
        {
            ButtonIcon = FontAwesome.Solid.Trash;
        }

        protected override void UpdateColors(bool hovered)
        {
            Background.Colour = hovered ? Theme.Red : Theme.Background3;
            Icon.Colour = hovered ? Theme.Background3 : Theme.Text;
        }
    }
}
