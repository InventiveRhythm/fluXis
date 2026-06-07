using System.Linq;
using fluXis.Online.API;
using fluXis.Online.API.Models.Clubs;
using fluXis.Online.API.Requests.Clubs;
using fluXis.Overlay.Network.Tabs;
using NUnit.Framework;
using osu.Framework.Allocation;

namespace fluXis.Tests.Network;

public partial class TestDashboardClub : AbstractDashboardTabTest<DashboardClubTab>
{
    [BackgroundDependencyLoader]
    private void load()
    {
        TestAPI.HandleRequest += req =>
        {
            if (req is ClubRequest cr)
            {
                var club = APIClub.CreateUnknown(1);
                club.Members = CreateDummyUsers(12).ToList();
                cr.TriggerSuccess(new APIResponse<APIClub>(200, "", club));
            }
        };
    }

    [Test]
    public void TestWithClub()
    {
        AddStep("add club to user", () => TestAPI.User.Value.Club = APIClub.CreateUnknown(1));
        CreateAddStep();
    }
}
