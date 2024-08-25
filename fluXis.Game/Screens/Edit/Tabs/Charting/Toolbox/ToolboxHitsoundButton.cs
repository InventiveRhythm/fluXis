using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Actions;
using fluXis.Game.Screens.Edit.Actions.Notes.Hitsound;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Game.Screens.Edit.Tabs.Shared.Toolbox;
using fluXis.Game.Screens.Gameplay.Audio.Hitsounds;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Toolbox;

public partial class ToolboxHitsoundButton : ToolboxButton
{
    protected override string Text { get; }

    public override LocalisableString TooltipText
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
    private EditorActionStack actions { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorPlayfield playfield { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private ChartingContainer chartingContainer { get; set; }

    private IEnumerable<HitObject> hits => BlueprintContainer.SelectionHandler.SelectedObjects.Where(o => o is HitObject).Cast<HitObject>();

    protected override bool IsSelected
    {
        get
        {
            if (!hits.Any())
                return chartingContainer.CurrentHitSound.Value == sampleFormatted;

            return hits.All(h =>
            {
                if (string.IsNullOrEmpty(h.HitSound))
                    return sample == "normal";

                return h.HitSound == sampleFormatted;
            });
        }
    }

    protected override bool PlayClickSound => false;

    private string sample { get; }
    private string sampleFormatted => $"{Hitsounding.DEFAULT_PREFIX}{sample}";

    public ToolboxHitsoundButton(string display, string sample)
    {
        Text = display;
        this.sample = sample;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        BlueprintContainer.SelectionHandler.SelectedObjects.BindCollectionChanged((_, _) => UpdateSelectionState(), true);
        map.HitSoundsChanged += UpdateSelectionState;

        chartingContainer.CurrentHitSound.BindValueChanged(_ => UpdateSelectionState(), true);
        playfield.HitSoundPlayed += sound =>
        {
            if (sound == sampleFormatted)
                Flash.FadeTo(Flash.Alpha + .2f).FadeOut(clock.BeatTime * 2, Easing.OutQuint);
        };
    }

    public override void Select()
    {
        if (!hits.Any())
        {
            chartingContainer.CurrentHitSound.Value = sampleFormatted;
            return;
        }

        actions.Add(new NoteHitsoundChangeAction(hits.ToArray(), sampleFormatted));
        UpdateSelectionState();
    }

    protected override bool OnClick(ClickEvent e)
    {
        playfield.PlayHitSound(new HitObject { HitSound = sampleFormatted });
        return base.OnClick(e);
    }

    protected override Drawable CreateIcon()
    {
        var icon = sample switch
        {
            "normal" => FontAwesome6.Solid.Drum,
            "drum" => FontAwesome6.Solid.Drum,
            "clap" => FontAwesome6.Solid.HandsClapping,
            _ => FontAwesome6.Solid.Drum
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
