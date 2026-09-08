using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Utils.Extensions;
using JetBrains.Annotations;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Framework.Platform;
using osu.Framework.Threading;
using osuTK;

namespace fluXis.Graphics.UserInterface.Form;

public partial class FormColor : BaseFormComponent<Colour4, FormColor>, IHasPopover, IHasCursorType, IHasTooltip
{
    CursorType IHasCursorType.Cursor => CursorType.Hand;
    LocalisableString IHasTooltip.TooltipText => Description ?? "";

    private FluXisSpriteText text;
    private FluXisTextBox input;

    private Box box;

    public FormColor(LocalisableString label, Colour4 value)
        : this(label, new Bindable<Colour4> { Value = value })
    {
    }

    public FormColor(LocalisableString label, Bindable<Colour4> bind)
        : base(label, bind)
    {
    }

    protected override Drawable CreateContent() => new GridContainer
    {
        RelativeSizeAxes = Axes.Both,
        ColumnDimensions = [new Dimension(), new Dimension(GridSizeMode.Absolute, 64)],
        Content = new[]
        {
            new Drawable[]
            {
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding { Left = PADDING },
                    Spacing = new Vector2(INNER_GAP),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Children =
                    [
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            Height = 16,
                            Child = LabelSprite = CreateDefaultLabel().WithAnchor(Anchor.CentreLeft)
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            Height = 22,
                            Children =
                            [
                                text = new FluXisSpriteText
                                {
                                    WebFontSize = 18,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft
                                },
                                input = new FluXisTextBox
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Height = 22,
                                    Text = Bindable.Value.ToHex(),
                                    PlaceholderText = "#RRGGBB",
                                    LengthLimit = 7,
                                    FontSize = FluXisSpriteText.GetWebFontSize(18),
                                    BackgroundActive = Theme.Background3,
                                    BackgroundInactive = Theme.Background3,
                                    SidePadding = 0,
                                    Alpha = 0,
                                    OnFocusAction = StartHighlight,
                                    OnFocusLostAction = () =>
                                    {
                                        StopHighlight();
                                        input.Hide();
                                        text.Show();
                                    },
                                    OnCommitAction = () =>
                                    {
                                        if (Colour4.TryParseHex(input.Text, out var col))
                                            Bindable.Value = col.Opacity(1f);
                                    },
                                    CommitOnFocusLost = true
                                }
                            ]
                        }
                    ]
                },
                new Container
                {
                    Size = new Vector2(40),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    CornerRadius = 4,
                    Masking = true,
                    Children =
                    [
                        box = new Box { RelativeSizeAxes = Axes.Both }
                    ]
                }
            }
        }
    };

    protected override void LoadComplete()
    {
        Bindable.BindValueChanged(v =>
        {
            text.Text = input.Text = v.NewValue.ToHex();
            box.FadeColour(v.NewValue, 50);
        }, true);
        base.LoadComplete();
    }

    [CanBeNull]
    private ScheduledDelegate openDelegate;

    protected override bool OnClick(ClickEvent e)
    {
        openDelegate = Scheduler.AddDelayed(() =>
        {
            StartHighlight();
            this.ShowPopover();
        }, 100);

        return true;
    }

    protected override bool OnDoubleClick(DoubleClickEvent e)
    {
        openDelegate?.Cancel();
        openDelegate = null;

        input.Show();
        text.Hide();
        GetContainingFocusManager()?.ChangeFocus(input);

        return true;
    }

    public Popover GetPopover() => new FluXisPopover
    {
        ContentPadding = 0,
        OnClose = StopHighlight,
        Child = new FluXisColorPicker { Current = { BindTarget = Bindable } }
    };
}
