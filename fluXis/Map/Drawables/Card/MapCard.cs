using System;
using fluXis.Graphics;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API.Models.Maps;
using fluXis.Overlay.Navigator;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Map.Drawables.Card;

public partial class MapCard : CompositeDrawable
{
    public const int CARD_HEIGHT = 128;
    public const int CARD_RADIUS = 12;

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private OnlineNavigator navigator { get; set; }

    public APIMapSet MapSet { get; }

    public Func<bool> ClickAction { get; init; } = () => false;
    public Action<bool> ExpandAction { get; init; }

    private readonly Container scaling;
    private readonly Expand expand;

    public MapCard(APIMapSet set)
    {
        MapSet = set;
        Size = new Vector2(410, CARD_HEIGHT);

        InternalChild = scaling = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Children = [expand = new Expand(set, TriggerClick), new Header(set)]
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        expand.Expanded.BindValueChanged(v =>
        {
            ExpandAction?.Invoke(v.NewValue);
            scaling.ScaleTo(v.NewValue ? 1.04f : 1f, Styling.TRANSITION_MOVE, Easing.OutQuint);
            expand.ResizeHeightTo(v.NewValue ? CARD_HEIGHT + expand.ContentHeight : CARD_HEIGHT, Styling.TRANSITION_MOVE, Easing.OutQuint);
            expand.FadeEdgeEffectTo(v.NewValue ? Styling.SHADOW_OPACITY : 0f, Styling.TRANSITION_FADE);
        }, true);
        FinishTransforms(true);
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (ClickAction())
            return true;

        navigator?.PushMapSet(MapSet.ID);
        return true;
    }

    private static RoundedChip createChip(string text, ColourInfo color)
    {
        var light = Theme.IsBright(color.AverageColour);

        return new RoundedChip
        {
            Text = text,
            TextColour = (light ? Colour4.Black : Colour4.White).Opacity(.75f),
            BackgroundColour = color,
            WebFontSize = 10,
            Height = 16
        };
    }
}
