using System;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Online.Drawables.Images;
using fluXis.Screens.Edit.Modding;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Charting.Modding;

public partial class EditorPlayfieldModding : CompositeComponent
{
    [Resolved]
    private EditorModding modding { get; set; }

    private Container highlights { get; }
    private Container comments { get; }

    public EditorPlayfieldModding(Container highlights, Container comments)
    {
        this.highlights = highlights;
        this.comments = comments;

        RelativeSizeAxes = Axes.Both;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        modding.OnEnable += () => modding.Requests.ForEach(addRequest);
        modding.OnDisable += () =>
        {
            highlights?.Clear();
            comments.Clear();
        };
    }

    private void addRequest(EditorChangeRequest request)
    {
        comments.Add(new CommentBox(request));
        highlights.Add(new Highlight(request));
    }

    private partial class CommentBox : RequestBase
    {
        public CommentBox(EditorChangeRequest request)
            : base(request)
        {
            AutoSizeAxes = Axes.Both;
            X = 20;

            CornerRadius = 8;
            Masking = true;

            BorderThickness = 4;
            BorderColour = request.AccentColor;

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Theme.Background2
                },
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding(16),
                    Spacing = new Vector2(8),
                    Children = new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Horizontal,
                            Spacing = new Vector2(8),
                            Children = new Drawable[]
                            {
                                new LoadWrapper<DrawableAvatar>
                                {
                                    Size = new Vector2(24),
                                    CornerRadius = 12,
                                    Masking = true,
                                    LoadContent = () => new DrawableAvatar(request.Action.User) { RelativeSizeAxes = Axes.Both },
                                    OnComplete = d => d.FadeInFromZero(400),
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft
                                },
                                new FluXisSpriteText
                                {
                                    Text = request.Action.User.PreferredName,
                                    WebFontSize = 12,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft
                                }
                            }
                        },
                        new FluXisTextFlow
                        {
                            AutoSizeAxes = Axes.Both,
                            MaximumSize = new Vector2(360, 0),
                            WebFontSize = 14,
                            Text = request.Request.CommentContent
                        }
                    }
                }
            };
        }
    }

    private partial class Highlight : RequestBase
    {
        public Highlight(EditorChangeRequest request)
            : base(request)
        {
            RelativeSizeAxes = Axes.X;
            Colour = request.AccentColor;
            Alpha = .4f;

            InternalChild = new Box { RelativeSizeAxes = Axes.Both };
        }

        protected override void UpdateHeight(float? h) => Height = h ?? 100;
    }

    private abstract partial class RequestBase : CompositeDrawable
    {
        [Resolved]
        private EditorPlayfield playfield { get; set; }

        protected EditorChangeRequest Request { get; }

        protected RequestBase(EditorChangeRequest request)
        {
            Request = request;
            Origin = Anchor.BottomLeft;
        }

        protected override void Update()
        {
            base.Update();

            Y = playfield.PositionAtTime(Request.Request.StartTime);

            if (Request.Request.EndTime is null)
            {
                UpdateHeight(null);
                return;
            }

            var end = playfield.PositionAtTime(Request.Request.EndTime.Value);
            UpdateHeight(Math.Abs(Y - end));
        }

        protected virtual void UpdateHeight(float? h)
        {
        }
    }
}
