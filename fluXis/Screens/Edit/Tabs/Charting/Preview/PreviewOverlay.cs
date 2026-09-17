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

        var players = currentRuleset.PlayableMode?.Players;

        if (players == null) return;

        List<Modes.Gameplay.Playfield> allPlayfields = [];

        foreach (var player in players)
        {
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (player.MainPlayfield != null) allPlayfields.Add(player.MainPlayfield);
            if (player.SubPlayfields != null) allPlayfields.AddRange(player.SubPlayfields);
            // ReSharper enable ConditionIsAlwaysTrueOrFalse
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
        public readonly Modes.Gameplay.Playfield Playfield;
        private readonly int index;
        private readonly int subIndex;

        public PlayfieldIndex(Modes.Gameplay.Playfield playfield)
        {
            Playfield = playfield;

            // canonical idx
            index = playfield.PlayerIndex + 1;
            subIndex = playfield.PlayfieldIndex + 1;

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
