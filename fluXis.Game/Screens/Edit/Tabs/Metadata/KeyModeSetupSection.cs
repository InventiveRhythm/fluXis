using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Edit.Tabs.Metadata;

public partial class KeyModeSetupSection : SetupSection
{
    [Resolved]
    private EditorValues values { get; set; }

    private readonly FillFlowContainer<KeyModeButton> keyModeContainer;

    public KeyModeSetupSection(RealmMap map)
        : base("Keys")
    {
        AddInternal(new Container
        {
            AutoSizeAxes = Axes.Y,
            RelativeSizeAxes = Axes.X,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Surface
                },
                keyModeContainer = new FillFlowContainer<KeyModeButton>
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
                        new KeyModeButton(this, 7)
                    }
                }
            }
        });

        setKeyMode(map.KeyCount);
    }

    private void setKeyMode(int keyMode)
    {
        keyModeContainer.Children.ForEach(b => b.Selected = b.KeyMode == keyMode);
        values?.Editor.SetKeyMode(keyMode);
    }

    private partial class KeyModeButton : Container
    {
        private readonly Box background;
        private readonly FillFlowContainer<TicTac> keyContainer;
        private readonly KeyModeSetupSection parent;

        private bool selected;

        public int KeyMode { get; }

        public bool Selected
        {
            set
            {
                selected = value;

                background.FadeTo(value ? .1f : 0f, 200);

                if (IsHovered)
                    background.FadeTo(.2f, 200);
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
                    Alpha = 0
                },
                new Container
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        keyContainer = new FillFlowContainer<TicTac>
                        {
                            AutoSizeAxes = Axes.Both,
                            Origin = Anchor.TopCentre,
                            Anchor = Anchor.TopCentre
                        },
                        new FluXisSpriteText
                        {
                            Text = $"{keys} Keys",
                            Y = 30,
                            Origin = Anchor.TopCentre,
                            Anchor = Anchor.TopCentre
                        }
                    }
                }
            };

            for (int i = 0; i < keys; i++)
                keyContainer.Add(new TicTac(25));
        }

        protected override bool OnHover(HoverEvent e)
        {
            background.FadeTo(.2f, 200);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            background.FadeTo(selected ? .1f : 0f, 200);
            base.OnHoverLost(e);
        }

        protected override bool OnClick(ClickEvent e)
        {
            background.FadeTo(.4f)
                      .FadeTo(.2f, 400);
            parent.setKeyMode(KeyMode);

            for (var i = 0; i < keyContainer.Count; i++)
            {
                var ticTac = keyContainer[i];
                ticTac.Delay(i * 50)
                      .ResizeHeightTo(30, 200)
                      .Then()
                      .ResizeHeightTo(25, 200);
            }

            return base.OnClick(e);
        }
    }
}
