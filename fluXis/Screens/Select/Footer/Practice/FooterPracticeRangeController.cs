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
using osu.Framework.Utils;
using osuTK;
using osuTK.Input;
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

    private float originalWidth = 1;

    private int endTime = 1;

    private int defaultStartX = 0;
    private int defaultEndX = 0;

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
                Origin = Anchor.Centre,
                OnDragBind = e =>
                {
                    if (startPoint.X <= defaultStartX)
                        start.Value = 0;
                    else
                    {
                       float timeMs = posToTime(startPoint.X + 10);
                       start.Value = (int)(timeMs / 1000f); 
                    }
                },
                OnRightClickBind = e =>
                {
                    startPoint.X = defaultStartX;
                    start.Value = 0;
                },
            },
            endPoint = new RangePoint(this, end, false)
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
                OnDragBind = e =>
                {
                    if (endPoint.X / defaultEndX > 0.99)
                        end.Value = endTime;
                    else
                    {
                        float timeMs = posToTime(endPoint.X - 5);
                        end.Value = (int)(timeMs / 1000f);
                    }
                },
                OnRightClickBind = e =>
                {
                    endPoint.X = defaultEndX;
                    end.Value = endTime;
                },
            }
        };

        startPoint.SetOtherPoint(endPoint);
        endPoint.SetOtherPoint(startPoint);
        
        start.BindValueChanged(v =>
        {
            if (v.NewValue * 1000 <= 0)
                startPoint.UpdatePosition(defaultStartX);
            else
            {
                float newPos = timeToPos(v.NewValue * 1000);
                if (newPos >= endPoint.X - 10) return;
                startPoint.UpdatePosition(newPos);
            }
        });

        end.BindValueChanged(v =>
        {
            if (v.NewValue * 1000 >= endTime)
                endPoint.UpdatePosition(defaultEndX);
            else
            {
                float newPos = timeToPos(v.NewValue * 1000);
                if (newPos <= startPoint.X + 10) return;
                endPoint.UpdatePosition(newPos);
            }
        });
    }

    protected override void LoadComplete()
    {
        originalWidth = DrawWidth;

        maps.MapBindable.BindValueChanged(mapChanged, true);

        defaultStartX = 0;
        defaultEndX = (int)originalWidth + 7;

        startPoint.X = defaultStartX;
        endPoint.X = defaultEndX;
    }
    
    private void mapChanged(ValueChangedEvent<RealmMap> v)
    {
        startPoint.X = defaultStartX;
        endPoint.X = defaultEndX;

        var info = v.NewValue.GetMapInfo();

        if (info is null || info.HitObjects.Count == 0)
        {
            endTime = 1;
            return;
        }
            
        endTime = (int)info.EndTime;
    }

    private float timeToPos(float time) => time / endTime * DrawWidth;
    private float posToTime(float pos) => pos / DrawWidth * endTime;

    private partial class RangePoint : CompositeDrawable
    {
        private Sample dragSample;
        private double lastSampleTime = 0;
        private const int sample_interval = 50;

        private const float line_width = 3f;
        private const float triangle_size = 15f;
        private const float drag_area_width = 30f;

        private readonly FooterPracticeRangeController parent;
        public readonly BindableNumber<int> BindableValue;
        private readonly bool isStartPoint;
        private RangePoint otherPoint;
        private bool isDragging;
        public Action<DragEvent> OnDragBind;
        public Action<MouseDownEvent> OnRightClickBind;

        private Box dragArea;

        public RangePoint(FooterPracticeRangeController parent, BindableNumber<int> bindableValue, bool isStartPoint)
        {
            this.parent = parent;
            this.isStartPoint = isStartPoint;
            BindableValue = bindableValue;

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

                new Box
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Y,
                    Width = line_width,
                    Colour = Color4.White
                },

                new FluXisSpriteIcon
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.BottomCentre,
                    Icon = FontAwesome.Solid.CaretDown,
                    Size = new Vector2(triangle_size),
                    Colour = Color4.White,
                },

                new FluXisSpriteIcon
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
                newX = Math.Max(0, Math.Min(Parent.DrawWidth + 7, newX));

                if (otherPoint != null)
                {
                    if (isStartPoint)
                    {
                        if (!(BindableValue.Value + 1 > otherPoint.BindableValue.Value && e.Delta.X > 0))
                            newX = Math.Min(newX, otherPoint.X - triangle_size);
                    }
                    else
                    {
                        if (!(BindableValue.Value - 1 < otherPoint.BindableValue.Value && e.Delta.X < 0))
                            newX = Math.Max(newX, otherPoint.X + triangle_size);
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
            this.FadeColour(Theme.Highlight, 100);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            this.FadeColour(Color4.White, 100);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            this.ScaleTo(0.95f, 100);
            if (e.Button == MouseButton.Right)
                OnRightClickBind?.Invoke(e);
            return true;
        }

        protected override bool OnClick(ClickEvent e) { return true; }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            this.ScaleTo(1f, 100);
        }
    }
}