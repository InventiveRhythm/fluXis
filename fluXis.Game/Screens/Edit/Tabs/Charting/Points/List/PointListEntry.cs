using System;
using System.Collections.Generic;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Charting.Points.Settings;
using fluXis.Game.Screens.Edit.Tabs.Charting.Points.Settings.Preset;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Points.List;

public partial class PointListEntry : Container
{
    protected virtual string Text => "Timed Point";
    protected virtual Colour4 Color => Colour4.White;

    public Action<IEnumerable<Drawable>> ShowSettings { get; set; }
    public Action RequestClose { get; set; }
    public ITimedObject Object { get; }

    [Resolved]
    protected EditorValues Values { get; private set; }

    [Resolved]
    private UISamples samples { get; set; }

    private Box hover;
    private FluXisSpriteText timeText;
    private FluXisSpriteText valueText;

    protected PointListEntry(ITimedObject obj)
    {
        Object = obj;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 32;
        Masking = true;
        CornerRadius = 5;
        Colour = Color;

        InternalChildren = new Drawable[]
        {
            hover = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            timeText = new FluXisSpriteText
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                X = 10
            },
            valueText = new FluXisSpriteText
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                X = -10
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        UpdateValues();
    }

    protected virtual string CreateValueText() => "";

    public void OpenSettings()
    {
        ShowSettings?.Invoke(CreateSettings());
    }

    protected virtual IEnumerable<Drawable> CreateSettings()
    {
        return new Drawable[]
        {
            new PointSettingsTitle(Text, () =>
            {
                Values.MapInfo.Remove(Object);
                RequestClose?.Invoke();
            }),
            new PointSettingsTime(Values.MapInfo, Object)
        };
    }

    public void UpdateValues()
    {
        timeText.Text = TimeUtils.Format(Object.Time);
        valueText.Text = CreateValueText();
    }

    protected override bool OnHover(HoverEvent e)
    {
        samples.Hover();
        hover.FadeTo(.2f, 50);
        return false;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeOut(200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        samples.Click();
        OpenSettings();
        return true;
    }
}
