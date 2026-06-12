using System;
using System.Linq;
using fluXis.Audio;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Graphics.UserInterface.Menus.Items;
using fluXis.Screens.Edit.Tabs.Shared.Toolbox;
using fluXis.Screens.Gameplay.Audio.Hitsounds;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Charting.Toolbox;

public partial class ToolboxCustomHitsoundButton : ToolboxHitsoundButton, IHasPopover
{
    public override LocalisableString TooltipText => "Makes this note play a custom hitsound.";

    [Resolved]
    private Hitsounding hitsounding { get; set; }

    private Drawable spacing;
    private EditButton edit;

    public ToolboxCustomHitsoundButton(string display, string sample)
        : base(display, sample)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        var content = Child;
        Remove(content, false);

        Child = new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            ColumnDimensions =
            [
                new Dimension(),
                new Dimension(GridSizeMode.AutoSize),
                new Dimension(GridSizeMode.AutoSize)
            ],
            Content = new[]
            {
                new[]
                {
                    content,
                    spacing = Empty(),
                    edit = new EditButton { Action = this.ShowPopover }
                }
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        const int max = EditorToolbox.CONTENT_OPENED - EditorToolbox.CONTENT_CLOSED;
        var cur = DrawWidth - EditorToolbox.CONTENT_CLOSED;
        var progress = Math.Clamp(cur / max, 0, 1);
        edit.Width = progress * EditorToolbox.CONTENT_CLOSED;
        spacing.Width = progress * 5;
    }

    public Popover GetPopover()
    {
        var menu = new FluXisMenu(Direction.Vertical, true);
        menu.MaskingRadius = 8;
        menu.MaxHeight = 480;
        menu.Items = hitsounding.GetMapFiles(true).Select(x => new MenuActionItem(x, Phosphor.Bold.Waveform, () => apply(x))).ToArray();
        menu.Open();

        var popover = new FluXisPopover
        {
            ContentPadding = 0,
            BodyRadius = 8,
            AllowableAnchors = [Anchor.CentreRight],
            Child = menu
        };

        if (ChartingContainer.Toolbox != null)
        {
            popover.State.BindValueChanged(x =>
            {
                if (x.NewValue == Visibility.Hidden)
                {
                    ChartingContainer.Toolbox.Locked.Value = false;
                    ChartingContainer.Toolbox.Expanded.Value = ChartingContainer.Toolbox.IsHovered;
                }
            });

            ChartingContainer.Toolbox.Locked.Value = true;
        }

        return popover;

        void apply(string str)
        {
            Sample = str;
            UpdateText(str);
            PlaySound();
            Select();
        }
    }

    private partial class EditButton : ClickableContainer
    {
        [Resolved]
        private UISamples samples { get; set; }

        private readonly Container content;
        private readonly HoverLayer hover;
        private readonly FlashLayer flash;

        public EditButton()
        {
            RelativeSizeAxes = Axes.Y;

            InternalChild = content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                CornerRadius = 5,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Theme.Background4
                    },
                    hover = new HoverLayer(),
                    flash = new FlashLayer(),
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Padding = new MarginPadding(12),
                        Child = new FluXisSpriteIcon
                        {
                            Size = new Vector2(24),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Icon = Phosphor.Bold.PencilSimple,
                            Scale = new Vector2(.8f)
                        }
                    }
                }
            };
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            content.ScaleTo(.9f, 1000, Easing.OutQuint);
            return true;
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            content.ScaleTo(1, 1000, Easing.OutElastic);
            base.OnMouseUp(e);
        }

        protected override bool OnHover(HoverEvent e)
        {
            hover.Show();
            samples.Hover();
            return false;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.Hide();
        }

        protected override bool OnClick(ClickEvent e)
        {
            flash.Show();
            samples.Click();
            return base.OnClick(e);
        }
    }
}
