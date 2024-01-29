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

        AddStep("Show Level 1", () => panels.Content = new AchievementOverlay(new Achievement
        {
            ID = "test-1",
            Level = 1,
            Name = "Test Achievement",
            Description = "This is a test achievement."
        }));

        AddStep("Show Level 2", () => panels.Content = new AchievementOverlay(new Achievement
        {
            ID = "test-2",
            Level = 2,
            Name = "Test Achievement",
            Description = "This is a test achievement."
        }));

        AddStep("Show Level 3", () => panels.Content = new AchievementOverlay(new Achievement
        {
            ID = "test-3",
            Level = 3,
            Name = "Test Achievement",
            Description = "This is a test achievement."
        }));
    }
}
