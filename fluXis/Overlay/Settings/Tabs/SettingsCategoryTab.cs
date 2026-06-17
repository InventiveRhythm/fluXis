using System;
using System.Linq;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Overlay.Settings.Tabs;

public partial class SettingsCategoryTab : Container
{
    private SettingsSection section { get; }
    private Bindable<SettingsSection> currentSection { get; }

    [Resolved]
    private UISamples samples { get; set; }

    [CanBeNull]
    public Action<Drawable> ScrollToItem { get; set; }

    private Container scalingContainer;
    private Container subsections;

    private HoverLayer hover;
    private FlashLayer flash;

    private float animation = 0f;

    public SettingsCategoryTab(SettingsSection section, Bindable<SettingsSection> bind)
    {
        this.section = section;
        currentSection = bind;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 48;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            scalingContainer = new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = Height,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.Centre,
                CornerRadius = 8,
                Masking = true,
                Y = Height / 2f,
                Children = new Drawable[]
                {
                    hover = new HoverLayer(),
                    flash = new FlashLayer(),
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.X,
                        RelativeSizeAxes = Axes.Y,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(16),
                        Padding = new MarginPadding(12),
                        Children = new Drawable[]
                        {
                            new FluXisSpriteIcon
                            {
                                Size = new Vector2(24),
                                Icon = section.Icon,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            },
                            new FluXisSpriteText
                            {
                                Text = section.Title,
                                WebFontSize = 16,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            }
                        }
                    }
                }
            },
            subsections = new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding { Top = Height },
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.Y,
                        Width = 4,
                        X = 6,
                        Origin = Anchor.TopCentre,
                        Padding = new MarginPadding { Vertical = 4 },
                        Masking = true,
                        Child = new Circle
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Theme.Foreground,
                        }
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Padding = new MarginPadding { Left = 12 },
                        ChildrenEnumerable = section.SubSections.Select(x =>
                        {
                            var item = new SettingsSubCategoryButton(x) { Action = () => ScrollToItem?.Invoke(x) };
                            x.OnMatchingChanged += v => item.FadeTo(v ? 1 : 0);
                            return item;
                        })
                    },
                }
            },
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        section.OnMatchingChanged += v => this.FadeTo(v ? 1 : 0);

        currentSection.BindValueChanged(v =>
        {
            if (v.NewValue == section)
            {
                scalingContainer.FadeTo(1f, 200);
                this.TransformTo(nameof(animation), 1f, 400, Easing.OutQuint);
            }
            else
            {
                scalingContainer.FadeTo(0.75f, 200);
                this.TransformTo(nameof(animation), 0f, 400, Easing.OutQuint);
            }
        }, true);

        FinishTransforms(true);
    }

    protected override void Update()
    {
        base.Update();

        var height = subsections.Height - scalingContainer.Height;
        Height = scalingContainer.Height + height * animation;
    }

    protected override bool OnClick(ClickEvent e)
    {
        currentSection.Value = section;
        samples.Click();
        flash.Show();
        return true;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        scalingContainer.ScaleTo(.9f, 1000, Easing.OutQuint);
        return false;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        scalingContainer.ScaleTo(1f, 1000, Easing.OutElastic);
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
}
