using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Utils.Extensions;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Framework.Platform;
using osuTK;

namespace fluXis.Graphics.UserInterface.Form;

public partial class FormCheckbox : BaseFormComponent<bool, FormCheckbox>, IHasCursorType
{
    CursorType IHasCursorType.Cursor => CursorType.Hand;

    private Container box;
    private FluXisSpriteIcon icon;

    protected override Colour4 BackgroundColor => Theme.Background2;

    public FormCheckbox(LocalisableString label, bool value)
        : this(label, new Bindable<bool> { Value = value })
    {
    }

    public FormCheckbox(LocalisableString label, Bindable<bool> bind)
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
                new FillFlowContainer()
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Padding = new MarginPadding { Left = PADDING },
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(INNER_GAP),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Children =
                    [
                        new ForcedHeightText(true)
                        {
                            RelativeSizeAxes = Axes.X,
                            Text = Label,
                            WebFontSize = 16
                        },
                        new ForcedHeightText(true)
                        {
                            RelativeSizeAxes = Axes.X,
                            Text = Description ?? "",
                            WebFontSize = 14,
                            TextColor = Theme.TextVariant,
                            Alpha = Description == null ? 0 : 1
                        },
                    ]
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Children =
                    [
                        box = new Container
                        {
                            Size = new Vector2(32),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            CornerRadius = 8,
                            Masking = true,
                            Children =
                            [
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Theme.Background2
                                }
                            ]
                        },
                        icon = new FluXisSpriteIcon
                        {
                            Icon = Phosphor.Fill.CheckFat,
                            Size = new Vector2(24),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Colour = Theme.Background2
                        }
                    ]
                }
            }
        }
    };

    protected override void LoadComplete()
    {
        Bindable.BindValueChanged(_ => updateState(), true);
        base.LoadComplete();
    }

    private void updateState()
    {
        box.BorderColorTo(Value ? Theme.Highlight : Theme.Background6, 400, Easing.OutQuint)
           .BorderTo(Value ? 16 : 4, 400, Easing.OutQuint);

        icon.ScaleTo(Value ? 1f : 0f, 400, Easing.OutBack);
    }

    protected override bool OnClick(ClickEvent e)
    {
        Bindable.Value = !Bindable.Value;
        return true;
    }
}
