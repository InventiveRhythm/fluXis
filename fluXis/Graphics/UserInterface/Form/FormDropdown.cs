using System.Collections.Generic;
using JetBrains.Annotations;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;
using osu.Framework.Platform;

namespace fluXis.Graphics.UserInterface.Form;

public partial class FormDropdown<T> : BaseFormComponent<T, FormDropdown<T>>, IHasCursorType, IHasTooltip
{
    CursorType IHasCursorType.Cursor => CursorType.Hand;
    LocalisableString IHasTooltip.TooltipText => Description ?? "";

    public T[] Items { get; }

    public FormDropdown(LocalisableString label, [NotNull] Bindable<T> bind, IEnumerable<T> items)
        : base(label, bind)
    {
        Items = [.. items];
    }

    protected override Drawable CreateWrapper() => new InnerDropdown(this)
    {
        RelativeSizeAxes = Axes.X,
        Items = Items,
        Current = Bindable
    };

    protected override Drawable CreateContent() => Empty();

    private partial class InnerDropdown : Dropdown<T>
    {
        internal readonly FormDropdown<T> ParentDropdown;

        public InnerDropdown(FormDropdown<T> parent)
        {
            ParentDropdown = parent;
        }

        protected override DropdownHeader CreateHeader() => new InnerHeader(this);
        protected override DropdownMenu CreateMenu() => new InnerMenu();
    }
}
