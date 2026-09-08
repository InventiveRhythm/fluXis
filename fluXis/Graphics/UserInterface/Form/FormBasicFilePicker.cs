using System.IO;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Files;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Utils;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Graphics.UserInterface.Form;

public partial class FormBasicFilePicker : BaseFormComponent<string, FormBasicFilePicker>, IDragDropHandler
{
    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private PanelContainer panels { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    public string[] AllowedExtensions { get; init; } = [];
    public string MapDirectory { get; init; }

    private ForcedHeightText text;
    private bool empty => string.IsNullOrEmpty(Value);

    public FormBasicFilePicker(LocalisableString label, string value)
        : this(label, new Bindable<string> { Value = value })
    {
    }

    public FormBasicFilePicker(LocalisableString label, [NotNull] Bindable<string> bind)
        : base(label, bind)
    {
    }

    protected override Drawable CreateContent() => new FillFlowContainer
    {
        RelativeSizeAxes = Axes.X,
        AutoSizeAxes = Axes.Y,
        Direction = FillDirection.Vertical,
        Spacing = new Vector2(INNER_GAP),
        Padding = new MarginPadding(PADDING),
        Anchor = Anchor.CentreLeft,
        Origin = Anchor.CentreLeft,
        Children =
        [
            new ForcedHeightText(true)
            {
                Text = Label,
                RelativeSizeAxes = Axes.X,
                WebFontSize = 14,
                Height = 16,
                Colour = Theme.TextVariant
            },
            text = new ForcedHeightText(true)
            {
                Text = empty ? "Click to select file..." : Value,
                RelativeSizeAxes = Axes.X,
                WebFontSize = 18,
                Height = 22,
                Colour = empty ? Theme.Foreground : Theme.Text
            }
        ]
    };

    protected override bool OnClick(ClickEvent e)
    {
        if (panels == null)
            return false;

        panels.Add(new FileSelect
        {
            AllowedExtensions = AllowedExtensions,
            MapDirectory = MapDirectory,
            OnFileSelected = setPath
        });

        return true;
    }

    private void setPath(FileInfo file)
    {
        Value = file.FullName;
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
