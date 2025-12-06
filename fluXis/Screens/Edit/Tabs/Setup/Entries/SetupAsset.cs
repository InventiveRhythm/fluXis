using System;
using System.IO;
using fluXis.Database;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Files;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Utils;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Edit.Tabs.Setup.Entries;

public partial class SetupAsset : SetupEntry, IDragDropHandler
{
    protected override float ContentSpacing => -2;
    protected override bool ShowHoverFlash => true;

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private PanelContainer panels { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    public string[] AllowedExtensions { get; init; } = Array.Empty<string>();

    public Action<FileInfo> OnChange { get; init; } = _ => { };

    private TruncatingText text;

    private bool empty => string.IsNullOrEmpty(path);
    private string path;

    public SetupAsset(string title, string path = "")
        : base(title)
    {
        this.path = path;
    }

    protected override Drawable CreateContent() => text = new TruncatingText
    {
        RelativeSizeAxes = Axes.X,
        Text = empty ? "Click to select file..." : path,
        WebFontSize = 18,
        Colour = empty ? Theme.Foreground : Theme.Text
    };

    protected override bool OnClick(ClickEvent e)
    {
        if (panels == null)
            return false;

        panels.Content = new FileSelect
        {
            AllowedExtensions = AllowedExtensions,
            MapDirectory = MapFiles.GetFullPath($"{map.RealmMap.MapSet.ID}"),
            OnFileSelected = setPath
        };

        return true;
    }

    private void setPath(FileInfo file)
    {
        path = file.Name;
        OnChange.Invoke(file);

        text.Colour = Theme.Text;
        text.Text = file.Name;
    }

    public bool OnDragDrop(string file)
    {
        var info = new FileInfo(file);
        setPath(info);
        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        Schedule(() => game.AddDragDropHandler(this));
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e) => game.RemoveDragDropHandler(this);
}
