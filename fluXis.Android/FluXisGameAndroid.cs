using System.IO;
using fluXis.Game;

namespace fluXis.Android;

public partial class FluXisGameAndroid : FluXisGame
{
    protected override void LoadComplete()
    {
        base.LoadComplete();

        var importFolder = Host.Storage.GetFullPath("import");
        if (!Directory.Exists(importFolder)) Directory.CreateDirectory(importFolder);

        var importFiles = Directory.GetFiles(importFolder);

        if (importFiles.Length <= 0) return;

        HandleDragDrop(importFiles);
    }
}
