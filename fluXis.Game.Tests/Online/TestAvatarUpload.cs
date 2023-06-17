using System.IO;
using System.Net.Http;
using fluXis.Game.Configuration;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Import;
using osu.Framework.Allocation;
using osu.Framework.IO.Network;
using osu.Framework.Logging;

namespace fluXis.Game.Tests.Online;

public partial class TestAvatarUpload : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        var stack = new FluXisScreenStack();
        Add(stack);

        const string url = "http://localhost:2434/assets/avatar/1";

        AddStep("Show file import screen", () => stack.Push(new FileImportScreen
        {
            AllowedExtensions = new[] { ".png" },
            OnFileSelected = file =>
            {
                Logger.Log($"File selected: {file.Name}");

                byte[] data = File.ReadAllBytes(file.FullName);

                var request = new WebRequest("http://localhost:2434/account/update/avatar");
                request.AllowInsecureRequests = true;
                request.AddHeader("Authorization", $"{config.Get<string>(FluXisSetting.Token)}");
                request.AddFile("file", data);
                request.Method = HttpMethod.Post;
                request.Perform();
                Logger.Log($"Response: {request.GetResponseString()}");
            }
        }));
    }
}
