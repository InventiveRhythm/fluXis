using System;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Online.API.Models.Multi;
using fluXis.Online.Drawables.Images;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Multiplayer.SubScreens.Open.List.List;

public partial class LobbySlot : Container, IHasPopover
{
    private MultiplayerRoom room { get; }
    private Action<string> joinAction { get; }

    public LobbySlot(MultiplayerRoom room, Action<string> joinAction)
    {
        this.room = room;
        this.joinAction = joinAction;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 650;
        Height = 90;
        CornerRadius = 10;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background4
            },
            new LoadWrapper<DrawableOnlineBackground>
            {
                RelativeSizeAxes = Axes.Both,
                LoadContent = () => new DrawableOnlineBackground(room.Map),
                OnComplete = d => d.FadeInFromZero(400)
            },
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background2,
                Alpha = .5f
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(20),
                Child = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Y,
                    Width = 300,
                    Direction = FillDirection.Vertical,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Spacing = new Vector2(-2),
                    Children = new Drawable[]
                    {
                        new TruncatingText
                        {
                            Text = room.Name,
                            RelativeSizeAxes = Axes.X,
                            WebFontSize = 24
                        },
                        new TruncatingText
                        {
                            Text = $"hosted by {room.Host.Username}",
                            RelativeSizeAxes = Axes.X,
                            WebFontSize = 14
                        }
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (room.Privacy == MultiplayerPrivacy.Private)
        {
            this.ShowPopover();
            return true;
        }

        joinAction?.Invoke(null);
        return true;
    }

    public Popover GetPopover() => new PasswordPopover(pw => joinAction?.Invoke(pw));

    private partial class PasswordPopover : FluXisPopover
    {
        private Action<string> join { get; }
        private FluXisTextBox box;

        public PasswordPopover(Action<string> join)
        {
            this.join = join;
            ContentPadding = 12;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Content.Child = box = new FluXisTextBox
            {
                Size = new Vector2(220, 40),
                FontSize = FluXisSpriteText.GetWebFontSize(14),
                BorderColour = Theme.Highlight,
                BorderThickness = 4,
                SidePadding = 12,
                IsPassword = true
            };
            box.OnCommitAction = () => join?.Invoke(box.Text);
        }

        protected override void OnFocus(FocusEvent e)
        {
            GetContainingFocusManager()?.ChangeFocus(box);
            base.OnFocus(e);
        }
    }
}
