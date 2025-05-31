using fluXis.Database.Maps;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API.Models.Maps;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;

namespace fluXis.Map.Drawables;

public partial class DifficultyChip : CircularContainer, IHasTooltip
{
    public LocalisableString TooltipText => fallback ? "This is a calculated fallback value and might not represent the actual rating." : "";

    public RealmMap RealmMap
    {
        set
        {
            if (value.Rating > 0)
                setRating(value.Rating, false);
            else
                setRating((value.Filters?.NotesPerSecond / 2) ?? 0, true);
        }
    }

    public APIMap APIMap
    {
        set
        {
            if (value.Rating > 0)
                setRating(value.Rating, false);
            else
                setRating(value.NotesPerSecond / 2, true);
        }
    }

    public float WebFontSize { set => FontSize = FluXisSpriteText.GetWebFontSize(value); }
    public float FontSize { get; set; } = 18;

    private double rating;
    private bool fallback;

    private Box box;
    private FluXisSpriteText text;

    [BackgroundDependencyLoader]
    private void load()
    {
        Masking = true;

        InternalChildren = new Drawable[]
        {
            box = new Box { RelativeSizeAxes = Axes.Both },
            text = new FluXisSpriteText
            {
                FontSize = FontSize,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = .75f,
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        updateRating();
    }

    private void setRating(double value, bool fallback)
    {
        rating = value;
        this.fallback = fallback;

        if (box == null || text == null)
            return;

        updateRating();
    }

    private void updateRating()
    {
        var color = FluXisColors.GetDifficultyColor((float)rating);
        var texColor = Colour4.Black;
        var str = rating.ToStringInvariant("00.00");

        if (fallback)
        {
            color = color.Darken(.5f);
            texColor = Colour4.White;
            str += "?";
        }

        box.Colour = color;
        text.Colour = texColor;
        text.Text = str;
    }
}
