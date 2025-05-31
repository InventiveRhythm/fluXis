using System;
using fluXis.Graphics;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Utils;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Overlay.Notifications.Tasks;

public partial class TaskNotification : CompositeDrawable
{
    private TaskNotificationData data { get; }

    private Container content;
    private Box iconBox;
    private SpriteIcon icon;
    private TruncatingText statusText;
    private Box progressBackground;
    private CircularContainer progress;
    private Box progressFill;

    private Sample appear;
    private Sample disappear;
    private Sample finish;
    private Sample error;

    private readonly Color4 colorWorking = Colour4.FromHex("#66AACC");
    private readonly Color4 colorFailed = Colour4.FromHex("#CC6666");
    private readonly Color4 colorFinished = Colour4.FromHex("#66CC66");

    public float TargetY = 0;

    public TaskNotification(TaskNotificationData data)
    {
        this.data = data;
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        appear = samples.Get("UI/Notifications/in");
        disappear = samples.Get("UI/Notifications/out");
        finish = samples.Get("UI/Notifications/finish");
        error = samples.Get("UI/Notifications/error");

        Size = new Vector2(400, 80);

        InternalChild = content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            CornerRadius = 20,
            Masking = true,
            EdgeEffect = FluXisStyles.ShadowSmall,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background2
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(10),
                    Child = new GridContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        ColumnDimensions = new[]
                        {
                            new Dimension(GridSizeMode.Absolute, 60),
                            new Dimension()
                        },
                        Content = new[]
                        {
                            new Drawable[]
                            {
                                new Container
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    CornerRadius = 10,
                                    Masking = true,
                                    Children = new Drawable[]
                                    {
                                        iconBox = new Box
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Colour = FluXisColors.Background4
                                        },
                                        icon = new FluXisSpriteIcon
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Size = new Vector2(32),
                                            Colour = FluXisColors.Background2
                                        }
                                    }
                                },
                                new Container
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Padding = new MarginPadding { Horizontal = 8, Vertical = 5 },
                                    Children = new Drawable[]
                                    {
                                        new FillFlowContainer
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Direction = FillDirection.Vertical,
                                            Spacing = new Vector2(0, -3),
                                            Children = new Drawable[]
                                            {
                                                new TruncatingText
                                                {
                                                    RelativeSizeAxes = Axes.X,
                                                    Text = data.Text,
                                                    WebFontSize = 16
                                                },
                                                statusText = new TruncatingText
                                                {
                                                    RelativeSizeAxes = Axes.X,
                                                    WebFontSize = 12,
                                                    Alpha = 0.8f
                                                }
                                            }
                                        },
                                        new CircularContainer
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Height = 8,
                                            Masking = true,
                                            Anchor = Anchor.BottomLeft,
                                            Origin = Anchor.BottomLeft,
                                            Children = new Drawable[]
                                            {
                                                progressBackground = new Box
                                                {
                                                    RelativeSizeAxes = Axes.Both,
                                                    Colour = FluXisColors.Background3
                                                },
                                                progress = new CircularContainer
                                                {
                                                    RelativeSizeAxes = Axes.Both,
                                                    Width = 0.5f,
                                                    Masking = true,
                                                    Children = new Drawable[]
                                                    {
                                                        progressFill = new Box
                                                        {
                                                            RelativeSizeAxes = Axes.Both,
                                                            Colour = FluXisColors.Background4
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        Show();

        updateState(LoadingState.Working);
        data.ProgressBindable.BindValueChanged(e => Scheduler.ScheduleIfNeeded(() => resize(e.NewValue)));
        data.StateBindable.BindValueChanged(e => Scheduler.ScheduleIfNeeded(() => updateState(e.NewValue)));
    }

    protected override void Update()
    {
        base.Update();

        if (Precision.AlmostEquals(TargetY, Y))
            Y = TargetY;
        else
            Y = (float)Interpolation.Lerp(TargetY, Y, Math.Exp(-0.01 * Time.Elapsed));
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (data.ClickAction == null || data.State != LoadingState.Complete)
            return false;

        data.ClickAction.Invoke();
        return true;
    }

    public override void Show()
    {
        appear?.Play();
        content.MoveToX(-500).MoveToX(0, 500, Easing.OutQuint);
    }

    public void Hide(float delay = 0)
    {
        this.Delay(delay).FadeOut(300).Expire().OnComplete(_ => disappear?.Play());
        content.Delay(delay).MoveToX(-500, 500, Easing.OutQuint);
    }

    private void updateState(LoadingState state, bool instant = false)
    {
        icon.ClearTransforms();
        icon.RotateTo(0);

        switch (state)
        {
            case LoadingState.Working:
                setColor(colorWorking, instant ? 0 : 500);
                resize(data.Progress);
                icon.Icon = data.WorkingIcon;
                statusText.Text = data.TextWorking;

                if (data.SpinIcon)
                    icon.Spin(1000, RotationDirection.Clockwise);

                break;

            case LoadingState.Failed:
                setColor(colorFailed, instant ? 0 : 500);
                icon.Icon = FontAwesome6.Solid.XMark;
                statusText.Text = data.TextFailed;
                Hide(10000);
                error?.Play();
                break;

            case LoadingState.Complete:
                setColor(colorFinished, instant ? 0 : 500);
                resize(1);
                icon.Icon = FontAwesome6.Solid.Check;
                statusText.Text = data.TextFinished;
                finish?.Play();
                Hide(data.ClickAction != null ? 5000 : 1000);
                break;
        }
    }

    private void setColor(Colour4 color, float duration = 0)
    {
        iconBox.FadeColour(color, duration);
        progressFill.FadeColour(color, duration);
        progressBackground.FadeColour(color.Darken(0.5f), duration);
    }

    private void resize(float taskProgress)
    {
        progress.ResizeWidthTo(taskProgress, 200, Easing.OutQuint);
    }
}
