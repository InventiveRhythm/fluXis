using System;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline;
using fluXis.Storyboards;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
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

    private float beatLength => map.MapInfo.GetTimingPoint(animation.StartTime).MsPerBeat;

    private readonly StoryboardAnimation animation;
    private readonly StoryboardAnimationRow row;

    public StoryboardAnimationEntry(StoryboardAnimation animation, StoryboardAnimationRow row, Colour4 color)
    {
        this.animation = animation;
        this.row = row;

        Anchor = Anchor.CentreLeft;
        Origin = Anchor.Centre;

        Size = new Vector2(StoryboardAnimationsList.ROW_HEIGHT);
        InternalChild = new FluXisSpriteIcon
        {
            Icon = FontAwesome6.Solid.Diamond,
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(0.6f),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Colour = color
        };
    }

    protected override void Update()
    {
        base.Update();

        X = Math.Clamp(timeline.PositionAtTime(animation.StartTime, Parent!.DrawWidth), -DrawWidth / 2f, Parent.DrawWidth + DrawWidth / 2f);
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
                new PointSettingsTime(map, animation),
                new PointSettingsLength<StoryboardAnimation>(map, animation, beatLength),
                new PointSettingsTextBox
                {
                    Text = "Start Value",
                    DefaultText = animation.ValueStart,
                    OnTextChanged = t =>
                    {
                        animation.ValueStart = t.Text;
                        map.Update(animation);
                    }
                },
                new PointSettingsTextBox
                {
                    Text = "End Value",
                    DefaultText = animation.ValueEnd,
                    OnTextChanged = t =>
                    {
                        animation.ValueEnd = t.Text;
                        map.Update(animation);
                    }
                },
                new PointSettingsEasing<StoryboardAnimation>(map, animation),
            }
        }
    };
}
