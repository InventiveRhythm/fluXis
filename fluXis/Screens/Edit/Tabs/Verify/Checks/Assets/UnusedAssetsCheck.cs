using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Database;

namespace fluXis.Screens.Edit.Tabs.Verify.Checks.Assets;

public class UnusedAssetsCheck : IVerifyCheck
{
    public IEnumerable<VerifyIssue> Check(EditorMap map)
    {
        var set = map.MapSet;
        var infos = set.Maps.Select(x => x.GetMapInfo())
                       .Where(x => x != null).ToList();

        var storyboards = infos.DistinctBy(x => x.StoryboardFile)
                               .Select(x => x.GetStoryboard())
                               .Where(x => x != null).ToList();

        var setPath = MapFiles.GetFullPath($"{set.ID}");

        if (!Directory.Exists(setPath))
            yield break;

        var files = Directory.EnumerateFiles(setPath, "*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            if (file.EndsWith(".fsc"))
                continue;

            var relative = file.Replace(setPath, string.Empty).TrimStart(Path.DirectorySeparatorChar).Replace("\\", "/");
            var exists = false;

            if (relative.StartsWith(".vscode"))
            {
                // Ignore .vscode folder
                continue;
            }

            foreach (var info in infos)
            {
                exists |= info.BackgroundFile == relative;
                exists |= info.AudioFile == relative;
                exists |= info.VideoFile == relative;
                exists |= info.EffectFile == relative;
                exists |= info.CoverFile == relative;
                exists |= info.StoryboardFile == relative;

                foreach (var hitObject in info.HitObjects)
                {
                    exists |= hitObject.HitSound == relative;
                }
            }

            foreach (var storyboard in storyboards)
            {
                foreach (var element in storyboard.Elements)
                {
                    var path = element.GetParameter("file", "");
                    exists |= path == relative;
                }
            }

            if (!exists)
            {
                yield return new VerifyIssue(
                    VerifyIssueSeverity.Problematic,
                    VerifyIssueCategory.Assets,
                    null,
                    $"The file \"{relative}\" is not used in any map."
                );
            }
        }
    }
}
