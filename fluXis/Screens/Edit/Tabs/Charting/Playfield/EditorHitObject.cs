using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield;

public abstract partial class EditorHitObject : CompositeDrawable
{
    [Resolved]
    protected EditorPlayfield Playfield { get; private set; }

    [Resolved]
    protected EditorClock EditorClock { get; private set; }

    [Resolved]
    private EditorSettings settings { get; set; }

    public HitObject Data { get; }

    public virtual bool Visible => Math.Abs(EditorClock.CurrentTime - Data.Time) <= 2000 / settings.Zoom;
    protected virtual Colour4 TextColor => Theme.TextDark;

    private FluXisSpriteText groupText;
    private FluXisSpriteText sampleText;

    private bool overZero = true;
    private const int max_distance = 100;

    protected EditorHitObject(HitObject hit)
    {
        Data = hit;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = EditorHitObjectContainer.NOTEWIDTH;
        AutoSizeAxes = Axes.Y;
        Origin = Anchor.BottomLeft;

        InternalChildren = CreateContent().Concat(new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(-4),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Children =
            [
                groupText = new FluXisSpriteText
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Colour = TextColor,
                    WebFontSize = 12
                },
                sampleText = new FluXisSpriteText
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Colour = TextColor,
                    WebFontSize = 10
                }
            ]
        }.Yield()).ToArray();
    }

    protected abstract IEnumerable<Drawable> CreateContent();

    protected override void Update()
    {
        base.Update();

        groupText.Text = Data.Group;
        sampleText.Text = Data.HitSound?.Replace(".wav", "") ?? ":normal";

        X = Playfield.HitObjectContainer.PositionFromLane(Data.Lane);
        Y = Playfield.HitObjectContainer.PositionAtTime(Data.Time);

        if (Data.Time <= EditorClock.CurrentTime && EditorClock.CurrentTime - Data.Time <= max_distance && overZero)
            Playfield.PlayHitSound(Data);

        overZero = Data.Time > EditorClock.CurrentTime;
    }

    protected override bool OnHover(HoverEvent e) => true;
}
