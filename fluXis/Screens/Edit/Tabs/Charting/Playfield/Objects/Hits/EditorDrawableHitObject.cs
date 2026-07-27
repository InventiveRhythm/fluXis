using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Objects.Hits;

public abstract partial class EditorDrawableHitObject : EditorDrawableObject
{
    public new HitObject Data => base.Data as HitObject;

    protected virtual Colour4 TextColor => Theme.TextDark;

    private FluXisSpriteText groupText;
    private FluXisSpriteText sampleText;

    private bool overZero = true;
    private const int max_distance = 100;

    protected EditorDrawableHitObject(HitObject hit)
        : base(hit)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildrenEnumerable = CreateContent().Concat(new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(-4),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Children =
            [
                groupText = new FluXisSpriteText
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Colour = TextColor,
                    WebFontSize = 12
                },
                sampleText = new FluXisSpriteText
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Colour = TextColor,
                    WebFontSize = 10
                }
            ]
        }.Yield());
    }

    protected abstract IEnumerable<Drawable> CreateContent();

    protected override void Update()
    {
        base.Update();

        groupText.Text = Data.Group;
        sampleText.Text = Data.HitSound?.Replace(".wav", "") ?? ":normal";

        if (Data.Time <= EditorClock.CurrentTime && EditorClock.CurrentTime - Data.Time <= max_distance && overZero)
            Playfield.PlayHitSound(Data);

        overZero = Data.Time > EditorClock.CurrentTime;
    }
}
