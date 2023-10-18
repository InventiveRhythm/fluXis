using System;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Drawables.Online;
using fluXis.Game.Online.API.Maps;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Map.Drawables;

public partial class MapCard : Container
{
    public APIMapSet MapSet { get; }
    public Action<APIMapSet> OnClickAction { get; set; }

    private Container background;
    private Container cover;

    public MapCard(APIMapSet mapSet)
    {
        MapSet = mapSet;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 430;
        Height = 100;
        CornerRadius = 20;
        Masking = true;

        if (MapSet == null)
        {
            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background3
                },
                new FluXisSpriteText
                {
                    Text = "Missing mapset data.",
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            };
            return;
        }

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background3
            },
            background = new Container
            {
                RelativeSizeAxes = Axes.Both
            },
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = 0.5f
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(10),
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(10),
                Children = new Drawable[]
                {
                    cover = new Container
                    {
                        Size = new Vector2(80),
                        CornerRadius = 10,
                        Masking = true
                    },
                    new FillFlowContainer
                    {
                        Width = 320,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Spacing = new Vector2(-3),
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                RelativeSizeAxes = Axes.X,
                                Text = MapSet.Title,
                                FontSize = 24,
                                Shadow = true,
                                Truncate = true
                            },
                            new FluXisSpriteText
                            {
                                RelativeSizeAxes = Axes.X,
                                Text = $"by {MapSet.Artist}",
                                FontSize = 16,
                                Shadow = true,
                                Truncate = true
                            },
                            new FluXisSpriteText
                            {
                                RelativeSizeAxes = Axes.X,
                                Text = $"mapped by {MapSet.Creator?.GetName()}",
                                FontSize = 16,
                                Shadow = true,
                                Truncate = true
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.X,
                                Height = 20,
                                Margin = new MarginPadding { Top = 5 },
                                Children = new Drawable[]
                                {
                                    new CircularContainer
                                    {
                                        AutoSizeAxes = Axes.X,
                                        RelativeSizeAxes = Axes.Y,
                                        Masking = true,
                                        EdgeEffect = new EdgeEffectParameters
                                        {
                                            Type = EdgeEffectType.Shadow,
                                            Colour = Colour4.Black.Opacity(0.25f),
                                            Radius = 5
                                        },
                                        Children = new Drawable[]
                                        {
                                            new Box
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Colour = FluXisColors.GetStatusColor(MapSet.Status)
                                            },
                                            new FluXisSpriteText
                                            {
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre,
                                                Text = MapSet.Status switch
                                                {
                                                    0 => "Unsubmitted",
                                                    1 => "Pending",
                                                    2 => "Impure",
                                                    3 => "Pure",
                                                    _ => "Unknown"
                                                },
                                                Colour = FluXisColors.TextDark,
                                                FontSize = 16,
                                                Margin = new MarginPadding { Horizontal = 8 }
                                            }
                                        }
                                    },
                                    new CircularContainer
                                    {
                                        AutoSizeAxes = Axes.X,
                                        RelativeSizeAxes = Axes.Y,
                                        Masking = true,
                                        EdgeEffect = new EdgeEffectParameters
                                        {
                                            Type = EdgeEffectType.Shadow,
                                            Colour = Colour4.Black.Opacity(0.25f),
                                            Radius = 5
                                        },
                                        Anchor = Anchor.CentreRight,
                                        Origin = Anchor.CentreRight,
                                        Children = new Drawable[]
                                        {
                                            new Box
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Colour = getKeymodeColor()
                                            },
                                            new FluXisSpriteText
                                            {
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre,
                                                FontSize = 16,
                                                Text = getKeymodeString(),
                                                Margin = new MarginPadding { Horizontal = 8 }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        LoadComponentAsync(new DrawableOnlineBackground(MapSet), background.Add);
        LoadComponentAsync(new DrawableOnlineCover(MapSet), cover.Add);
    }

    protected override bool OnClick(ClickEvent e)
    {
        OnClickAction?.Invoke(MapSet);
        return true;
    }

    private string getKeymodeString()
    {
        var lowest = MapSet.Maps.Min(x => x.KeyMode);
        var highest = MapSet.Maps.Max(x => x.KeyMode);

        return lowest == highest ? $"{lowest}K" : $"{lowest}-{highest}K";
    }

    private ColourInfo getKeymodeColor()
    {
        var lowest = MapSet.Maps.Min(x => x.KeyMode);
        var highest = MapSet.Maps.Max(x => x.KeyMode);

        return ColourInfo.GradientHorizontal(FluXisColors.GetKeyColor(lowest), FluXisColors.GetKeyColor(highest));
    }
}
