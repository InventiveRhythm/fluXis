using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Database;
using fluXis.Map;
using fluXis.Storyboards;
using fluXis.Storyboards.Drawables;
using fluXis.Utils;

namespace fluXis.Screens.Edit.Tabs.Verify.Checks.Assets;

public class UnusedAssetsCheck : IVerifyCheck
{
    public IEnumerable<VerifyIssue> Check(IVerifyContext ctx)
    {
        var set = ctx.MapSet;
        var infos = set.Maps.Select(x => x.GetMapInfo())
                       .Where(x => x != null).ToList();

        var storyboards = infos
                          .Select<MapInfo, (MapInfo map, Storyboard storyboard)>(x => (x, x.GetStoryboard()))
                          .Where(x => x.storyboard != null).ToList();

        var storyboardFiles = new HashSet<string>();

        foreach (var sb in storyboards)
        {
            var root = set.GetPathForFile("");
            if (!Path.IsPathRooted(root)) root = MapFiles.GetFullPath(root);
            root = Path.TrimEndingDirectorySeparator(root);

            var storyboard = sb.storyboard.JsonCopy();
            var draw = new DrawableStoryboard(sb.map, storyboard, root);
            ctx.LoadComponent(draw);

            foreach (var element in storyboard.Elements)
            {
                var path = "";

                switch (element.Type)
                {
                    case StoryboardElementType.Sprite:
                        path = element.GetParameter("file", "");
                        break;

                    case StoryboardElementType.Script:
                        path = element.GetParameter("path", "");
                        break;
                }

                if (string.IsNullOrWhiteSpace(path))
                    continue;

                storyboardFiles.Add(path);
            }

            draw.Dispose();
        }

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

            // Ignore .vscode folder for lua scripting
            if (relative.StartsWith(".vscode"))
                continue;

            foreach (var info in infos)
            {
                exists |= info.BackgroundFile == relative;
                exists |= info.AudioFile == relative;
                exists |= info.VideoFile == relative;
                exists |= info.EffectFile == relative;
                exists |= info.CoverFile == relative;
                exists |= info.StoryboardFile == relative;

                foreach (var hitObject in info.HitObjects)
                    exists |= hitObject.HitSound == relative;
            }

            exists |= storyboardFiles.Any(x => string.Equals(Path.ChangeExtension(x, null), Path.ChangeExtension(relative, null), StringComparison.OrdinalIgnoreCase));

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
