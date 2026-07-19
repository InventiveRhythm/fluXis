using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Text;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Drawables.Clubs;
using fluXis.Online.Drawables.Images;
using fluXis.Utils.Extensions;
using Humanizer;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Extensions.LocalisationExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Overlay.Navigator.Pages.Rankings;

public partial class RankingsTable : TableContainer
{
    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private OnlineNavigator navigator { get; set; }

    public RankingsTable()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        RowSize = new Dimension(GridSizeMode.Absolute, 32);

        Columns = new TableColumn[]
        {
            new("", Anchor.CentreRight, new Dimension(GridSizeMode.Absolute, 36)),
            new("", Anchor.Centre, new Dimension(GridSizeMode.Absolute, 40)),
            new("Player", Anchor.CentreLeft),
            new("OVR", Anchor.CentreRight, new Dimension(GridSizeMode.Absolute, 52)),
            new("PR", Anchor.CentreRight, new Dimension(GridSizeMode.Absolute, 52)),
            new("Avg. Acc", Anchor.CentreRight, new Dimension(GridSizeMode.Absolute, 72)),
        };
    }

    public void SetData(List<APIUser> users)
    {
        Content = users.Select((u, idx) => new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = $"#{idx + 1}",
                WebFontSize = 12
            },
            new LoadWrapper<DrawableAvatar>
            {
                Size = new Vector2(24),
                CornerRadius = 4,
                Masking = true,
                LoadContent = () => new DrawableAvatar(u)
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            },
            new ClickableContainer
            {
                RelativeSizeAxes = Axes.Both,
                Action = () => navigator?.PushUser(u.ID),
                Child = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Spacing = new Vector2(4),
                    Children = new[]
                    {
                        new ClubTag(u.Club)
                        {
                            Alpha = u.Club == null ? 0 : 1,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            WebFontSize = 10
                        },
                        createName(u)
                    }
                }
            },
            new FluXisSpriteText
            {
                Text = ((int)(u.Statistics?.OverallRating ?? 0)).ToMetric(decimals: 1),
                WebFontSize = 12
            },
            new FluXisSpriteText
            {
                Text = ((int)(u.Statistics?.PotentialRating ?? 0)).ToMetric(decimals: 1),
                WebFontSize = 12
            },
            new FluXisSpriteText
            {
                Text = u.Statistics?.OverallAccuracy.ToLocalisableString("00.00\\%") ?? "",
                WebFontSize = 12
            }
        }).ToArray().ToRectangular();

        Drawable createName(APIUser user)
        {
            if (user.NamePaint is null)
            {
                return new FluXisSpriteText
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Text = user.Username,
                    WebFontSize = 12
                };
            }

            return new GradientText
            {
                Colour = user.NamePaint.Colors.CreateColorInfo(),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Text = user.Username,
                WebFontSize = 12
            };
        }
    }

    protected override Drawable CreateHeader(int index, TableColumn column) => new FluXisSpriteText
    {
        Text = column?.Header ?? string.Empty,
        WebFontSize = 10,
        Alpha = .6f
    };
}
