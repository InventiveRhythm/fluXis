using System;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Graphics.UserInterface.Form;

public partial class FormDropdown<T>
{
    private partial class InnerSearchBar : DropdownSearchBar
    {
        private readonly Action<bool> updateFocus;

        public InnerSearchBar(Action<bool> updateFocus)
        {
            this.updateFocus = updateFocus;
        }

        protected override TextBox CreateTextBox() => new FluXisTextBox
        {
            PlaceholderText = "Search",
            FontSize = FluXisSpriteText.GetWebFontSize(18),
            SidePadding = 0,
            BackgroundActive = Theme.Background3,
            BackgroundInactive = Theme.Background3,
            OnFocusAction = () => updateFocus(true),
            OnFocusLostAction = () => updateFocus(false)
        };

        protected override void PopIn() => this.FadeInFromZero(50);
        protected override void PopOut() => this.FadeOut(50);
    }
}
