using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;

namespace fluXis.Game.Storyboards.Drawables;

public partial class DrawableStoryboardElement : CompositeDrawable
{
    public StoryboardElement Element { get; }

    private List<ElementAction> actions { get; } = new();

    public DrawableStoryboardElement(StoryboardElement element)
    {
        Element = element;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Element.Anchor;
        Origin = Element.Origin;
        X = Element.StartX;
        Y = Element.StartY;
        Width = Element.Width;
        Height = Element.Height;
        Colour = Colour4.FromRGBA(Element.Color);

        foreach (var animation in Element.Animations)
        {
            actions.Add(new ElementAction
            {
                Time = animation.StartTime,
                Action = () =>
                {
                    switch (animation.Type)
                    {
                        case StoryboardAnimationType.MoveX:
                            this.MoveToX(animation.StartFloat);
                            this.MoveToX(animation.EndFloat, animation.Duration, animation.Easing);
                            break;

                        case StoryboardAnimationType.MoveY:
                            this.MoveToY(animation.StartFloat);
                            this.MoveToY(animation.EndFloat, animation.Duration, animation.Easing);
                            break;

                        case StoryboardAnimationType.Scale:
                            this.ScaleTo(animation.StartFloat);
                            this.ScaleTo(animation.EndFloat, animation.Duration, animation.Easing);
                            break;

                        case StoryboardAnimationType.ScaleVector:
                            this.ScaleTo(animation.StartVector);
                            this.ScaleTo(animation.EndVector, animation.Duration, animation.Easing);
                            break;

                        case StoryboardAnimationType.Width:
                            this.ResizeWidthTo(animation.StartFloat);
                            this.ResizeWidthTo(animation.EndFloat, animation.Duration, animation.Easing);
                            break;

                        case StoryboardAnimationType.Height:
                            this.ResizeHeightTo(animation.StartFloat);
                            this.ResizeHeightTo(animation.EndFloat, animation.Duration, animation.Easing);
                            break;

                        case StoryboardAnimationType.Rotate:
                            this.RotateTo(animation.StartFloat);
                            this.RotateTo(animation.EndFloat, animation.Duration, animation.Easing);
                            break;

                        case StoryboardAnimationType.Fade:
                            if (animation.StartFloat == 0)
                            {
                                this.FadeTo(0.001f); // 0 will just make it not update
                                this.FadeTo(animation.EndFloat, animation.Duration, animation.Easing);
                            }
                            else
                            {
                                this.FadeTo(animation.StartFloat);
                                this.FadeTo(animation.EndFloat, animation.Duration, animation.Easing);
                            }

                            break;

                        case StoryboardAnimationType.Color:
                            var startColour = Colour4.FromHex(animation.ValueStart);
                            var endColour = Colour4.FromHex(animation.ValueEnd);

                            this.FadeColour(startColour);
                            this.FadeColour(endColour, animation.Duration, animation.Easing);
                            break;
                    }
                }
            });
        }
    }

    protected override void Update()
    {
        base.Update();

        while (actions.Count > 0 && actions.First().Time <= Time.Current)
        {
            var action = actions.First();
            Logger.LogPrint($"Action at {action.Time} is being executed at {Time.Current}");
            action.Action?.Invoke();
            actions.Remove(action);
        }
    }

    private class ElementAction
    {
        public double Time { get; init; }
        public Action Action { get; init; }
    }
}
