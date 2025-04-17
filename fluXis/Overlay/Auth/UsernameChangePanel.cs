using System;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Online.API.Requests.Users;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Auth.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Overlay.Auth;

public partial class UsernameChangePanel : Panel
{
    [Resolved]
    private IAPIClient api { get; set; }

    private FluXisSpriteText errorText;
    private AuthOverlayTextBox box;

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 540;
        AutoSizeAxes = Axes.Y;

        Content.RelativeSizeAxes = Axes.X;
        Content.AutoSizeAxes = Axes.Y;
        Content.Child = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Padding = new MarginPadding(40),
            Spacing = new Vector2(10),
            Children = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = "Username Change",
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    WebFontSize = 32
                },
                new FluXisTextFlow
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    TextAnchor = Anchor.TopCentre,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    WebFontSize = 14,
                    Text = "Your username has been marked as inappropriate and needs to be changed. Please make sure to choose an appropriate name."
                },
                errorText = new TruncatingText
                {
                    Text = "error message",
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    MaxWidth = 420,
                    Colour = FluXisColors.Red,
                    WebFontSize = 14,
                    Alpha = 0
                },
                Empty().With(d => d.Anchor = d.Origin = Anchor.TopCentre),
                box = new AuthOverlayTextBox
                {
                    PlaceholderText = "New Username",
                },
                new AuthOverlayButton("Change!") { Action = perform }
            }
        };
    }

    private void perform()
    {
        var name = box.Text;

        if (string.IsNullOrEmpty(name))
        {
            setError("Username cannot be empty.");
            return;
        }

        if (api.User.Value.Username.Equals(name, StringComparison.InvariantCultureIgnoreCase))
        {
            setError("New username cannot match the old one.");
            return;
        }

        StartLoading();

        var req = new ChangeUsernameRequest(name);
        req.Success += _ =>
        {
            api.User.Value.Username = name;
            StopLoading();
        };
        req.Failure += ex =>
        {
            setError(ex.Message);
            StopLoading(false);
        };
        api.PerformRequestAsync(req);
    }

    private void setError(string msg)
    {
        if (string.IsNullOrEmpty(msg))
        {
            errorText.Alpha = 0;
            return;
        }

        errorText.Text = msg;
        errorText.Alpha = 1;
    }

    protected override Drawable CreateBackground() => new Box
    {
        RelativeSizeAxes = Axes.Both,
        Colour = FluXisColors.Background2
    };
}
