using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

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

    public virtual bool Visible => Math.Abs(EditorClock.CurrentTime - Data.Time) <= 2000;

    private FluXisSpriteText text { get; set; }

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

        InternalChildren = CreateContent().Concat(new Drawable[]
        {
            new Container
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.X,
                Height = Data.Type == 1 ? 20 : 36,
                Child = text = new FluXisSpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = Theme.Background2,
                    Alpha = 0
                }
            }
        }).ToArray();
    }

    protected abstract IEnumerable<Drawable> CreateContent();

    protected override void Update()
    {
        base.Update();

        text.Text = Data.HitSound?.Replace(".wav", "") ?? "";
        text.Alpha = settings.ShowSamples.Value ? 1 : 0;

        X = Playfield.HitObjectContainer.PositionFromLane(Data.Lane);
        Y = Playfield.HitObjectContainer.PositionAtTime(Data.Time);

        if (Data.Time <= EditorClock.CurrentTime && EditorClock.CurrentTime - Data.Time <= max_distance && overZero)
            Playfield.PlayHitSound(Data);

        overZero = Data.Time > EditorClock.CurrentTime;
    }

    protected override bool OnHover(HoverEvent e) => true;
}
