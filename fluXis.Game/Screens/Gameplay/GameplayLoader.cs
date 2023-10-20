using System.Collections.Generic;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Cover;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Mods;
using fluXis.Game.Online.Activity;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Gameplay;

public partial class GameplayLoader : FluXisScreen
{
    public override float Zoom => 1.3f;
    public override float ParallaxStrength => 10f;
    public override bool ShowToolbar => false;
    public override float BackgroundDim => 0.5f;
    public override float BackgroundBlur => 0.5f;
    public override bool AllowMusicControl => false;
    public override UserActivity InitialActivity => new UserActivity.LoadingGameplay();

    [Resolved]
    private AudioClock clock { get; set; }

    public RealmMap Map { get; set; }
    public List<IMod> Mods { get; set; }

    private Container loadingContainer;
    private FillFlowContainer content;

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Depth = -1;

        InternalChild = content = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Vertical,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Children = new Drawable[]
            {
                new Container
                {
                    Size = new Vector2(250),
                    CornerRadius = 20,
                    Masking = true,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Children = new Drawable[]
                    {
                        new DrawableCover(Map.MapSet)
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            FillMode = FillMode.Fill
                        },
                        loadingContainer = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Colour4.Black,
                                    Alpha = 0.5f
                                },
                                new LoadingIcon
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Size = new Vector2(100)
                                }
                            }
                        }
                    }
                },
                new FluXisSpriteText
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Text = Map.Metadata.Artist,
                    FontSize = 22,
                    Margin = new MarginPadding { Top = 20 }
                },
                new FluXisSpriteText
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Text = Map.Metadata.Title,
                    FontSize = 40
                },
                new FluXisSpriteText
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Text = $"[{Map.Difficulty}] mapped by {Map.Metadata.Mapper}",
                    FontSize = 28
                }
            }
        };

        LoadComponentAsync(new GameplayScreen(Map, Mods), screen =>
        {
            if (this.IsCurrentScreen())
            {
                loadingContainer.FadeOut(200);
                clock.Delay(400).Schedule(() => clock.FadeOut(400));
                this.Delay(800).Schedule(() => this.Push(screen));
            }
            else screen.Dispose();
        });
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.ScaleTo(.9f).ScaleTo(1, 400, Easing.OutQuint).FadeInFromZero(200);
        clock.LowPassFilter.CutoffTo(1000, 400, Easing.OutQuint);
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        this.FadeIn(800).Then().FadeOut(200);
        content.MoveToY(800, 1200, Easing.InQuint);
        clock.LowPassFilter.CutoffTo(LowPassFilter.MAX);
    }

    public override void OnResuming(ScreenTransitionEvent e) => this.Exit();
}
