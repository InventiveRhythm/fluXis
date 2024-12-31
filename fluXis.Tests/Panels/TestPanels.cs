using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Types;
using osu.Framework.Allocation;

namespace fluXis.Tests.Panels;

public partial class TestPanels : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        CreateClock();

        var panelContainer = new PanelContainer();
        Add(panelContainer);

        AddStep("Add ButtonPanel", () =>
        {
            var panel = new ButtonPanel
            {
                Text = "Test",
                SubText = "Test Subtext",
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
