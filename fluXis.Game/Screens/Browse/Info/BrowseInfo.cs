using System;
using System.Linq;
using System.Threading;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map;
using fluXis.Game.Online.API.Models.Maps;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.User;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace fluXis.Game.Screens.Browse.Info;

public partial class BrowseInfo : Container
{
    [Resolved]
    private MapStore mapStore { get; set; }

    [Resolved]
    private UserProfileOverlay profile { get; set; }

    public Bindable<APIMapSet> BindableSet { get; set; } = new();

    private Container background;
    private Container cover;
    private FluXisSpriteText title;
    private FluXisSpriteText artist;
    private FluXisButton downloadButton;

    private BrowseInfoChip creatorChip;
    private BrowseInfoChip bpmChip;
    private BrowseInfoChip lengthChip;
    private BrowseInfoChip keysChip;
    private BrowseInfoChip uploadedChip;
    private BrowseInfoChip updatedChip;
    private BrowseInfoChip rankedChip;
    private BrowseInfoChip sourceChip;
    private BrowseInfoTagsChip tagsChip;
    private FillFlowContainer<BrowseInfoMap> mapFlow;

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
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 300,
                CornerRadius = 20,
                Masking = true,
                Children = new Drawable[]
                {
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
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Direction = FillDirection.Vertical,
                        Children = new Drawable[]
                        {
                            cover = new Container
                            {
                                Size = new Vector2(150),
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                CornerRadius = 20,
                                Masking = true,
                                Margin = new MarginPadding { Bottom = 10 },
                                EdgeEffect = FluXisStyles.ShadowSmall
                            },
                            title = new FluXisSpriteText
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                FontSize = 22,
                                Shadow = true
                            },
                            artist = new FluXisSpriteText
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                FontSize = 16,
                                Shadow = true
                            },
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Direction = FillDirection.Horizontal,
                                Margin = new MarginPadding { Top = 10 },
                                Children = new Drawable[]
                                {
                                    downloadButton = new FluXisButton
                                    {
                                        Width = 100,
                                        Height = 40,
                                        FontSize = 20,
                                        Text = "Download",
                                        Action = () => mapStore.DownloadMapSet(BindableSet.Value),
                                        Enabled = false
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Top = 300 },
                Child = new FluXisScrollContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Full,
                        Padding = new MarginPadding(10),
                        Spacing = new Vector2(10),
                        Children = new Drawable[]
                        {
                            creatorChip = new BrowseInfoChip
                            {
                                Title = "Creator",
                                DefaultText = "Unknown Creator"
                            },
                            bpmChip = new BrowseInfoChip
                            {
                                Title = "BPM",
                                DefaultText = "Unknown BPM"
                            },
                            lengthChip = new BrowseInfoChip
                            {
                                Title = "Length",
                                DefaultText = "Unknown Length"
                            },
                            keysChip = new BrowseInfoChip
                            {
                                Title = "Keys",
                                DefaultText = "Unknown Keymode"
                            },
                            uploadedChip = new BrowseInfoChip
                            {
                                Title = "Date Uploaded",
                                DefaultText = "Unknown Date"
                            },
                            updatedChip = new BrowseInfoChip
                            {
                                Title = "Last Updated",
                                DefaultText = "Never Updated"
                            },
                            rankedChip = new BrowseInfoChip
                            {
                                Title = "Date Ranked",
                                DefaultText = "Unranked"
                            },
                            sourceChip = new BrowseInfoChip
                            {
                                Title = "Source",
                                DefaultText = "No Source"
                            },
                            tagsChip = new BrowseInfoTagsChip(),
                            new CircularContainer
                            {
                                Width = 760,
                                Height = 4,
                                Masking = true,
                                Child = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = FluXisColors.Background5
                                }
                            },
                            mapFlow = new FillFlowContainer<BrowseInfoMap>
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Spacing = new Vector2(5)
                            }
                        }
                    }
                }
            }
        };
    }

    private CancellationTokenSource tokenSource;

    protected override void LoadComplete()
    {
        base.LoadComplete();

        BindableSet.BindValueChanged(e =>
        {
            if (e.NewValue == null) return;

            tokenSource ??= new CancellationTokenSource();
            tokenSource.Cancel();

            tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            title.Text = e.NewValue.Title;
            artist.Text = e.NewValue.Artist;
            downloadButton.Enabled = true;

            creatorChip.Text = e.NewValue.Creator.Username;
            creatorChip.OnClickAction = () => profile.ShowUser(e.NewValue.Creator.ID);

            var minBPM = e.NewValue.Maps.Min(x => x.Bpm);
            var maxBPM = e.NewValue.Maps.Max(x => x.Bpm);
            bpmChip.Text = minBPM == maxBPM ? $"{minBPM} BPM" : $"{minBPM}-{maxBPM} BPM";

            lengthChip.Text = $"{TimeUtils.Format(e.NewValue.Maps.Max(x => x.Length), false)}";

            var minKey = e.NewValue.Maps.Min(x => x.KeyMode);
            var maxKey = e.NewValue.Maps.Max(x => x.KeyMode);
            keysChip.Text = minKey == maxKey ? $"{minKey}K" : $"{minKey}-{maxKey}K";

            uploadedChip.Text = DateTimeOffset.FromUnixTimeSeconds(e.NewValue.Submitted).ToString("MMMM dd yyyy");
            updatedChip.Text = DateTimeOffset.FromUnixTimeSeconds(e.NewValue.LastUpdated).ToString("MMMM dd yyyy");
            rankedChip.Text = "";
            sourceChip.Text = e.NewValue.Source;
            tagsChip.Tags = e.NewValue.Tags;

            mapFlow.Clear();

            BindableSet.Value.Maps.ForEach(x => mapFlow.Add(new BrowseInfoMap(BindableSet.Value, x)));

            LoadComponentAsync(new Background(BindableSet.Value), b =>
            {
                Schedule(() =>
                {
                    background.Add(b);
                    b.FadeInFromZero(200).OnComplete(_ =>
                    {
                        if (background.Children.Count > 1)
                            background.Children[0].Expire();
                    });
                });
            }, token);

            LoadComponentAsync(new Cover(BindableSet.Value), c =>
            {
                Schedule(() =>
                {
                    cover.Add(c);
                    c.FadeInFromZero(200).OnComplete(_ =>
                    {
                        if (cover.Children.Count > 1)
                            cover.Children[0].Expire();
                    });
                });
            }, token);
        }, true);
    }

    private partial class Cover : Sprite
    {
        private readonly APIMapSet mapSet;

        public Cover(APIMapSet mapSet)
        {
            this.mapSet = mapSet;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures, FluxelClient fluxel)
        {
            RelativeSizeAxes = Axes.Both;
            FillMode = FillMode.Fill;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Texture = textures.Get($"{fluxel.Endpoint.AssetUrl}/cover/{mapSet.ID}");
        }
    }

    private partial class Background : Sprite
    {
        private readonly APIMapSet mapSet;

        public Background(APIMapSet mapSet)
        {
            this.mapSet = mapSet;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures, FluxelClient fluxel)
        {
            RelativeSizeAxes = Axes.Both;
            FillMode = FillMode.Fill;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Texture = textures.Get($"{fluxel.Endpoint.AssetUrl}/background/{mapSet.ID}");
        }
    }
}
