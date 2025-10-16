using System;
using System.Linq;
using fluXis.Graphics.Background;
using fluXis.Map;
using fluXis.Screens;
using fluXis.Screens.Course;
using osu.Framework.Allocation;

namespace fluXis.Tests.Courses;

public partial class TestCourseScreen : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(MapStore maps)
    {
        CreateClock();

        var stack = new FluXisScreenStack();
        TestDependencies.Cache(stack);
        TestDependencies.CacheAs(new GlobalBackground());
        Add(stack);

        LoadComponent(GlobalClock);

        var queue = new[]
        {
            maps.GetMapFromGuid(Guid.Parse("3b55b380-e533-4eea-bf16-4b98d9776583")),
            maps.GetMapFromGuid(Guid.Parse("f2ea5b0a-7ee4-4839-b0af-ea379c657a26")),
            maps.GetMapFromGuid(Guid.Parse("9e03c3de-a694-43e4-b0a4-dcfb3f97239d"))
        };

        AddStep("Push Screen", () => stack.Push(new CourseScreen(queue.ToList())));
    }
}
