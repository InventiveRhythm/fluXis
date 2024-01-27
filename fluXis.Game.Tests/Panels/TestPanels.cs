using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Panel;
using osu.Framework.Allocation;

namespace fluXis.Game.Tests.Panels;

public partial class TestPanels : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        var panelContainer = new PanelContainer();
        Add(panelContainer);

        AddStep("Add ButtonPanel", () =>
        {
            var panel = new ButtonPanel
            {
                Text = "Test",
                SubText = "Test Subtext",
                ButtonWidth = 200,
                Buttons = new[]
                {
                    new ButtonData
                    {
                        Text = "normal button",
                        Action = () => { }
                    },
                    new ButtonData
                    {
                        Text = "dangerous button",
                        Color = FluXisColors.ButtonRed,
                        Action = () => { }
                    },
                    new ButtonData
                    {
                        Text = "green button",
                        Color = FluXisColors.ButtonGreen,
                        Action = () => { }
                    },
                    new ButtonData
                    {
                        Text = "hold button",
                        HoldToConfirm = true,
                        Action = () => { }
                    }
                }
            };

            panelContainer.Content = panel;
        });

        AddStep("Remove Panel", () => panelContainer.Content = null);
    }
}
