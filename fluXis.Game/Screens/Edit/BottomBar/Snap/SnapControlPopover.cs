using System;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Skinning;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Edit.BottomBar.Snap;

public partial class SnapControlPopover : CompositeDrawable
{
    [Resolved]
    private EditorSettings settings { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;

        InternalChild = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Full,
            MaximumSize = new Vector2(196, 0),
            ChildrenEnumerable = SnapControl.DefaultSnaps.Select(snap => new SnapChip(snap, updateSnap))
        };
    }

    private void updateSnap(int num)
    {
        settings.SnapDivisor = num;
        this.HidePopover();
    }

    private partial class SnapChip : CompositeDrawable
    {
        [Resolved]
        private UISamples samples { get; set; }

        private int snap { get; }
        private Action<int> action { get; }

        private Box hover;
        private Box flash;

        public SnapChip(int snap, Action<int> action)
        {
            this.snap = snap;
            this.action = action;
        }

        [BackgroundDependencyLoader]
        private void load(SkinManager skinManager)
        {
            Size = new Vector2(96, 42);

            var idx = Array.IndexOf(SnapControl.DefaultSnaps, snap);
            Colour = skinManager.SkinJson.SnapColors.GetColor(idx);

            InternalChildren = new Drawable[]
            {
                hover = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                flash = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                new FluXisSpriteText
                {
                    Text = $"1/{snap.ToOrdinalShort(true)}",
                    WebFontSize = 14,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            };
        }

        protected override bool OnHover(HoverEvent e)
        {
            hover.FadeTo(.2f, 50);
            samples.Hover();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.FadeOut(200);
        }

        protected override bool OnClick(ClickEvent e)
        {
            samples.Click();
            flash.FadeOutFromOne(1000, Easing.OutQuint);
            action?.Invoke(snap);
            return true;
        }
    }
}
