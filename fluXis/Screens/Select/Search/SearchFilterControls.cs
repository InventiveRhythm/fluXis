using System;
using fluXis.Audio;
using fluXis.Configuration;
using fluXis.Graphics;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Localization;
using fluXis.Online.Collections;
using fluXis.Screens.Select.Search.Filters;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Select.Search;

public partial class SearchFilterControls : CompositeDrawable
{
    private readonly Bindable<Collection> collection;

    public SearchFilterControls(Bindable<Collection> collection)
    {
        this.collection = collection;
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config, CollectionManager collections)
    {
        RelativeSizeAxes = Axes.X;
        Width = .5f;
        Height = 40;
        Y = 90;

        InternalChild = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Padding = new MarginPadding { Left = 24 },
            Child = new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                Shear = new Vector2(-.2f, 0),
                ColumnDimensions = new Dimension[]
                {
                    new(GridSizeMode.Absolute, 360),
                    new(GridSizeMode.Absolute, 12),
                    new(),
                    new(GridSizeMode.Absolute, 12),
                    new(),
                    new(GridSizeMode.Absolute, 12),
                    new(GridSizeMode.Absolute, 40),
                },
                Content = new[]
                {
                    new[]
                    {
                        new SearchCollectionFilter(collections, collection),
                        Empty(),
                        new SearchFilterControl<MapUtils.SortingMode>(LocalizationStrings.SongSelect.SortBy, Enum.GetValues<MapUtils.SortingMode>(),
                            config.GetBindable<MapUtils.SortingMode>(FluXisSetting.SortingMode)),
                        Empty(),
                        new SearchFilterControl<MapUtils.GroupingMode>(LocalizationStrings.SongSelect.GroupBy, Enum.GetValues<MapUtils.GroupingMode>(),
                            config.GetBindable<MapUtils.GroupingMode>(FluXisSetting.GroupingMode)),
                        Empty(),
                        new InverseButton(config.GetBindable<bool>(FluXisSetting.SortingInverse)),
                    }
                }
            }
        };
    }

    public override void Show() => this.MoveToX(-100).MoveToX(-10, Styling.TRANSITION_MOVE, Easing.OutQuint);
    public override void Hide() => this.MoveToX(-100, Styling.TRANSITION_MOVE, Easing.OutQuint);

    private partial class InverseButton : CompositeDrawable
    {
        [Resolved]
        private UISamples samples { get; set; }

        private readonly Bindable<bool> inverse;

        private Container content;
        private HoverLayer hover;
        private FlashLayer flash;
        private FluXisSpriteIcon icon;

        public InverseButton(Bindable<bool> inverse)
        {
            this.inverse = inverse;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;

            InternalChild = content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                CornerRadius = 8,
                Masking = true,
                EdgeEffect = Styling.ShadowSmall,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Theme.Background3
                    },
                    hover = new HoverLayer(),
                    flash = new FlashLayer(),
                    icon = new FluXisSpriteIcon
                    {
                        Size = new Vector2(16),
                        Icon = FontAwesome6.Solid.AngleDown,
                        Shear = new Vector2(.2f, 0),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            inverse.BindValueChanged(statusChanged, true);
            FinishTransforms(true);
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            inverse.ValueChanged -= statusChanged;
        }

        private void statusChanged(ValueChangedEvent<bool> v)
        {
            icon.Scale = new Vector2(1, v.NewValue ? -1 : 1);
        }

        protected override bool OnClick(ClickEvent e)
        {
            inverse.Value = !inverse.Value;
            samples.Click();
            flash.Show();
            return true;
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            content.ScaleTo(0.9f, 1000, Easing.OutQuint);
            return true;
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            content.ScaleTo(1, 800, Easing.OutElasticHalf);
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
    }
}
