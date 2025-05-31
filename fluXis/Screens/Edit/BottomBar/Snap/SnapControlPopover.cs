using System;
using System.Linq;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Skinning;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Edit.BottomBar.Snap;

public partial class SnapControlPopover : CompositeDrawable
{
    [Resolved]
    private EditorSettings settings { get; set; }

    private FluXisTextBox custom;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;

        InternalChild = new FillFlowContainer
        {
            Width = 196,
            AutoSizeAxes = Axes.Y,
            Children = new Drawable[]
            {
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Full,
                    ChildrenEnumerable = SnapControl.DefaultSnaps.Select(snap => new SnapChip(snap, _ => updateSnap(snap)))
                },
                custom = new FluXisTextBox
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 42,
                    FontSize = FluXisSpriteText.GetWebFontSize(14),
                    SidePadding = 12,
                    PlaceholderText = "Custom snap...",
                    Text = SnapControl.DefaultSnaps.Contains(settings.SnapDivisor) ? string.Empty : settings.SnapDivisor.ToString(),
                    OnTextChanged = () =>
                    {
                        if (int.TryParse(custom.Text, out int snap) && snap > 0)
                            updateSnap(snap, false);
                        else
                            custom.NotifyError();
                    },
                    OnCommitAction = this.HidePopover
                }
            }
        };
    }

    private void updateSnap(int num, bool hide = true)
    {
        settings.SnapDivisor = num;
        if (hide) this.HidePopover();
    }

    private partial class SnapChip : CompositeDrawable
    {
        [Resolved]
        private UISamples samples { get; set; }

        private int snap { get; }
        private Action<int> action { get; }

        private HoverLayer hover;
        private FlashLayer flash;

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
                hover = new HoverLayer(),
                flash = new FlashLayer(),
                new FluXisSpriteText
                {
                    Text = $"1/{snap.NumberWithOrderSuffix()}",
                    WebFontSize = 14,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            };
        }

        protected override bool OnHover(HoverEvent e)
        {
            hover.Show();
            samples.Hover();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.Hide();
        }

        protected override bool OnClick(ClickEvent e)
        {
            samples.Click();
            flash.Show();
            action?.Invoke(snap);
            return true;
        }
    }
}
