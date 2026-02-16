using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osuTK;

namespace fluXis.Graphics.Background;

public partial class StripeBackground : Drawable
{
    public float Angle;
    public float Thickness = 24;
    public float Gap = 10;

    public ColourInfo BackgroundColor = Colour4.Transparent;
    public ColourInfo StripesColor = Colour4.White;

    public StripeBackground()
    {
        RelativeSizeAxes = Axes.Both;
    }

    protected override DrawNode CreateDrawNode() => new StripeDrawNode(this);

    private class StripeDrawNode : DrawNode
    {
        protected new StripeBackground Source => (StripeBackground)base.Source;
        private float angleRad;
        private float thickness;
        private float gap;

        private ColourInfo backgroundColor = Colour4.Transparent;
        private ColourInfo stripesColor = Colour4.White;

        public StripeDrawNode(StripeBackground source) : base(source)
        {
        }

        public override void ApplyState()
        {
            base.ApplyState();
            thickness = Source.Thickness;
            gap = Source.Gap;
            angleRad = Source.Angle * (MathF.PI / 180.0f);
            backgroundColor = Source.BackgroundColor;
            stripesColor = Source.StripesColor;
        }

        protected override void Draw(IRenderer renderer)
        {
            base.Draw(renderer);

            var texture = renderer.WhitePixel;
            
            var bgQuad = new Quad(
                Vector2Extensions.Transform(new Vector2(0, 0), DrawInfo.Matrix),
                Vector2Extensions.Transform(new Vector2(Source.DrawWidth, 0), DrawInfo.Matrix),
                Vector2Extensions.Transform(new Vector2(0, Source.DrawHeight), DrawInfo.Matrix),
                Vector2Extensions.Transform(new Vector2(Source.DrawWidth, Source.DrawHeight), DrawInfo.Matrix)
            );
            
            renderer.DrawQuad(texture, bgQuad, backgroundColor);

            float dirX = MathF.Cos(angleRad);
            float dirY = MathF.Sin(angleRad);
            
            float perpX = -dirY;
            float perpY = dirX;

            float w = Source.DrawWidth;
            float h = Source.DrawHeight;
            
            float diagonal = MathF.Sqrt(w * w + h * h);
            int stripes = (int)Math.Ceiling(diagonal * 2 / thickness);

            for (int i = -stripes/2; i < stripes; i++)
            {
                float offset = i * (thickness + gap);
                
                float startX = w/2 + offset * dirX - diagonal * perpX;
                float startY = h/2 + offset * dirY - diagonal * perpY;
                
                float endX = startX + 2 * diagonal * perpX;
                float endY = startY + 2 * diagonal * perpY;

                var sQuad = new Quad(
                    Vector2Extensions.Transform(new Vector2(startX, startY), DrawInfo.Matrix),
                    Vector2Extensions.Transform(new Vector2(startX + thickness * dirX, startY + thickness * dirY), DrawInfo.Matrix),
                    Vector2Extensions.Transform(new Vector2(endX + thickness * dirX, endY + thickness * dirY), DrawInfo.Matrix),
                    Vector2Extensions.Transform(new Vector2(endX, endY), DrawInfo.Matrix)
                );
                
                renderer.DrawQuad(texture, sQuad, stripesColor);
            }
        }
    }
}
