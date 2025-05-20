using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Audio;
using fluXis.Graphics;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Map;
using fluXis.Map.Drawables;
using fluXis.Mods;
using fluXis.Mods.Drawables;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Multiplayer.SubScreens.Open.Lobby.UI.Disc;

public partial class MultiLobbyDisc : CircularContainer
{
    [Resolved]
    private MapStore maps { get; set; }

    [Resolved]
    private IAmplitudeProvider amplitudes { get; set; }

    private Container spinning;
    private SpriteStack<MapCover> covers;
    private FlashLayer flash;

    private ForcedHeightText title;
    private ForcedHeightText artist;
    private ForcedHeightText difficulty;
    private RoundedChip modeChip;
    private DifficultyChip difficultyChip;
    private ModList modsList;

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        BorderThickness = 24;
        Masking = true;
        EdgeEffect = FluXisStyles.Glow(Colour4.White, 16);

        Children = new Drawable[]
        {
            spinning = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    new BufferedContainer(cachedFrameBuffer: true)
                    {
                        RelativeSizeAxes = Axes.Both,
                        BlurSigma = new Vector2(12),
                        RedrawOnScale = false,
                        FrameBufferScale = new Vector2(.25f),
                        Child = covers = new SpriteStack<MapCover>
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        }
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Black,
                        Alpha = .5f
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(.6f),
                X = 80,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Child = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Spacing = new Vector2(8),
                    Children = new Drawable[]
                    {
                        title = new ForcedHeightText(true, 800)
                        {
                            WebFontSize = 32,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Shadow = true
                        },
                        artist = new ForcedHeightText(true, 800)
                        {
                            WebFontSize = 24,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Shadow = true
                        },
                        difficulty = new ForcedHeightText(true, 800)
                        {
                            WebFontSize = 16,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Shadow = true,
                            Alpha = .8f
                        },
                        new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Direction = FillDirection.Horizontal,
                            Spacing = new Vector2(6),
                            Children = new Drawable[]
                            {
                                modeChip = new RoundedChip
                                {
                                    Height = 20,
                                    TextColour = Colour4.Black.Opacity(.75f)
                                },
                                difficultyChip = new DifficultyChip
                                {
                                    Width = 80,
                                    Height = 20
                                }
                            }
                        },
                        modsList = new ModList
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Margin = new MarginPadding { Top = 16 }
                        }
                    }
                }
            },
            flash = new FlashLayer()
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        maps.MapBindable.BindValueChanged(v =>
        {
            var map = v.NewValue;
            covers.Add(new MapCover(map.MapSet) { FadeDuration = 0 });

            var col = map.Metadata.Color;
            var info = ColourInfo.GradientVertical(col.Lighten(0.2f), col);
            this.BorderColorTo(info);
            FadeEdgeEffectTo(col.Opacity(.5f));

            title.Text = map.Metadata.LocalizedTitle;
            artist.Text = map.Metadata.LocalizedArtist;
            difficulty.Text = map.Difficulty;
            modeChip.Text = $"{map.KeyCount}K";
            modeChip.BackgroundColour = FluXisColors.GetKeyColor(map.KeyCount);
            difficultyChip.RealmMap = map;

            flash.Show();
        }, true);

        spinning.Spin(60000, RotationDirection.Clockwise);
    }

    protected override void Update()
    {
        base.Update();

        var size = Math.Min(Parent!.DrawWidth, Parent.DrawHeight);
        Size = new Vector2(size);

        var amp = amplitudes.Amplitudes[..3].Average();
        spinning.Scale = new Vector2(1 + amp * 0.02f);
    }

    public void UpdateMods(List<IMod> mods) => modsList.Mods = mods;

    public override void Show()
    {
        this.MoveToX(100).RotateTo(10)
            .RotateTo(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint)
            .MoveToX(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint)
            .FadeInFromZero(FluXisScreen.FADE_DURATION);
    }

    public override void Hide()
    {
        this.RotateTo(10, FluXisScreen.MOVE_DURATION, Easing.OutQuint)
            .MoveToX(100, FluXisScreen.MOVE_DURATION, Easing.OutQuint)
            .FadeOut(FluXisScreen.FADE_DURATION);
    }
}
