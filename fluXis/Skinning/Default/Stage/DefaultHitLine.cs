using fluXis.Graphics.UserInterface.Color;
using fluXis.Skinning.Bases;
using fluXis.Skinning.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Skinning.Default.Stage;

public partial class DefaultHitLine : ColorableSkinDrawable
{
    private Colour4 left; // secondary
    private Colour4 right; // primary
    private bool updateColors;

    public DefaultHitLine(SkinJson skinJson)
        : base(skinJson, MapColor.Accent)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.TopCentre;
        Height = 3;
        Colour = ColourInfo.GradientHorizontal(left = GetIndexOrFallback(1, Theme.Secondary), right = GetIndexOrFallback(2, Theme.Primary));

        InternalChild = new Box
        {
            RelativeSizeAxes = Axes.Both
        };
    }

    protected override void RegisterToProvider()
    {
        ColorProvider?.Register(this, MapColor.Primary);
        ColorProvider?.Register(this, MapColor.Secondary);
    }

    protected override void UnregisterFromProvider()
    {
        ColorProvider?.Unregister(this, MapColor.Primary);
        ColorProvider?.Unregister(this, MapColor.Secondary);
    }

    public override void UpdateColor(MapColor index, Colour4 color)
    {
        switch (index)
        {
            case MapColor.Primary:
                left = color;
                break;

            case MapColor.Secondary:
                right = color;
                break;
        }

        updateColors = true;
    }

    protected override void Update()
    {
        base.Update();

        if (!updateColors)
            return;

        Colour = ColourInfo.GradientHorizontal(left, right);
        updateColors = false;
    }
}
