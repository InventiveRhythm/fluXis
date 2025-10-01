using System;
using fluXis.Database.Maps;
using fluXis.Graphics.Sprites.Text;
using fluXis.UI;
using fluXis.Utils;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;

namespace fluXis.Screens.Select.List.Items;

public class CollectionMissingItem : IListItem
{
    public Bindable<MapUtils.SortingMode> Sorting { get; set; }
    public Bindable<SelectedState> State { get; } = new();

    public SelectScreen Screen { get; set; }
    public ISelectionManager Selection { get; set; }

    public RealmMapMetadata Metadata => new();

    public float Size => 48;
    public float Position { get; set; }
    public bool Displayed { get; set; }

    public Drawable Drawable { get; set; }

    private readonly long count;

    public CollectionMissingItem(long count)
    {
        this.count = count;
    }

    public Drawable CreateDrawable()
    {
        if (Drawable != null)
            return Drawable;

        var draw = new Container
        {
            RelativeSizeAxes = Axes.X,
            Height = Size,
            CornerRadius = 8,
            Masking = true,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.Black,
                    Alpha = 0.5f
                },
                new FluXisSpriteText
                {
                    Text = $"The current collection is missing {count} item{(count > 1 ? "s" : "")}.",
                    FontSize = 20,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            }
        };

        draw.OnUpdate += d =>
        {
            if (Precision.AlmostEquals(Position, d.Y))
                d.Y = Position;
            else
                d.Y = (float)Interpolation.Lerp(Position, d.Y, Math.Exp(-0.01 * d.Time.Elapsed));
        };

        return Drawable = draw;
    }

    public void Bind() { }
    public void Unbind() { }
    public void Select(bool last = false) { }
    public bool ChangeChild(int by) => true;
    public bool Matches(object obj) => false;
    public bool MatchesFilter(SearchFilters filters) => true;

    public int CompareTo(IListItem other)
    {
        return 1;
    }
}
