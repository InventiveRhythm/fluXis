using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Utils.Extensions;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Graphics.UserInterface.Form;

public partial class FormToggle : BaseFormComponent<bool, FormToggle>
{
    protected override Colour4 BackgroundColor => Theme.Background2;

    public FormToggle(LocalisableString label, bool value)
        : this(label, new Bindable<bool> { Value = value })
    {
    }

    public FormToggle(LocalisableString label, Bindable<bool> bind)
        : base(label, bind)
    {
    }

    protected override Drawable CreateContent() => new GridContainer
    {
        RelativeSizeAxes = Axes.Both,
        ColumnDimensions = [new Dimension(), new Dimension(GridSizeMode.Absolute, 112)],
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
                new FluXisToggleSwitch { State = Bindable }.WithAnchor(Anchor.Centre)
            }
        }
    };
}
