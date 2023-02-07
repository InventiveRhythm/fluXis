using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Edit;

public partial class EditorToolbar : Container
{
    public EditorToolbar(Editor editor)
    {
        RelativeSizeAxes = Axes.X;
        Height = 50;
        Padding = new MarginPadding(5);

        Child = new Container
        {
            CornerRadius = 10,
            Masking = true,
            RelativeSizeAxes = Axes.Both,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.FromHex("#222228")
                },
                new FillFlowContainer
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    AutoSizeAxes = Axes.X,
                    RelativeSizeAxes = Axes.Y,
                    Direction = FillDirection.Horizontal,
                    Children = new Drawable[]
                    {
                        new EditorToolbarButton("file"),
                        new EditorToolbarButton("edit"),
                        new EditorToolbarButton("view"),
                        new EditorToolbarButton("timing")
                    }
                },
                new FillFlowContainer
                {
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    AutoSizeAxes = Axes.X,
                    RelativeSizeAxes = Axes.Y,
                    Direction = FillDirection.Horizontal,
                    Children = new Drawable[]
                    {
                        new EditorToolbarButton("setup", () => editor.ChangeTab(0)),
                        new EditorToolbarButton("compose", () => editor.ChangeTab(1)),
                        new EditorToolbarButton("timing", () => editor.ChangeTab(2))
                    }
                },
            }
        };
    }

    private partial class EditorToolbarButton : Container
    {
        private readonly Box background;
        private readonly Action onClick;

        public EditorToolbarButton(string text, Action onClick = null)
        {
            this.onClick = onClick ?? (() => { });

            RelativeSizeAxes = Axes.Y;
            AutoSizeAxes = Axes.X;
            Children = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.White,
                    Alpha = 0
                },
                new SpriteText
                {
                    Text = text,
                    Font = new FontUsage("Quicksand", 20, "Bold"),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Padding = new MarginPadding { Horizontal = 10 }
                }
            };
        }

        protected override bool OnHover(HoverEvent e)
        {
            background.FadeTo(.2f, 200);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            background.FadeOut(200);
        }

        protected override bool OnClick(ClickEvent e)
        {
            onClick();
            background.FadeTo(.4f).FadeTo(.2f, 200);
            return true;
        }
    }
}
