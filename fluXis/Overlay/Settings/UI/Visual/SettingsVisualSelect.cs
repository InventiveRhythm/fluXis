using System.Collections.Generic;
using System.Linq;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Overlay.Settings.UI.Visual;

#nullable enable

public abstract partial class SettingsVisualSelect<T> : FillFlowContainer
{
    private const float square_size = 128;

    public LocalisableString Label { get; init; }
    public LocalisableString Description { get; init; }
    public Bindable<T> Bindable { get; init; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(8);

        InternalChildren = new Drawable[]
        {
            new FillFlowContainer()
            {
                RelativeSizeAxes = Axes.X,
                Height = 22,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(6),
                Children = new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Text = Label,
                        WebFontSize = 16,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft
                    },
                    new QuestionTooltip(Description)
                }
            },
            new GridContainer
            {
                RelativeSizeAxes = Axes.X,
                Height = square_size,
                Content = new[]
                {
                    CreateImages().Cast<Drawable>().ToArray()
                }
            }
        };
    }

    protected abstract IEnumerable<ToggleImage> CreateImages();

    private partial class QuestionTooltip : FluXisSpriteText, IHasTooltip
    {
        public LocalisableString TooltipText { get; }

        public QuestionTooltip(LocalisableString text)
        {
            Text = "?";
            WebFontSize = 16;
            TooltipText = text;
            Alpha = string.IsNullOrWhiteSpace(text.ToString()) ? 0f : .8f;
            Anchor = Origin = Anchor.CentreLeft;
        }
    }

    protected partial class ToggleImage : Container, IHasTooltip
    {
        [Resolved]
        private UISamples samples { get; set; } = null!;

        public LocalisableString TooltipText { get; init; }
        private readonly string path;
        private readonly Bindable<T> bind;
        private readonly T match;

        public ToggleImage(string path, Bindable<T> bind, T match)
        {
            this.path = path;
            this.bind = bind;
            this.match = match;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            Size = new Vector2(square_size);
            Anchor = Origin = Anchor.Centre;
            CornerRadius = 8;
            Masking = true;

            BorderColour = Theme.Highlight;

            InternalChild = new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                FillMode = FillMode.Fill,
                Texture = textures.Get($"Settings/{path}")
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            bind.BindValueChanged(v => this.BorderTo(v.NewValue!.Equals(match) ? 4 : 0, 400, Easing.OutQuint), true);
            FinishTransforms(true);
        }

        protected override bool OnClick(ClickEvent e)
        {
            samples.Click();
            bind.Value = match;
            return true;
        }
    }
}
