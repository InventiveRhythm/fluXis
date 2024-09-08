using System;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Online.API.Requests.Maps.Votes;
using fluXis.Game.Online.Fluxel;
using fluXis.Shared.Components.Maps;
using Humanizer;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Game.Screens.Result.Sides.Types;

public partial class ResultsSideVoting : ResultsSideContainer
{
    protected override LocalisableString Title => "Voting";

    [Resolved]
    private RealmMap map { get; set; }

    [Resolved]
    private IAPIClient api { get; set; }

    private bool sendingRequest = false;
    private int currentVote;

    private Container container;
    private FluXisSpriteText count;

    private FluXisSpriteText error;
    private LoadingIcon loading;

    protected override void LoadComplete()
    {
        base.LoadComplete();

        sendingRequest = true;
        var req = new MapVotesRequest(map.OnlineID);
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

            var req = new MapVotesUpdateRequest(map.OnlineID, vote);
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

        loading.FadeOut(300);
        container.Delay(300).FadeIn(300);
        sendingRequest = false;
    }

    protected override Drawable CreateContent() => new Container()
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
                        WebFontSize = 16,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    },
                    new VoteButton(Colour4.Orange, FontAwesome6.Solid.ChevronUp, () => setVote(1)),
                    new VoteButton(Colour4.CornflowerBlue, FontAwesome6.Solid.ChevronDown, () => setVote(-1))
                    {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight
                    },
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

        private IconUsage icon { get; }
        private Action action { get; }

        private Box hover;
        private Box flash;

        public VoteButton(Colour4 color, IconUsage icon, Action action)
        {
            Colour = color;
            this.icon = icon;
            this.action = action;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Size = new Vector2(48);
            CornerRadius = 8;
            Masking = true;

            InternalChildren = new Drawable[]
            {
                hover = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                flash = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                new SpriteIcon
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
            hover.FadeTo(.2f, 50);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.FadeOut(200);
        }

        protected override bool OnClick(ClickEvent e)
        {
            samples.Click();
            flash.FadeOutFromOne(1000, Easing.OutQuint);
            action?.Invoke();
            return true;
        }
    }
}
