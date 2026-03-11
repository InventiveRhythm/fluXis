using System;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Outline;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline;
using fluXis.Screens.Edit.UI.Variable;
using fluXis.Screens.Edit.UI.Variable.Preset;
using fluXis.Storyboards;
using fluXis.Utils;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Storyboarding.Animations;

public partial class StoryboardAnimationEntry : CompositeDrawable, IHasPopover
{
    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private StoryboardTimeline timeline { get; set; }

    private float beatLength => map.MapInfo.GetTimingPoint(Animation.StartTime).MsPerBeat;

    private BindableBool isSelected = new(false);

    [CanBeNull]
    public Action<StoryboardAnimation> RequestRemove { get; init; }

    public StoryboardAnimation Animation { get; }
    private readonly StoryboardAnimationRow row;

    private readonly Circle length;
    private readonly OutlinedCircle outlineLength;
    private readonly FluXisSpriteIcon outlineDiamond;

    public StoryboardAnimationEntry(StoryboardAnimation animation, StoryboardAnimationRow row, Colour4 color)
    {
        Animation = animation;
        this.row = row;

        Anchor = Anchor.CentreLeft;
        Origin = Anchor.Centre;

        Size = new Vector2(StoryboardAnimationsList.ROW_HEIGHT);
        InternalChildren = new Drawable[]
        {
            length = new Circle
            {
                RelativeSizeAxes = Axes.Y,
                Height = 0.5f,
                Colour = color,
                Alpha = 0.5f,
                Anchor = Anchor.Centre,
                Origin = Anchor.CentreLeft,
            },
            outlineLength = new OutlinedCircle
            {
                RelativeSizeAxes = Axes.Y,
                Height = 0.5f,
                BorderThickness = 2f,
                Anchor = Anchor.Centre,
                Origin = Anchor.CentreLeft,
                Colour = color.Lighten(2f),
                Alpha = 0
            },
            outlineDiamond =new FluXisSpriteIcon
            {
                Icon = FontAwesome6.Solid.Diamond,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.75f),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = color.Lighten(1.5f),
                Alpha = 0
            },
            new FluXisSpriteIcon
            {
                Icon = FontAwesome6.Solid.Diamond,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.6f),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = color
            },
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        isSelected.BindValueChanged(e =>
        {
            outlineDiamond.Alpha = e.NewValue ? 1f : 0f;
            outlineLength.Alpha = e.NewValue ? 1f : 0f;
        });
    }

    protected override void Update()
    {
        base.Update();

        X = Math.Clamp(timeline.PositionAtTime(Animation.StartTime, Parent!.DrawWidth), -DrawWidth / 2f, Parent.DrawWidth + DrawWidth / 2f);

        var endX = timeline.PositionAtTime(Animation.EndTime, Parent!.DrawWidth);
        var clamped = Math.Max(endX - X, 0);
        length.Width = clamped;
        outlineLength.Width = clamped;
    }

    protected override bool OnClick(ClickEvent e)
    {
        this.ShowPopover();
        isSelected.Value = true;
        return true;
    }

    public Popover GetPopover() => new FluXisPopover
    {
        OnClose = () => isSelected.Value = false,
        Child = new FillFlowContainer
        {
            Width = 380,
            Direction = FillDirection.Vertical,
            AutoSizeAxes = Axes.Y,
            Spacing = new Vector2(12),
            Children = new Drawable[]
            {
                new EditorVariableTitle(Animation.Type.GetDescription(), () => RequestRemove?.Invoke(Animation), false),
                new EditorVariableTime(map, Animation),
                new EditorVariableLength<StoryboardAnimation>(map, Animation, beatLength),
                new EditorVariableTextBox
                {
                    Text = "Start Value",
                    CurrentValue = Animation.ValueStart,
                    OnValueChanged = t =>
                    {
                        if (validate(t.Text, out var parsed)) Animation.ValueStart = parsed;
                        else t.NotifyError();
                        map.Update(Animation);
                    },
                    OnCommit = t =>
                    {
                        if (t is not null && validate(t.Text, out var parsed)) t.Text = parsed;
                        else { t.Text = Animation.ValueStart; t.NotifyError(); }
                    }
                },
                new EditorVariableTextBox
                {
                    Text = "End Value",
                    CurrentValue = Animation.ValueEnd,
                    OnValueChanged = t =>
                    {
                        if (validate(t.Text, out var parsed)) Animation.ValueEnd = parsed;
                        else t.NotifyError();

                        map.Update(Animation);
                    },
                    OnCommit = t =>
                    {
                        if (t is not null && validate(t.Text, out var parsed)) t.Text = parsed;
                        else { t.Text = Animation.ValueEnd; t.NotifyError(); }
                    }
                },
                new EditorVariableEasing<StoryboardAnimation>(map, Animation),
            }
        }
    };

    private Type getAnimPrimitiveType() => Animation.Type switch
    {
        StoryboardAnimationType.MoveX or
        StoryboardAnimationType.MoveY or
        StoryboardAnimationType.Scale or
        StoryboardAnimationType.Width or
        StoryboardAnimationType.Height or
        StoryboardAnimationType.Rotate or
        StoryboardAnimationType.Fade or
        StoryboardAnimationType.Border => typeof(float),
        _ => null
    };

    private bool validate(string input, out string outStr)
    {
        outStr = input;

        var evalType = getAnimPrimitiveType();

        if (evalType is not null)
        {
            if (!input.TryEvaluateTo(evalType, out var result))
                return false;

            outStr = result.ToString();
            return true;
        }

        switch (Animation.Type)
        {
            case StoryboardAnimationType.ScaleVector:
                var split = input.Split(",");
                if (split.Length != 2) return false;

                if (!split[0].TryEvaluateTo(typeof(float), out var x) ||
                    !split[1].TryEvaluateTo(typeof(float), out var y))
                    return false;

                outStr = $"{x},{y}";
                return true;

            case StoryboardAnimationType.Color:
                return Colour4.TryParseHex(input, out _);

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
