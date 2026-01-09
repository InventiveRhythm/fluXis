using System;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline;
using fluXis.Storyboards;
using JetBrains.Annotations;
using osu.Framework.Allocation;
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

    [CanBeNull]
    public Action<StoryboardAnimation> RequestRemove { get; init; }

    public StoryboardAnimation Animation { get; }
    private readonly StoryboardAnimationRow row;

    private readonly Circle length;

    public StoryboardAnimationEntry(StoryboardAnimation animation, StoryboardAnimationRow row, Colour4 color)
    {
        Animation = animation;
        this.row = row;

        Anchor = Anchor.CentreLeft;
        Origin = Anchor.Centre;

        Size = new Vector2(StoryboardAnimationsList.ROW_HEIGHT);
        InternalChildren = new Drawable[]
        {
            new FluXisSpriteIcon
            {
                Icon = FontAwesome6.Solid.Diamond,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.6f),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = color
            },
            length = new Circle
            {
                RelativeSizeAxes = Axes.Y,
                Height = 0.5f,
                Colour = color,
                Alpha = 0.5f,
                Anchor = Anchor.Centre,
                Origin = Anchor.CentreLeft,
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        X = Math.Clamp(timeline.PositionAtTime(Animation.StartTime, Parent!.DrawWidth), -DrawWidth / 2f, Parent.DrawWidth + DrawWidth / 2f);

        var endX = timeline.PositionAtTime(Animation.EndTime, Parent!.DrawWidth);
        length.Width = Math.Max(endX - X, 0);
    }

    protected override bool OnClick(ClickEvent e)
    {
        this.ShowPopover();
        return true;
    }

    public Popover GetPopover() => new FluXisPopover
    {
        Child = new FillFlowContainer
        {
            Width = 380,
            Direction = FillDirection.Vertical,
            AutoSizeAxes = Axes.Y,
            Spacing = new Vector2(12),
            Children = new Drawable[]
            {
                new PointSettingsTitle(Animation.Type.GetDescription(), () => RequestRemove?.Invoke(Animation), false),
                new PointSettingsTime(map, Animation),
                new PointSettingsLength<StoryboardAnimation>(map, Animation, beatLength),
                new PointSettingsTextBox
                {
                    Text = "Start Value",
                    DefaultText = Animation.ValueStart,
                    OnTextChanged = t =>
                    {
                        Animation.ValueStart = t.Text;
                        map.Update(Animation);
                    }
                },
                new PointSettingsTextBox
                {
                    Text = "End Value",
                    DefaultText = Animation.ValueEnd,
                    OnTextChanged = t =>
                    {
                        Animation.ValueEnd = t.Text;
                        map.Update(Animation);
                    }
                },
                new PointSettingsEasing<StoryboardAnimation>(map, Animation),
            }
        }
    };
}
