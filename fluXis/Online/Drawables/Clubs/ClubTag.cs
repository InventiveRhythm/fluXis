using fluXis.Graphics.Sprites.Text;
using fluXis.Online.API.Models.Clubs;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Online.Drawables.Clubs;

public partial class ClubTag : FillFlowContainer
{
    public float FontSize { init => fontSize = value; }
    public float WebFontSize { init => fontSize = FluXisSpriteText.GetWebFontSize(value); }
    public bool Shadow { get; init; }

    private float fontSize { get; init; } = FluXisSpriteText.GetWebFontSize(24);

    private APIClub club { get; }

    public ClubTag(APIClub club)
    {
        this.club = club;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        if (club == null || club.ID < 1)
            return;

        Direction = FillDirection.Horizontal;
        AutoSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = "[",
                FontSize = fontSize,
                Shadow = Shadow
            },
            new GradientText
            {
                Text = club.Tag,
                FontSize = fontSize,
                Colour = club.CreateColorInfo(),
                Shadow = Shadow
            },
            new FluXisSpriteText
            {
                Text = "]",
                FontSize = fontSize,
                Shadow = Shadow
            }
        };
    }
}
