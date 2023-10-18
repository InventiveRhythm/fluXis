using fluXis.Game.Graphics;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Overlay.Settings;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Select.Footer.Options;

public partial class FooterOptions : VisibilityContainer
{
    protected override bool StartHidden => true;
    public SelectFooterButton Button { get; set; }
    public SelectFooter Footer { get; init; }

    [Resolved]
    private SettingsMenu settings { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 300;
        AutoSizeAxes = Axes.Y;
        X = 450;
        Margin = new MarginPadding { Bottom = 100 };
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomCentre;

        InternalChildren = new Drawable[]
        {
            new Container
            {
                Size = new Vector2(40),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomRight,
                Rotation = 45,
                Y = 20,
                Masking = true,
                CornerRadius = 10,
                EdgeEffect = FluXisStyles.ShadowMedium,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Masking = true,
                CornerRadius = 20,
                EdgeEffect = FluXisStyles.ShadowMedium,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(0, 5),
                        Padding = new MarginPadding(10),
                        Children = new Drawable[]
                        {
                            new FooterOptionButton
                            {
                                Text = "Edit Map",
                                Icon = FontAwesome.Solid.Pen,
                                Action = () =>
                                {
                                    Footer.Screen.EditMapSet(Footer.Screen.MapInfo.Value);
                                    State.Value = Visibility.Hidden;
                                }
                            },
                            new FooterOptionButton
                            {
                                Text = "Game Settings",
                                Icon = FontAwesome.Solid.Cog,
                                Action = () =>
                                {
                                    settings.Show();
                                    State.Value = Visibility.Hidden;
                                }
                            },
                            new FooterOptionButton
                            {
                                Text = "Delete MapSet",
                                Icon = FontAwesome.Solid.Trash,
                                Color = FluXisColors.Red,
                                Action = () =>
                                {
                                    Footer.Screen.OpenDeleteConfirm(Footer.Screen.MapSet.Value);
                                    State.Value = Visibility.Hidden;
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        var delta = Button.ScreenSpaceDrawQuad.Centre.X - ScreenSpaceDrawQuad.Centre.X;
        X += delta;
    }

    protected override bool OnHover(HoverEvent e) => true;
    protected override bool OnClick(ClickEvent e) => true;
    protected override bool OnDragStart(DragStartEvent e) => true;
    protected override bool OnScroll(ScrollEvent e) => true;

    protected override void PopIn()
    {
        this.FadeIn(200).MoveToY(0, 400, Easing.OutQuint);
    }

    protected override void PopOut()
    {
        this.FadeOut(200).MoveToY(40, 400, Easing.OutQuint);
    }
}
