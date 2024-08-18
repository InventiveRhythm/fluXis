using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Storyboards.Drawables;

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
                switch (animation.Type)
                {
                    case StoryboardAnimationType.MoveX:
                        this.MoveToX(animation.StartFloat).Then()
                            .MoveToX(animation.EndFloat, animation.Duration, animation.Easing);
                        break;

                    case StoryboardAnimationType.MoveY:
                        this.MoveToY(animation.StartFloat).Then()
                            .MoveToY(animation.EndFloat, animation.Duration, animation.Easing);
                        break;

                    case StoryboardAnimationType.Scale:
                        this.ScaleTo(animation.StartFloat).Then()
                            .ScaleTo(animation.EndFloat, animation.Duration, animation.Easing);
                        break;

                    case StoryboardAnimationType.ScaleVector:
                        this.ScaleTo(animation.StartVector).Then()
                            .ScaleTo(animation.EndVector, animation.Duration, animation.Easing);
                        break;

                    case StoryboardAnimationType.Width:
                        this.ResizeWidthTo(animation.StartFloat).Then()
                            .ResizeWidthTo(animation.EndFloat, animation.Duration, animation.Easing);
                        break;

                    case StoryboardAnimationType.Height:
                        this.ResizeHeightTo(animation.StartFloat).Then()
                            .ResizeHeightTo(animation.EndFloat, animation.Duration, animation.Easing);
                        break;

                    case StoryboardAnimationType.Rotate:
                        this.RotateTo(animation.StartFloat).Then()
                            .RotateTo(animation.EndFloat, animation.Duration, animation.Easing);
                        break;

                    case StoryboardAnimationType.Fade:
                        this.FadeTo(animation.StartFloat).Then()
                            .FadeTo(animation.EndFloat, animation.Duration, animation.Easing);

                        break;

                    case StoryboardAnimationType.Color:
                        var startColour = Colour4.FromHex(animation.ValueStart);
                        var endColour = Colour4.FromHex(animation.ValueEnd);

                        this.FadeColour(startColour).Then()
                            .FadeColour(endColour, animation.Duration, animation.Easing);
                        break;
                }
            }
        }
    }
}
