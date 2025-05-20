using fluXis.Database.Maps;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Map.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Result.Header;

public partial class ResultsMap : CompositeDrawable
{
    [Resolved]
    private RealmMap map { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;

        InternalChild = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(12),
            Children = new Drawable[]
            {
                new LoadWrapper<MapCover>
                {
                    Size = new Vector2(88),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    EdgeEffect = FluXisStyles.ShadowMedium,
                    CornerRadius = 12,
                    Masking = true,
                    LoadContent = () => new MapCover(map.MapSet)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                },
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Children = new Drawable[]
                    {
                        new FluXisSpriteText
                        {
                            Text = map.Metadata.LocalizedTitle,
                            WebFontSize = 20,
                            Shadow = true
                        },
                        new FluXisSpriteText
                        {
                            Text = map.Metadata.LocalizedArtist,
                            WebFontSize = 14,
                            Shadow = true,
                            Alpha = .8f
                        },
                        new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Horizontal,
                            Margin = new MarginPadding { Top = 4 },
                            Spacing = new Vector2(8),
                            Children = new Drawable[]
                            {
                                new DifficultyChip
                                {
                                    Size = new Vector2(80, 20),
                                    RealmMap = map,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    EdgeEffect = FluXisStyles.ShadowSmall
                                },
                                new FluXisSpriteText
                                {
                                    Text = $"{map.Difficulty}",
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    WebFontSize = 14,
                                    Shadow = true
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}
