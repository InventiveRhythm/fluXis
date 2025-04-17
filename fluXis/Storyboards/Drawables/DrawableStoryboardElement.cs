using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Storyboards.Drawables;

public partial class DrawableStoryboardElement : CompositeDrawable
{
    public StoryboardElement Element { get; }

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
        AlwaysPresent = true;

        foreach (var animation in Element.Animations)
        {
            using (BeginAbsoluteSequence(animation.StartTime))
            {
                var duration = Math.Max(animation.Duration, 0);

                switch (animation.Type)
                {
                    case StoryboardAnimationType.MoveX:
                        this.MoveToX(animation.StartFloat).Then()
                            .MoveToX(animation.EndFloat, duration, animation.Easing);
                        break;

                    case StoryboardAnimationType.MoveY:
                        this.MoveToY(animation.StartFloat).Then()
                            .MoveToY(animation.EndFloat, duration, animation.Easing);
                        break;

                    case StoryboardAnimationType.Scale:
                        this.ScaleTo(animation.StartFloat).Then()
                            .ScaleTo(animation.EndFloat, duration, animation.Easing);
                        break;

                    case StoryboardAnimationType.ScaleVector:
                        this.ScaleTo(animation.StartVector).Then()
                            .ScaleTo(animation.EndVector, duration, animation.Easing);
                        break;

                    case StoryboardAnimationType.Width:
                        this.ResizeWidthTo(animation.StartFloat).Then()
                            .ResizeWidthTo(animation.EndFloat, duration, animation.Easing);
                        break;

                    case StoryboardAnimationType.Height:
                        this.ResizeHeightTo(animation.StartFloat).Then()
                            .ResizeHeightTo(animation.EndFloat, duration, animation.Easing);
                        break;

                    case StoryboardAnimationType.Rotate:
                        this.RotateTo(animation.StartFloat).Then()
                            .RotateTo(animation.EndFloat, duration, animation.Easing);
                        break;

                    case StoryboardAnimationType.Fade:
                        this.FadeTo(animation.StartFloat).Then()
                            .FadeTo(animation.EndFloat, duration, animation.Easing);
                        break;

                    case StoryboardAnimationType.Color:
                        var startColour = Colour4.FromHex(animation.ValueStart);
                        var endColour = Colour4.FromHex(animation.ValueEnd);

                        this.FadeColour(startColour).Then()
                            .FadeColour(endColour, duration, animation.Easing);
                        break;
                }
            }
        }
    }
}
