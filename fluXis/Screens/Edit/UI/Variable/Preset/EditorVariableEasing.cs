using System;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Presets;
using fluXis.Map.Structures.Bases;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Edit.UI.Variable.Preset;

public partial class EditorVariableEasing<T> : EditorVariableDropdown<Easing>
    where T : class, ITimedObject, IHasEasing
{
    public EditorVariableEasing(EditorMap map, T obj)
    {
        Text = "Easing";
        TooltipText = "The easing function used to interpolate between scales.";
        Items = Enum.GetValues<Easing>();
        CurrentValue = obj.Easing;
        OnValueChanged = easing =>
        {
            obj.Easing = easing;
            map.Update(obj);
        };
    }

    protected override Drawable CreateExtraButton() => new Button(easing => Bindable.Value = easing);

    public partial class Button : Container, IHasTooltip
    {
        public LocalisableString TooltipText => "Preview easings.";

        [Resolved]
        private UISamples samples { get; set; }

        [CanBeNull, Resolved(CanBeNull = true)]
        private PanelContainer panels { get; set; }

        private readonly Action<Easing> pick;
        private Container content;
        private HoverLayer hover;
        private FlashLayer flash;

        public Button(Action<Easing> pick)
        {
            this.pick = pick;
        }

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
                        Icon = Phosphor.Bold.MagnifyingGlass
                    }
                }
            };
        }

        protected override bool OnClick(ClickEvent e)
        {
            samples.Click();
            flash.Show();
            panels?.Add(new EasingSelector(pick));
            return true;
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
