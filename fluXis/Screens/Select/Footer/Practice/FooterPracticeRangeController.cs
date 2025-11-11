using System;
using fluXis.Database.Maps;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;
using osuTK.Graphics;

namespace fluXis.Screens.Select.Footer.Practice;

public partial class FooterPracticeRangeController : Container
{
    [Resolved]
    private MapStore maps { get; set; }

    public Bindable<double> LowerBound { get; } = new();
    public Bindable<double> UpperBound { get; } = new();

    public float LowerBoundOffset { get; set; } = 0f;
    public float UpperBoundOffset { get; set; } = 0f;

    private float lowerBoundOffsetRelative { get; set; }
    private float upperBoundOffsetRelative { get; set; }

    private RangePoint lowerBound;
    private RangePoint upperBound;

    private BindableNumber<int> start { get; }
    private BindableNumber<int> end { get; }

    private int endTime = 1;

    private const float default_lowerbound_value = 0f;
    private const float default_upperbound_value = 1f;

    public FooterPracticeRangeController(BindableNumber<int> start, BindableNumber<int> end)
    {
        this.start = start;
        this.end = end;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomLeft;

        InternalChildren = new Drawable[]
        {
            lowerBound = new RangePoint(this, start, true)
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
                OnDragBind = _ =>
                {
                    lowerBoundOffsetRelative = getRelativePosition(LowerBoundOffset);
                    float relativePos = getRelativePosition(lowerBound.X);

                    if (relativePos <= 0.01f)
                        start.Value = 0;
                    else
                    {
                        float timeMs = (relativePos + lowerBoundOffsetRelative) * endTime;
                        start.Value = (int)(timeMs / 1000f);
                    }

                    LowerBound.Value = relativePos;
                },
                OnRightClickBind = _ => resetLowerBound(),
            },
            upperBound = new RangePoint(this, end, false)
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
                OnDragBind = _ =>
                {
                    upperBoundOffsetRelative = getRelativePosition(UpperBoundOffset);
                    float relativePos = getRelativePosition(upperBound.X);

                    if (relativePos >= 0.98f)
                        end.Value = (int)(endTime / 1000f);
                    else
                    {
                        float timeMs = (relativePos + upperBoundOffsetRelative) * endTime;
                        end.Value = (int)(timeMs / 1000f);
                    }

                    UpperBound.Value = relativePos;
                },
                OnRightClickBind = _ => resetUpperBound(),
            }
        };

        lowerBound.SetOtherPoint(upperBound);
        upperBound.SetOtherPoint(lowerBound);

        start.BindValueChanged(v =>
        {
            if (v.NewValue > 0)
            {
                float relativePos = Math.Clamp(v.NewValue * 1000f / endTime, 0f, 1f);
                float newPos = getAbsolutePosition(relativePos);

                if (newPos < upperBound.X)
                {
                    lowerBound.UpdatePosition(newPos);
                    LowerBound.Value = relativePos;
                }
            }
            else resetLowerBound();
        }, true);

        end.BindValueChanged(v =>
        {
            if (v.NewValue * 1000 < endTime)
            {
                float relativePos = Math.Clamp(v.NewValue * 1000f / endTime, 0f, 1f);
                float newPos = getAbsolutePosition(relativePos);

                if (newPos > lowerBound.X)
                {
                    upperBound.UpdatePosition(newPos);
                    UpperBound.Value = relativePos;
                }
            }
            else resetUpperBound();
        }, true);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        maps.MapBindable.BindValueChanged(mapChanged, true);

        resetLowerBound();
        resetUpperBound();
    }

    private void mapChanged(ValueChangedEvent<RealmMap> v)
    {
        var info = v.NewValue?.GetMapInfo();

        if (info is null || info.HitObjects.Count == 0)
        {
            endTime = 1;
        }
        else
        {
            endTime = (int)info.EndTime;
        }

        start.Value = 0;
        end.Value = (int)(endTime / 1000f);

        resetLowerBound();
        resetUpperBound();
    }

    private float getAbsolutePosition(float relativePos) => relativePos * DrawWidth;
    private float getRelativePosition(float absoluteX) => absoluteX / DrawWidth;

    private void resetLowerBound()
    {
        lowerBound.UpdatePosition(getAbsolutePosition(default_lowerbound_value));
        start.Value = 0;
        LowerBound.Value = default_lowerbound_value;
    }

    private void resetUpperBound()
    {
        upperBound.UpdatePosition(getAbsolutePosition(default_upperbound_value));
        end.Value = (int)(endTime / 1000f);
        UpperBound.Value = default_upperbound_value;
    }

    private partial class RangePoint : CompositeDrawable
    {
        private Bindable<double> samplePitch;
        private Sample dragSample;
        private double lastSampleTime;
        private const int sample_interval = 50;

        private const float line_width = 3f;
        private const float triangle_size = 15f;
        private const float drag_area_width = 30f;
        private const float min_distance = 10f;

        private readonly FooterPracticeRangeController parent;
        public readonly BindableNumber<int> BindableValue;
        private readonly bool isLowerBound;
        private RangePoint otherPoint;
        private bool isDragging;
        public Action<DragEvent> OnDragBind;
        public Action<MouseDownEvent> OnRightClickBind;

        public RangePoint(FooterPracticeRangeController parent, BindableNumber<int> bindableValue, bool isLowerBound)
        {
            this.parent = parent;
            this.isLowerBound = isLowerBound;
            BindableValue = bindableValue;
        }

        [BackgroundDependencyLoader]
        private void load(ISampleStore samples)
        {
            dragSample = samples.Get("UI/slider-tick");
            dragSample?.AddAdjustment(AdjustableProperty.Frequency, samplePitch = new Bindable<double>());

            RelativeSizeAxes = Axes.Y;
            Width = drag_area_width;
            Colour = Theme.Text;

            InternalChildren = new Drawable[]
            {
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

        public void SetOtherPoint(RangePoint other) => otherPoint = other;

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
            if (Parent == null) return;

            var mousePos = e.ScreenSpaceMousePosition;
            var parentScreenSpace = Parent.ScreenSpaceDrawQuad;

            bool inBoundsX = mousePos.X >= parentScreenSpace.TopLeft.X &&
                             mousePos.X <= parentScreenSpace.TopRight.X;

            if (!inBoundsX) return;

            var mouseLocalPos = Parent.ToLocalSpace(mousePos);
            float newX = mouseLocalPos.X;

            newX = Math.Clamp(newX, 0, Parent.DrawWidth);

            if (otherPoint != null)
            {
                if (isLowerBound)
                {
                    if (!(BindableValue.Value + 1 > otherPoint.BindableValue.Value && e.Delta.X > 0))
                        newX = Math.Min(newX, otherPoint.X - min_distance);
                }
                else
                {
                    if (!(BindableValue.Value - 1 < otherPoint.BindableValue.Value && e.Delta.X < 0))
                        newX = Math.Max(newX, otherPoint.X + min_distance);
                }
            }

            X = newX;
            OnDragBind?.Invoke(e);

            if (Clock.CurrentTime - lastSampleTime >= sample_interval)
            {
                samplePitch.Value = .7f + (X / Parent.DrawWidth) * .6f;
                dragSample?.Play();
                lastSampleTime = Clock.CurrentTime;
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
            this.FadeColour(Theme.Text, 100);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            this.ScaleTo(0.95f, 100);
            if (e.Button == MouseButton.Right)
                OnRightClickBind?.Invoke(e);
            return true;
        }

        protected override bool OnClick(ClickEvent e) => true;
        protected override void OnMouseUp(MouseUpEvent e) => this.ScaleTo(1f, 100);
    }
}
