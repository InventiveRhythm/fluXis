using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Actions.Notes.Hitsound;
using fluXis.Game.Screens.Gameplay;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Toolbox.Hitsound;

public partial class ToolboxHitsoundButton : ToolboxButton
{
    public override string Text { get; }

    public override string Tooltip
    {
        get
        {
            switch (sample)
            {
                case "normal":
                    return "Makes this note play the default hitsound.";

                default:
                    return $"Makes this note play the default {sample} hitsound.";
            }
        }
    }

    [Resolved]
    private EditorValues values { get; set; }

    private IEnumerable<HitObjectInfo> hits => BlueprintContainer.SelectionHandler.SelectedObjects.Where(o => o is HitObjectInfo).Cast<HitObjectInfo>();

    public override bool IsSelected => hits.Any() && hits.All(h =>
    {
        if (string.IsNullOrEmpty(h.HitSound))
            return sample == "normal";

        return h.HitSound == $"{Hitsounding.DEFAULT_PREFIX}{sample}";
    });

    private string sample { get; }

    public ToolboxHitsoundButton(string display, string sample)
    {
        Text = display;
        this.sample = sample;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        BlueprintContainer.SelectionHandler.SelectedObjects.BindCollectionChanged((_, _) => UpdateSelectionState(), true);
        values.MapInfo.HitSoundsChanged += UpdateSelectionState;
    }

    public override void Select()
    {
        values.ActionStack.Add(new NoteHitsoundChangeAction(values.MapInfo, hits.ToArray(), $"{Hitsounding.DEFAULT_PREFIX}{sample}"));

        UpdateSelectionState();
    }

    protected override Drawable CreateIcon()
    {
        var icon = sample switch
        {
            "normal" => FontAwesome.Solid.Drum,
            "drum" => FontAwesome.Solid.Drum,
            "clap" => FontAwesome.Solid.Hands,
            _ => FontAwesome.Solid.Drum
        };

        return new SpriteIcon
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Icon = icon
        };
    }
}
