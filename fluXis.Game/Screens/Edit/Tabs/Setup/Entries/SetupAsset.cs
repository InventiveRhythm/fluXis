using System;
using System.IO;
using fluXis.Game.Database;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Files;
using fluXis.Game.Graphics.UserInterface.Panel;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Edit.Tabs.Setup.Entries;

public partial class SetupAsset : SetupEntry
{
    protected override float ContentSpacing => -3;

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private PanelContainer panels { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    public string[] AllowedExtensions { get; init; } = Array.Empty<string>();
    public Action<FileInfo> OnChange { get; init; } = _ => { };

    public string Path
    {
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                value = "Click to select file...";
                empty = true;
            }

            path = value;

            if (text != null)
            {
                text.Text = path;
                text.Alpha = empty ? .6f : 1;
            }
        }
    }

    private TruncatingText text;
    private string path = string.Empty;
    private bool empty;

    public SetupAsset(string title)
        : base(title)
    {
    }

    protected override Drawable CreateContent()
    {
        return text = new TruncatingText
        {
            RelativeSizeAxes = Axes.X,
            Text = path,
            WebFontSize = 18,
            Alpha = empty ? .6f : 1
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (panels == null)
            return false;

        panels.Content = new FileSelect
        {
            AllowedExtensions = AllowedExtensions,
            MapDirectory = MapFiles.GetFullPath($"{map.RealmMap.MapSet.ID}"),
            OnFileSelected = file =>
            {
                Path = file.Name;
                OnChange.Invoke(file);
            }
        };

        return true;
    }
}
