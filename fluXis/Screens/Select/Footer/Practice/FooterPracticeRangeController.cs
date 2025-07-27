using System;
using fluXis.Database.Maps;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Screens.Select.Footer.Practice;

public partial class FooterPracticeRangeController : Container
{
    [Resolved]
    private MapStore maps { get; set; }

    private RangePoint startPoint;
    private RangePoint endPoint;
    
    private BindableNumber<int> start { get; }
    private BindableNumber<int> end { get; }

    private int endTime = 1;

    public FooterPracticeRangeController(BindableNumber<int> start, BindableNumber<int> end)
    {
        this.start = start;
        this.end = end;
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomLeft;

        InternalChildren = new Drawable[]
        {
            startPoint = new RangePoint(this, start, true)
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreRight,
            },
            endPoint = new RangePoint(this, end, false)
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
            }
        };

        startPoint.SetOtherPoint(endPoint);
        endPoint.SetOtherPoint(startPoint);

        startPoint.OnDragBind = e =>
        {
            float timeMs = posToTime(startPoint.X - 5);
            start.Value = (int)(timeMs / 1000f);
        };

        endPoint.OnDragBind = e =>
        {
            float timeMs = posToTime(endPoint.X + 10);
            end.Value = (int)(timeMs / 1000f);
        };
        
        start.BindValueChanged(v => startPoint.UpdatePosition(timeToPos(v.NewValue * 1000) + 10));
        end.BindValueChanged(v =>
        {
            if (v.NewValue * 1000 >= endTime)
            {
                endPoint.UpdatePosition(DrawWidth - 10);
            }
            else
            {
                endPoint.UpdatePosition(timeToPos(v.NewValue * 1000) - 5);
            }
        });
    }

    protected override void LoadComplete()
    {
        maps.MapBindable.BindValueChanged(mapChanged, true);

        startPoint.X = 10;
        endPoint.X = DrawWidth - 15;
    }
    
    private void mapChanged(ValueChangedEvent<RealmMap> v)
    {
        startPoint.X = 15;
        endPoint.X = DrawWidth - 15;

        var info = v.NewValue.GetMapInfo();
        endTime = (int)info.EndTime;
    }

    private float timeToPos(float time)
    {
        return (time / endTime) * DrawWidth;
    }

    private float posToTime(float pos)
    {
        return (pos / DrawWidth) * endTime;
    }

    private partial class RangePoint : CompositeDrawable
    {
        private Sample dragSample;
        private double lastSampleTime = 0;
        private const int sample_interval = 50;

        private const float line_width = 3f;
        private const float triangle_size = 15f;
        private const float drag_area_width = 30f;

        private readonly FooterPracticeRangeController parent;
        private readonly BindableNumber<int> bindableValue;
        private readonly bool isStartPoint;
        private RangePoint otherPoint;
        private bool isDragging;
        public Action<DragEvent> OnDragBind;

        private Box dragArea;
        private Box line;
        private FluXisSpriteIcon topTriangle;
        private FluXisSpriteIcon bottomTriangle;

        public RangePoint(FooterPracticeRangeController parent, BindableNumber<int> bindableValue, bool isStartPoint)
        {
            this.parent = parent;
            this.bindableValue = bindableValue;
            this.isStartPoint = isStartPoint;
            
            RelativeSizeAxes = Axes.Y;
            Width = drag_area_width;

            InternalChildren = new Drawable[]
            {
                dragArea = new Box
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0f,
                    Colour = Color4.Transparent
                },

                line = new Box
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Y,
                    Width = line_width,
                    Colour = Color4.White
                },

                topTriangle = new FluXisSpriteIcon
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.BottomCentre,
                    Icon = FontAwesome.Solid.CaretDown,
                    Size = new Vector2(triangle_size),
                    Colour = Color4.White,
                },

                bottomTriangle = new FluXisSpriteIcon
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.TopCentre,
                    Icon = FontAwesome.Solid.CaretUp,
                    Size = new Vector2(triangle_size),
                    Colour = Color4.White
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(ISampleStore samples)
        {
            dragSample = samples.Get("UI/slider-tick");
        }

        public void SetOtherPoint(RangePoint other)
        {
            otherPoint = other;
        }

        public void UpdatePosition(float newX)
        {
            if (!isDragging)
                X = newX;
        }

        protected override bool OnDragStart(DragStartEvent e)
        {
            isDragging = true;
            return true;
        }

        protected override void OnDrag(DragEvent e)
        {
            float newX = X + e.Delta.X;

            if (Parent != null)
            {
                newX = Math.Max(10, Math.Min(Parent.DrawWidth - 10, newX));

                if (otherPoint != null)
                {
                    if (isStartPoint)
                    {
                        newX = Math.Min(newX, Math.Abs(otherPoint.X + dragArea.DrawWidth) - 15);
                    }
                    else
                    {
                        newX = Math.Max(newX, Math.Abs(otherPoint.X - dragArea.DrawWidth) + 15);
                    }
                }

                X = newX;
                OnDragBind?.Invoke(e);
                
                if (Clock.CurrentTime - lastSampleTime >= sample_interval)
                {
                    dragSample.Play();
                    lastSampleTime = Clock.CurrentTime;
                }
            }
        }

        protected override void OnDragEnd(DragEndEvent e)
        {
            isDragging = false;
        }

        protected override bool OnHover(HoverEvent e)
        {
            this.FadeColour(Theme.AccentGradient, 100);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            this.FadeColour(Color4.White, 100);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            this.ScaleTo(0.95f, 100);
            return true;
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            this.ScaleTo(1f, 100);
        }
    }
}