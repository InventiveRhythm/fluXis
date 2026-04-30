using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events.Groups;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.UI.Variable;
using fluXis.Screens.Edit.UI.Variable.Preset;
using fluXis.Utils;
using JetBrains.Annotations;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries.Groups;

public partial class LoopEventEntry : PointListEntry
{
    protected override string Text => "Loop";
    protected override Colour4 Color => Theme.Loop;

    private LoopEvent loop => Object as LoopEvent;

    public LoopEventEntry(LoopEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => loop.JsonCopy();

    protected override Drawable[] CreateValueContent() => new FluXisSpriteText
    {
        Text = $"{loop.TargetGroup} x{loop.Count} {(int)loop.Distance}ms",
        Colour = Color
    }.Yield().ToArray<Drawable>();

    protected override IEnumerable<Drawable> CreateSettings() => base.CreateSettings().Concat([
        new EditorVariableTextBox
        {
            Text = "Target Group",
            TooltipText = "The group to loop.",
            CurrentValue = loop.TargetGroup,
            OnValueChanged = v =>
            {
                loop.TargetGroup = v.Text;
                Map.Update(loop);
            }
        },
        new DistanceInput("Distance", "The time between loops", Map, loop, BeatLength),
        new CountInput(Map, loop, () => loop.Distance)
        {
            Text = "Count",
            TooltipText = "How many times the group should loop.",
            CurrentValue = loop.Count,
            Step = 1,
            Min = 1,
            Max = 128,
            OnValueChanged = v =>
            {
                loop.Count = v;
                Map.Update(loop);
            }
        }
    ]);

    private partial class DistanceInput : EditorVariableBeats<LoopEvent>
    {
        protected override double Value
        {
            get => Object.Distance;
            set => Object.Distance = value;
        }

        public DistanceInput(string text, string tooltip, EditorMap map, LoopEvent obj, float beatLength)
            : base(map, obj, beatLength)
        {
            Text = text;
            TooltipText = tooltip;
            Min = 1 / 16;
        }
    }

    private partial class CountInput : EditorVariableNumber<int>
    {
        [NotNull]
        private readonly EditorMap map;

        [NotNull]
        private readonly LoopEvent obj;

        [NotNull]
        private readonly Func<double> getDistance;

        public CountInput([NotNull] EditorMap map, [NotNull] LoopEvent obj, [NotNull] Func<double> getDistance)
        {
            this.map = map;
            this.obj = obj;
            this.getDistance = getDistance;
        }

        protected override Drawable CreateExtraButton() => new EditorVariableToCurrentButton
        {
            Action = t =>
            {
                double old = obj.Time;
                double diff = t - old;
                int newValue = (int)Math.Max(diff / getDistance(), 0);
                CurrentValue = newValue;
                obj.Count = newValue;
                map.Update(obj);
            }
        };
    }
}
