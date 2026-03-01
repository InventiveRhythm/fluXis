using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API.Models.Maps.Modding;
using fluXis.Online.Drawables.Images;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Overlay.MapSet.UI.Modding;

public partial class ModdingActionSimple : Container
{
    private readonly APIModdingAction action;

    public ModdingActionSimple(APIModdingAction action)
    {
        this.action = action;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        string actionText =
            action.Type == APIModdingActionType.Submitted ? " submitted the mapset to the queue " :
            action.Type == APIModdingActionType.Update ? " updated the mapset " :
            $" {action.Type.ToString()} "; // fallback for unhandled types

        AutoSizeAxes = Axes.Y;
        RelativeSizeAxes = Axes.X;

        InternalChild = new FillFlowContainer
        {
            Direction = FillDirection.Horizontal,
            AutoSizeAxes = Axes.Y,
            RelativeSizeAxes = Axes.X,
            Children = new Drawable[]
            {
                new ModdingActionTypeCircle(action.Type),
                new LoadWrapper<DrawableAvatar>
                {
                    CornerRadius = 12,
                    Margin = new MarginPadding { Left = 8, Right = 8 },
                    Origin = Anchor.CentreLeft,
                    Anchor = Anchor.CentreLeft,
                    Size = new Vector2(24, 24),
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
                    WebFontSize = 14,
                    Origin = Anchor.CentreLeft,
                    Anchor = Anchor.CentreLeft,
                    Text = action.User.PreferredName,
                },
                new FluXisSpriteText
                {
                    WebFontSize = 14,
                    Origin = Anchor.CentreLeft,
                    Anchor = Anchor.CentreLeft,
                    Alpha = 0.8f,
                    Text = actionText,
                },
                new FluXisSpriteText
                {
                    WebFontSize = 12,
                    Origin = Anchor.CentreLeft,
                    Anchor = Anchor.CentreLeft,
                    Margin = new MarginPadding { Left = 4 },
                    Colour = Theme.Text,
                    Alpha = 0.6f,
                    Text = TimeUtils.GetFromSeconds(action.Timestamp).ToString("d MMM yyyy"),
                }
            }
        };
    }
}
