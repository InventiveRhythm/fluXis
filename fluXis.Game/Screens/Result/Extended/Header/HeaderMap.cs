using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Result.Extended.Header;

public partial class HeaderMap : CompositeDrawable
{
    [Resolved]
    private RealmMap map { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        CornerRadius = 20;
        Masking = true;
        EdgeEffect = FluXisStyles.ShadowMedium;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            new LoadWrapper<MapBackground>
            {
                RelativeSizeAxes = Axes.Both,
                LoadContent = () => new MapBackground(map)
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                OnComplete = d => d.FadeInFromZero(400)
            },
            new SectionedGradient
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = .5f
            },
            new SectionedGradient
            {
                RelativeSizeAxes = Axes.Both,
                Colour = map.Metadata.Color,
                Alpha = .5f
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(10),
                Children = new Drawable[]
                {
                    new LoadWrapper<MapCover>
                    {
                        Size = new Vector2(120),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        CornerRadius = 20,
                        Masking = true,
                        LoadContent = () => new MapCover(map.MapSet)
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        },
                        OnComplete = d => d.FadeInFromZero(400)
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Children = new Drawable[]
                        {
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Direction = FillDirection.Horizontal,
                                Spacing = new Vector2(5),
                                Children = new Drawable[]
                                {
                                    new FluXisSpriteText
                                    {
                                        Text = $"{map.Metadata.Title}",
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        WebFontSize = 24,
                                        Shadow = true
                                    },
                                    new FluXisSpriteText
                                    {
                                        Text = $"by {map.Metadata.Artist}",
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        WebFontSize = 18,
                                        Alpha = .8f,
                                        Shadow = true
                                    }
                                }
                            },
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Direction = FillDirection.Horizontal,
                                Spacing = new Vector2(5),
                                Children = new Drawable[]
                                {
                                    new DifficultyChip
                                    {
                                        Size = new Vector2(80, 20),
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Rating = map.Filters.NotesPerSecond,
                                        FontSize = FluXisSpriteText.GetWebFontSize(14),
                                        EdgeEffect = FluXisStyles.ShadowSmall
                                    },
                                    new FluXisSpriteText
                                    {
                                        Text = $"{map.Difficulty}",
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        WebFontSize = 16,
                                        Alpha = .8f,
                                        Shadow = true
                                    },
                                    new FluXisSpriteText
                                    {
                                        Text = $"mapped by {map.Metadata.Mapper}",
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        WebFontSize = 14,
                                        Alpha = .6f,
                                        Shadow = true
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}
