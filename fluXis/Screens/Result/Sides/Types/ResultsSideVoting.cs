using System;
using fluXis.Audio;
using fluXis.Database.Maps;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Requests.MapSets.Votes;
using fluXis.Online.Fluxel;
using Humanizer;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Result.Sides.Types;

public partial class ResultsSideVoting : ResultsSideContainer
{
    protected override LocalisableString Title => "Voting";

    [Resolved]
    private RealmMap map { get; set; }

    [Resolved]
    private IAPIClient api { get; set; }

    private bool sendingRequest;
    private int currentVote;

    private Container container;

    private VoteButton upButton;
    private FluXisSpriteText count;
    private VoteButton downButton;

    private FluXisSpriteText error;
    private LoadingIcon loading;

    protected override void LoadComplete()
    {
        base.LoadComplete();

        sendingRequest = true;
        var req = new MapVotesRequest(map.MapSet.OnlineID);
        req.Success += res => setData(res.Data);
        req.Failure += _ =>
        {
            loading.FadeOut(300);
            error.Delay(300).FadeIn(300);
        };

        api.PerformRequestAsync(req);
    }

    private void setVote(int value)
    {
        if (sendingRequest)
            return;

        sendingRequest = true;
        container.FadeOut(300).OnComplete(_ =>
        {
            if (api.User.Value is null)
            {
                error.Text = "Log in to use this feature.";
                error.FadeIn(300).Then(600).FadeOut(300);
                container.Delay(1200).FadeIn(300);
                sendingRequest = false;
                return;
            }

            loading.FadeIn(300);

            var vote = value;

            if (currentVote == value)
                vote = 0;

            var req = new MapVotesUpdateRequest(map.MapSet.OnlineID, vote);
            req.Success += res => setData(res.Data);
            req.Failure += ex =>
            {
                error.Text = ex.Message;
                loading.FadeOut(300);
                error.Delay(300).FadeIn(300);
            };

            api.PerformRequestAsync(req);
        });
    }

    private void setData(APIMapVotes votes)
    {
        currentVote = votes.YourVote;
        var total = votes.UpVotes - votes.DownVotes;

        // thank you humanizer for long support :thumbsup:
        count.Text = total != 0 ? ((int)total).ToMetric() : "No Votes.";

        upButton.Active = currentVote >= 0;
        downButton.Active = currentVote <= 0;

        loading.FadeOut(300);
        container.Delay(300).FadeIn(300);
        sendingRequest = false;
    }

    protected override Drawable CreateContent() => new Container
    {
        RelativeSizeAxes = Axes.X,
        Height = 48,
        Children = new Drawable[]
        {
            container = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
                Children = new Drawable[]
                {
                    count = new FluXisSpriteText
                    {
                        WebFontSize = 20,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    },
                    upButton = new VoteButton(FluXisColors.VoteUp, FontAwesome6.Solid.AngleUp, () => setVote(1)),
                    downButton = new VoteButton(FluXisColors.VoteDown, FontAwesome6.Solid.AngleDown, () => setVote(-1))
                    {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight
                    }
                }
            },
            error = new FluXisSpriteText
            {
                Text = "Failed to fetch votes...",
                WebFontSize = 14,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0
            },
            loading = new LoadingIcon
            {
                Size = new Vector2(36),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        }
    };

    private partial class VoteButton : CompositeDrawable
    {
        [Resolved]
        private UISamples samples { get; set; }

        public bool Active { set => Colour = value ? color : FluXisColors.Background6; }

        private Colour4 color { get; }
        private IconUsage icon { get; }
        private Action action { get; }

        private HoverLayer hover;
        private FlashLayer flash;

        public VoteButton(Colour4 color, IconUsage icon, Action action)
        {
            this.color = color;
            this.icon = icon;
            this.action = action;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Size = new Vector2(48);
            CornerRadius = 8;
            Masking = true;
            Colour = color;

            InternalChildren = new Drawable[]
            {
                hover = new HoverLayer(),
                flash = new FlashLayer(),
                new FluXisSpriteIcon
                {
                    Icon = icon,
                    Size = new Vector2(28),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            };
        }

        protected override bool OnHover(HoverEvent e)
        {
            samples.Hover();
            hover.Show();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.Hide();
        }

        protected override bool OnClick(ClickEvent e)
        {
            samples.Click();
            flash.Show();
            action?.Invoke();
            return true;
        }
    }
}
