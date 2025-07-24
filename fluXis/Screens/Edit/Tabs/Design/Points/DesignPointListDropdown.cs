using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Design.Points;

public partial class DesignPointListDropdown : FluXisDropdown<PointsList.DropdownEntry>
{
    public DesignPointListDropdown()
    {
        Width = 150;
    }

    protected override DropdownHeader CreateHeader() => new CustomHeader(this);

    protected override LocalisableString GenerateItemText(PointsList.DropdownEntry item) => item.Text;

    private partial class CustomHeader : FluXisDropdownHeader
    {
        private DesignPointListDropdown dropdown { get; }

        public CustomHeader(DesignPointListDropdown dropdown)
        {
            this.dropdown = dropdown;

            Height = 32;
            CornerRadius = 16;
            Foreground.Padding = new MarginPadding { Left = 12, Right = 10 };
            Icon.Size = new Vector2(14);
            LabelText.WebFontSize = 12;

            BackgroundColour = Theme.Background3;
            BackgroundColourHover = Theme.Background4;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            dropdown.Current.BindValueChanged(e => LabelText.Colour = e.NewValue.Color, true);
        }
    }
}
