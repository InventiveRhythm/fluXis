using System;
using fluXis.Audio;
using fluXis.Database.Maps;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Requests.Maps;
using fluXis.Online.API.Requests.MapSets.Votes;
using fluXis.Online.Fluxel;
using fluXis.Overlay.MapSet.UI;
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

    [Resolved]
    private PanelContainer panels { get; set; }

    private bool sendingRequest;
    private int currentVote;

    private FillFlowContainer container;

    private VoteButton upButton;
    private FluXisSpriteText count;
    private VoteButton downButton;
    private FluXisButton voteRateButton;
    private Container alreadyVotedContainer;
    private FluXisSpriteText alreadyVotedText;

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

        //TODO : for this to work properly, fluxel's MapRoute need to pass the userid to map.ToAPI() in order for map.HasVotedRate to be correct
        //       also, to avoid performing two requests, maybe we should create another route that returns both the up/down-vote status and whether the user already voted on rating
        //       ideally we'd also want to retrieve the user's vote value so we can display it (would require more fluxel changes)
        var req = new MapRequest(map.OnlineID);
        req.Success += apiMap => displayRateVoteButton(!apiMap.Data.HasVotedRate);
        req.Failure += ex => displayRateVoteButton(false, "Failed to get vote status");

        api.PerformRequestAsync(req);
    }

    protected override Drawable CreateContent() => new Container
    {
        RelativeSizeAxes = Axes.X,
        AutoSizeAxes = Axes.Y,
        AutoSizeDuration = 400,
        AutoSizeEasing = Easing.Out,
        Children = new Drawable[]
        {
            container = new FillFlowContainer
            {
                Alpha = 0,
                AutoSizeAxes = Axes.Y,
                RelativeSizeAxes = Axes.X,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(20),
                Children = new Drawable[]
                {
                    new Container
                    {
                        AutoSizeAxes = Axes.Y,
                        RelativeSizeAxes = Axes.X,
                        Children = new Drawable[]
                        {
                            count = new FluXisSpriteText
                            {
                                WebFontSize = 20,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                            },
                            upButton = new VoteButton(Theme.VoteUp, FontAwesome6.Solid.AngleUp, () => setVote(1)),
                            downButton = new VoteButton(Theme.VoteDown, FontAwesome6.Solid.AngleDown, () => setVote(-1))
                            {
                                Anchor = Anchor.TopRight,
                                Origin = Anchor.TopRight
                            }
                        }
                    },
                    voteRateButton = new FluXisButton
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 40,
                        Text = "Rate Vote",
                        Action = () => panels.Content = new RateVoteFormPanel(map.OnlineID, displayUserRateVote),
                        Alpha = 0f
                    },
                    alreadyVotedContainer = new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 40,
                        Alpha = 0f,
                        Child = alreadyVotedText = new FluXisSpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            WebFontSize = 16,
                            Text = "Your rate vote: TODO" //TODO: display current vote here (would require changes in fluxel)
                        }
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

    private void displayRateVoteButton(bool display, string customText = "")
    {
        voteRateButton.Alpha = display ? 1f : 0f;
        alreadyVotedContainer.Alpha = display ? 0f : 1f;
        if (customText != "") alreadyVotedText.Text = customText;
    }

    private void displayUserRateVote(float rating)
    {
        voteRateButton.Alpha = 0f;
        alreadyVotedContainer.Alpha = 1f;
        alreadyVotedText.Text = $"Your rate vote: {rating}";
    }

    private partial class VoteButton : CompositeDrawable
    {
        [Resolved]
        private UISamples samples { get; set; }

        public bool Active { set => Colour = value ? color : Theme.Background6; }

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
