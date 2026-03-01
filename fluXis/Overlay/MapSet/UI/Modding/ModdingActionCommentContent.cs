using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Online.API.Models.Maps.Modding;
using fluXis.Online.Drawables.Images;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Overlay.MapSet.UI.Modding;

public partial class ModdingActionCommentContent : Container
{
    private readonly APIModdingAction action;

    public ModdingActionCommentContent(APIModdingAction action)
    {
        this.action = action;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Y;
        RelativeSizeAxes = Axes.X;
        Masking = true;
        CornerRadius = 8;
        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background2,
            },
            new FillFlowContainer
            {
                Direction = FillDirection.Vertical,
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding(16),
                Spacing = new Vector2(0, 8),
                Children = new Drawable[]
                {
                    new FillFlowContainer // line with pfp, username and time
                    {
                        Direction = FillDirection.Horizontal,
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Spacing = new Vector2(8, 0),
                        Children = new Drawable[]
                        {
                            new LoadWrapper<DrawableAvatar>
                            {
                                CornerRadius = 10,
                                Origin = Anchor.CentreLeft,
                                Anchor = Anchor.CentreLeft,
                                Size = new Vector2(20, 20),
                                Masking = true,
                                LoadContent = () => new DrawableAvatar(action.User)
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre
                                },
                            },
                            new FluXisSpriteText
                            {
                                Origin = Anchor.CentreLeft,
                                Anchor = Anchor.CentreLeft,
                                WebFontSize = 12,
                                Text = action.User.Username
                            },
                            new FluXisSpriteText
                            {
                                WebFontSize = 10,
                                Origin = Anchor.CentreLeft,
                                Anchor = Anchor.CentreLeft,
                                Text = TimeUtils.GetFromSeconds(action.Timestamp).ToString("d MMM yyyy"),
                                Colour = Theme.Text,
                                Alpha = 0.8f,
                            }
                        }
                    },
                    new FluXisTextFlow
                    {
                        WebFontSize = 14,
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Text = action.Content,
                    }
                }
            }
        };
    }
}
