using fluXis.Graphics.UserInterface.Panel;
using fluXis.Online.API.Models.Other;
using fluXis.Overlay.Achievements;
using osu.Framework.Allocation;

namespace fluXis.Tests.Overlay;

public partial class TestAchievementOverlay : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        CreateClock();

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
