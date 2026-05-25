using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Buttons.Presets;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Panel.Types;
using osu.Framework.Allocation;
using osu.Framework.Platform;

namespace fluXis.Graphics.UserInterface.Panel.Presets;

public partial class ExternalLinkPanel : ButtonPanel
{
    [Resolved]
    private FluXisGameBase game { get; set; }

    [Resolved]
    private Clipboard clipboard { get; set; }

    public ExternalLinkPanel(string link)
    {
        Icon = FontAwesome6.Solid.Link;
        Text = "Just to make sure...";
        CreateSubText = flow =>
        {
            flow.AddText("You're about to open the following link in your browser:\n");
            flow.AddText<FluXisSpriteText>(link, text => text.Colour = Theme.Highlight);
        };
        Buttons =
        [
            new PrimaryButtonData(() => game.OpenLink(link, true)),
            new SecondaryButtonData("Copy it instead.", () => clipboard.SetText(link)),
            new CancelButtonData()
        ];
    }
}
