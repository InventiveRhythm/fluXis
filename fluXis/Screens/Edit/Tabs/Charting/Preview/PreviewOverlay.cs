using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Screens.Gameplay.Ruleset;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Edit.Tabs.Charting.Preview;

public partial class PreviewOverlay : Container
{
    public required ChartingPreview Preview { get; init; }

    private Bindable<bool> showPlayfieldIndices = new();

    private RulesetContainer currentRuleset;

    [BackgroundDependencyLoader]
    private void load(EditorSettings settings)
    {
        showPlayfieldIndices = settings.ShowPlayfieldIndices;

        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        showPlayfieldIndices.BindValueChanged(val => Alpha = val.NewValue ? 1 : 0, true);
    }

    protected override void Update()
    {
        base.Update();

        if (!showPlayfieldIndices.Value) return;

        var newRuleset = Preview.Ruleset;

        if (newRuleset == null)
        {
            if (currentRuleset == null) return;

            currentRuleset = null;
            Clear();

            return;
        }

        if (currentRuleset != newRuleset)
        {
            currentRuleset = newRuleset;
            Clear();
        }

        var players = currentRuleset.PlayfieldManager?.Players;

        if (players == null) return;

        List<Gameplay.Ruleset.Playfields.Playfield> allPlayfields = [];

        foreach (var player in players)
        {
            if (player.MainPlayfield != null) allPlayfields.Add(player.MainPlayfield);
            if (player.SubPlayfields != null) allPlayfields.AddRange(player.SubPlayfields);
        }

        if (Children.Count != allPlayfields.Count)
        {
            Clear();
            AddRange(allPlayfields.Select(p => new PlayfieldIndex(p)));
        }

        foreach (var child in Children)
        {
            if (child is PlayfieldIndex idx)
            {
                idx.X = idx.Playfield.X;
                idx.Y = idx.Playfield.Y;
                idx.Alpha = idx.Playfield.Alpha;
            }
        }
    }

    private partial class PlayfieldIndex : CompositeDrawable
    {
        public readonly Gameplay.Ruleset.Playfields.Playfield Playfield;
        private readonly int index;
        private readonly int subIndex;

        public PlayfieldIndex(Gameplay.Ruleset.Playfields.Playfield playfield)
        {
            Playfield = playfield;

            // canonical idx
            index = playfield.Index + 1;
            subIndex = playfield.SubIndex + 1;

            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            AutoSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children =
                [
                    new FluXisSpriteText
                    {
                        Text = subIndex.ToString(),
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        FontSize = 96
                    },
                    new FluXisSpriteText
                    {
                        Text = index.ToString(),
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        FontSize = 64
                    },
                ]
            };
        }
    }
}
