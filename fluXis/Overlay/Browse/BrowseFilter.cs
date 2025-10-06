using System.Collections.Generic;
using System.Linq;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Screens.Select.Search.Dropdown;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Browse;

public partial class BrowseFilter<T> : FluXisFilterButtonsBase<T> where T : struct
{
    private readonly BindableList<T> selected;
    private readonly Option[] optionArray;

    protected override T[] Values { get; }
    protected override string Label { get; }
    protected override float FontSize { get; set; } = 18;
    private List<T> filterList;
    protected override List<T> FilterList => filterList;
    public override T[] DefaultFilter { get; set; } = System.Array.Empty<T>();

    public BrowseFilter(LocalisableString label, BindableList<T> selected, IEnumerable<Option> options, InputManager input)
        : base(input)
    {
        this.selected = selected;
        filterList = selected.ToList();
        Label = label.ToString();
        optionArray = options.ToArray();
        Values = optionArray.Select(o => o.Value).ToArray();
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Height = 20;
        Padding = new MarginPadding { Horizontal = 0 };

        ButtonFlow.Anchor = Anchor.CentreLeft;
        ButtonFlow.Origin = Anchor.CentreLeft;
        ButtonFlow.Margin = new MarginPadding { Left = Text.DrawWidth + 10 };

        OnFilterChanged = updateBindableList;
        UpdateAllButtons();
    }

    private void updateBindableList()
    {
        selected.Clear();
        selected.AddRange(filterList);
    }

    protected override Drawable CreateButton(T value)
    {
        var option = optionArray.First(o => EqualityComparer<T>.Default.Equals(o.Value, value));
        return new Button(option, selected, this);
    }

    protected new void OnValueClick(T value)
    {
        base.OnValueClick(value);
        OnFilterChanged.Invoke();
    }

    public class Option
    {
        public T Value { get; }
        public LocalisableString Text { get; }
        public Colour4 Color { get; }

        public Option(T value, LocalisableString text, Colour4 color)
        {
            Value = value;
            Text = text;
            Color = color;
        }
    }

    private partial class Button : CompositeDrawable, ISelectableButton<T>
    {
        [Resolved]
        private UISamples samples { get; set; }

        private readonly Option option;
        private readonly BindableList<T> selected;
        private readonly BrowseFilter<T> parent;

        private Box background;
        private HoverLayer hover;
        private FlashLayer flash;
        private FluXisSpriteText text;

        public Button(Option option, BindableList<T> selected, BrowseFilter<T> parent)
        {
            this.option = option;
            this.selected = selected;
            this.parent = parent;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            AutoSizeAxes = Axes.X;
            Height = 20;
            Anchor = Anchor.CentreLeft;
            Origin = Anchor.CentreLeft;
            CornerRadius = 4;
            Masking = true;

            InternalChildren = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = option.Color,
                    Alpha = 0
                },
                hover = new HoverLayer(),
                flash = new FlashLayer(),
                text = new FluXisSpriteText
                {
                    Text = option.Text,
                    WebFontSize = 12,
                    Margin = new MarginPadding { Horizontal = 8 },
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = .6f
                }
            };
        }

        public void UpdateSelection()
        {
            bool enabled = (parent.FilterList.Count == 0 && parent.DefaultFilter.Length == 0) || parent.FilterList.Contains(option.Value);

            background.Alpha = enabled ? 1f : 0f;
            text.Alpha = enabled ? 1f : .6f;
            text.Colour = enabled ? Theme.TextDark : Theme.Text;
        }

        protected override bool OnHover(HoverEvent e)
        {
            samples.Hover();
            hover.Show();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.Hide();
        }

        protected override bool OnClick(ClickEvent e)
        {
            samples.Click();
            flash.Show();

            parent.OnValueClick(option.Value);
            return true;
        }

        public void Flash() => flash.Show();
    }
}