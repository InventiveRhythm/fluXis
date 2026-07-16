using System.Collections.Generic;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Text;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Drawables.Clubs;
using fluXis.Online.Drawables.Images;
using fluXis.Utils.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Overlay.Navigator.Pages.UserV2;

public partial class UserProfileHeader : CompositeDrawable
{
    public UserProfileHeader(APIUser user)
    {
        RelativeSizeAxes = Axes.X;
        Height = 144;

        InternalChild = new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            ColumnDimensions =
            [
                new Dimension(GridSizeMode.Absolute, Height),
                new Dimension(GridSizeMode.Absolute, 16),
                new Dimension()
            ],
            Content = new[]
            {
                new Drawable[]
                {
                    new LoadWrapper<DrawableAvatar>
                    {
                        RelativeSizeAxes = Axes.Both,
                        CornerRadius = Height * Styling.AVATAR_ROUNDING_FACTOR,
                        Masking = true,
                        LoadContent = () => new DrawableAvatar(user) { RelativeSizeAxes = Axes.Both },
                        OnComplete = d => d.FadeInFromZero(Styling.TRANSITION_FADE)
                    },
                    Empty(),
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Direction = FillDirection.Vertical,
                        ChildrenEnumerable = createTexts(user)
                    }
                }
            }
        };
    }

    private IEnumerable<Drawable> createTexts(APIUser user)
    {
        var top = new FillFlowContainer
        {
            AutoSizeAxes = Axes.X,
            Height = 20,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(6)
        };

        if (user.Club != null)
        {
            top.Add(new ClubTag(user.Club)
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                WebFontSize = 16
            });
        }

        if (user.NamePaint != null)
        {
            top.Add(new GradientText
            {
                Text = user.PreferredName,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                WebFontSize = 20,
                Colour = user.NamePaint.Colors.CreateColorInfo()
            });
        }
        else
        {
            top.Add(new FluXisSpriteText
            {
                Text = user.PreferredName,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                WebFontSize = 20
            });
        }

        yield return top;

        if (user.HasDisplayName)
        {
            yield return new ForcedHeightText
            {
                Margin = new MarginPadding { Top = 6 },
                Text = user.Username,
                WebFontSize = 16,
                Height = 16,
                Alpha = .8f
            };
        }
    }
}
