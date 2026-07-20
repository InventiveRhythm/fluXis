using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Map.Structures;
using fluXis.Screens.Edit.Actions;
using fluXis.Screens.Edit.Actions.Notes.Hitsound;
using fluXis.Screens.Edit.Tabs.Shared.Toolbox;
using fluXis.Screens.Gameplay.Audio.Hitsounds;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;

namespace fluXis.Screens.Edit.Tabs.Charting.Toolbox;

public partial class ToolboxHitsoundButton : ToolboxButton
{
    protected override string Text { get; }

    public override LocalisableString TooltipText
    {
        get
        {
            switch (Sample)
            {
                case "normal":
                    return "Makes this note play the default hitsound.";

                default:
                    return $"Makes this note play the default {Sample} hitsound.";
            }
        }
    }

    [Resolved]
    private EditorActionStack actions { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    private IEnumerable<HitObject> hits => BlueprintContainer.SelectionHandler.SelectedObjects;

    protected override bool IsSelected
    {
        get
        {
            if (!hits.Any())
                return ChartingContainer.CurrentHitSound.Value == sampleFormatted;

            return hits.All(h =>
            {
                if (string.IsNullOrEmpty(h.HitSound))
                    return Sample == "normal";

                return h.HitSound == sampleFormatted;
            });
        }
    }

    protected override bool PlayClickSound => false;

    protected string Sample { get; set; }
    private string sampleFormatted => $"{(included ? Hitsounding.DEFAULT_PREFIX : "")}{Sample}";
    private readonly bool included;

    public ToolboxHitsoundButton(string display, string sample, bool included = false)
    {
        this.included = included;
        Text = display;
        Sample = sample;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        BlueprintContainer.SelectionHandler.SelectedObjects.BindCollectionChanged((_, _) => UpdateSelectionState(), true);
        map.HitSoundsChanged += UpdateSelectionState;

        ChartingContainer.CurrentHitSound.BindValueChanged(_ => UpdateSelectionState(), true);
        ChartingContainer.Playfields.ForEach(p => p.HitSoundPlayed += sound =>
        {
            if (sound == sampleFormatted)
                Flash.FadeTo(Flash.Alpha + .2f).FadeOut(clock.BeatTime * 2, Easing.OutQuint);
        });
    }

    public override void Select()
    {
        if (!hits.Any())
        {
            ChartingContainer.CurrentHitSound.Value = sampleFormatted;
            return;
        }

        actions.Add(new NoteHitsoundChangeAction(hits.ToArray(), sampleFormatted));
        UpdateSelectionState();
    }

    protected override bool OnClick(ClickEvent e)
    {
        PlaySound();
        return base.OnClick(e);
    }

    protected void PlaySound() => ChartingContainer.Playfields.ForEach(p => p.PlayHitSound(new HitObject { HitSound = sampleFormatted }, true));

    protected override Drawable CreateIcon()
    {
        var icon = Sample switch
        {
            "clap" => Phosphor.Bold.HandsClapping,
            _ => FluXisIcon.Get(FluXisIconType.Drum)
        };

        return new FluXisSpriteIcon
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Icon = icon
        };
    }
}
