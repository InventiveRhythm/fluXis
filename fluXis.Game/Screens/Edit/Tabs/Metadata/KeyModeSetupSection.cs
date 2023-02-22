using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Edit.Tabs.Metadata
{
    public partial class KeyModeSetupSection : SetupSection
    {
        private readonly FillFlowContainer<KeyModeButton> keyModeContainer;

        public KeyModeSetupSection(RealmMap map)
            : base("Keys")
        {
            AddInternal(keyModeContainer = new FillFlowContainer<KeyModeButton>
            {
                CornerRadius = 10,
                Masking = true,
                RelativeSizeAxes = Axes.X,
                Height = 100,
                Children = new[]
                {
                    new KeyModeButton(this, 4),
                    new KeyModeButton(this, 5),
                    new KeyModeButton(this, 6),
                    new KeyModeButton(this, 7),
                }
            });

            SetKeyMode(map.KeyCount);
        }

        public void SetKeyMode(int keyMode)
        {
            keyModeContainer.Children.ForEach(b => b.Selected = b.KeyMode == keyMode);
        }

        private partial class KeyModeButton : Container
        {
            private readonly Box background;
            private readonly FillFlowContainer keyContainer;
            private readonly KeyModeSetupSection parent;

            private bool selected;

            private readonly Colour4 backgroundColour = FluXisColors.Surface;
            private readonly Colour4 backgroundColourHover = FluXisColors.Hover;
            private readonly Colour4 backgroundSelected = FluXisColors.Surface2;
            private readonly Colour4 backgroundColourClick = FluXisColors.Click;

            public int KeyMode { get; }

            public bool Selected
            {
                set
                {
                    selected = value;

                    setBackgroundColour(value ? backgroundSelected : backgroundColour);

                    if (IsHovered)
                        setBackgroundColour(backgroundColourHover);
                }
            }

            public KeyModeButton(KeyModeSetupSection parent, int keys)
            {
                this.parent = parent;
                KeyMode = keys;

                RelativeSizeAxes = Axes.Both;
                Width = .25f;
                Children = new Drawable[]
                {
                    background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = backgroundColour,
                    },
                    new Container
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Children = new Drawable[]
                        {
                            keyContainer = new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Origin = Anchor.TopCentre,
                                Anchor = Anchor.TopCentre,
                            },
                            new SpriteText
                            {
                                Text = $"{keys} Keys",
                                Font = new FontUsage("Quicksand", 20, "SemiBold"),
                                Y = 30,
                                Origin = Anchor.TopCentre,
                                Anchor = Anchor.TopCentre,
                            }
                        }
                    }
                };

                for (int i = 0; i < keys; i++)
                    keyContainer.Add(new TicTac(25));
            }

            private void setBackgroundColour(Colour4 colour)
            {
                background.FadeColour(colour, 200);
            }

            protected override bool OnHover(HoverEvent e)
            {
                setBackgroundColour(backgroundColourHover);
                return base.OnHover(e);
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                setBackgroundColour(selected ? backgroundSelected : backgroundColour);
                base.OnHoverLost(e);
            }

            protected override bool OnClick(ClickEvent e)
            {
                background.Colour = backgroundColourClick;
                setBackgroundColour(backgroundColourHover);
                parent.SetKeyMode(KeyMode);

                return base.OnClick(e);
            }
        }
    }
}
