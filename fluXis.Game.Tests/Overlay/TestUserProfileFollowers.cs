using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Overlay.User.Sidebar;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Tests.Overlay;

public partial class TestUserProfileFollowers : FluXisTestScene
{
    private ProfileFollowerList list;

    [SetUp]
    public void Setup() => Schedule(() =>
    {
        Child = new Container
        {
            Width = 300,
            AutoSizeAxes = Axes.Y,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Child = list = new ProfileFollowerList(-1)
        };
    });

    private List<APIUser> users(int num)
        => Enumerable.Range(1, num).Select(x => new APIUser { ID = 1, Username = $"User{x}" }).ToList();

    [Test]
    public void TestNoFollowers()
    {
        AddStep("set no followers", () => list.SetData(new List<APIUser>()));
    }

    [Test]
    public void TestThreeFollowers()
    {
        AddStep("set 3 followers", () => list.SetData(users(3)));
    }

    [Test]
    public void TestTenFollowers()
    {
        AddStep("set 10 followers", () => list.SetData(users(10)));
    }

    [Test]
    public void TestThousandFollowers()
    {
        AddStep("set 1,000 followers", () => list.SetData(users(1000)));
    }

    [Test]
    public void TestMillionFollowers()
    {
        AddStep("set 1,000,000 followers", () => list.SetData(users(1000000)));
    }
}
