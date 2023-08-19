using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Text;
using osu.Framework.Graphics;

namespace fluXis.Game.Overlay.Profile;

public partial class AboutMeProfileSection : ProfileSection
{
    private const string default_about_me = "No description set.";

    public override string Title => "About Me";

    public string AboutMe
    {
        set
        {
            aboutMe = string.IsNullOrEmpty(value) ? default_about_me : value;

            if (textFlow == null) return;

            Schedule(() => textFlow.Text = aboutMe);
            textFlow.Colour = aboutMe == default_about_me ? FluXisColors.Text2 : FluXisColors.Text;
        }
    }

    private FluXisTextFlow textFlow;
    private string aboutMe = default_about_me;

    public override Drawable CreateContent()
    {
        return textFlow = new FluXisTextFlow
        {
            // Text = aboutMe,
            FontSize = 23,
            Shadow = false,
            AutoSizeAxes = Axes.Y,
            RelativeSizeAxes = Axes.X,
            Anchor = Anchor.TopCentre,
            Origin = Anchor.TopCentre,
            TextAnchor = Anchor.TopCentre,
            Colour = aboutMe == default_about_me ? FluXisColors.Text2 : FluXisColors.Text
        };
    }
}
