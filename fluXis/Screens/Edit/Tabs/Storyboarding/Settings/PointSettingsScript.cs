using System;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Storyboarding.Settings;

public partial class PointSettingsScript : PointSettingsTextBox
{
    private readonly ForcedHeightText error;

    public string ErrorText
    {
        set
        {
            error.Text = value;
            error.Alpha = string.IsNullOrWhiteSpace(value) ? 0 : 1;
        }
    }

    [CanBeNull]
    public Action EditExternally { get; set; }

    public PointSettingsScript(string path)
    {
        Text = "Path";
        DefaultText = path;
        TextBoxWidth = 280;

        error = new ForcedHeightText
        {
            WebFontSize = 12,
            Height = 12,
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,
            TextColor = Theme.Red,
            Alpha = 0
        };
    }

    protected override void UpdateLeftTextFlow(FillFlowContainer flow)
    {
        flow.Spacing = new Vector2(4);
        flow.Add(error);
    }

    protected override Drawable CreateExtraButton() => new EditScriptButton { Action = () => EditExternally?.Invoke() };

    private partial class EditScriptButton : Container, IHasTooltip
    {
        public LocalisableString TooltipText => "Edit in external editor.";

        [Resolved]
        private UISamples samples { get; set; }

        public Action Action { get; init; }

        private Container content;
        private HoverLayer hover;
        private FlashLayer flash;

        [BackgroundDependencyLoader]
        private void load()
        {
            Size = new Vector2(32);

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
                        Colour = Theme.Background3
                    },
                    hover = new HoverLayer(),
                    flash = new FlashLayer(),
                    new FluXisSpriteIcon
                    {
                        Size = new Vector2(16),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Icon = FontAwesome6.Solid.Pencil
                    }
                }
            };
        }

        protected override bool OnClick(ClickEvent e)
        {
            samples.Click();
            flash.Show();
            Action?.Invoke();
            return base.OnClick(e);
        }

        protected override bool OnHover(HoverEvent e)
        {
            samples.Hover();
            hover.Show();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.Hide();
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            content.ScaleTo(0.95f, 1000, Easing.OutQuint);
            return true;
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            content.ScaleTo(1, 800, Easing.OutElasticHalf);
        }
    }
}
