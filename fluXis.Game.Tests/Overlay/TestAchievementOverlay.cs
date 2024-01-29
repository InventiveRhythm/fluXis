using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Online.API.Models.Other;
using fluXis.Game.Overlay.Achievements;
using osu.Framework.Allocation;

namespace fluXis.Game.Tests.Overlay;

public partial class TestAchievementOverlay : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        var panels = new PanelContainer();
        Add(panels);

        AddStep("Show Default", () => panels.Content = new AchievementOverlay(new Achievement
        {
            ID = "test-1",
            Name = "Test Achievement",
            Description = "This is a test achievement."
        }));

        AddStep("Show Red", () => panels.Content = new AchievementOverlay(new Achievement
        {
            ID = "test-2",
            Name = "Test Achievement",
            Description = "This is a test achievement.",
            ColorHex = "#ff5555"
        }));

        AddStep("Show Green", () => panels.Content = new AchievementOverlay(new Achievement
        {
            ID = "test-3",
            Name = "Test Achievement",
            Description = "This is a test achievement.",
            ColorHex = "#55ff55"
        }));
    }
}
