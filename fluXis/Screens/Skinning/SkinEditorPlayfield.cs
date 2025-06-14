using fluXis.Screens.Gameplay.Ruleset.Playfields;
using fluXis.Skinning;
using fluXis.Skinning.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Skinning;

public partial class SkinEditorPlayfield : Container
{
    public SkinJson SkinJson { get; set; }
    public ISkin Skin { get; set; }
    public int KeyMode { get; set; }

    private Drawable hitline;
    private FillFlowContainer receptorContainer;
    private Container hitObjectContainer;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        addContent();
    }

    public void Reload()
    {
        Clear();
        addContent();
    }

    private void addContent()
    {
        InternalChildren = new[]
        {
            new Stage(),
            receptorContainer = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre
            },
            hitObjectContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            hitline = Skin.GetHitLine().With(line =>
            {
                line.Width = 1;
                line.RelativeSizeAxes = Axes.X;
            })
        };

        for (int i = 0; i < KeyMode; i++)
        {
            var receptor = Skin.GetReceptor(i + 1, KeyMode, false);
            receptorContainer.Add(receptor);
            receptor.Width = 1f / KeyMode;

            var hitObject = Skin.GetHitObject(i + 1, KeyMode);
            hitObject.Anchor = Anchor.BottomLeft;
            hitObject.Origin = Anchor.BottomLeft;
            hitObjectContainer.Add(hitObject);
        }
    }

    protected override void Update()
    {
        var mode = SkinJson.GetKeymode(KeyMode);

        Width = mode.ColumnWidth * KeyMode;
        receptorContainer.Y = -mode.ReceptorOffset;

        var hitpos = mode.HitPosition;
        hitline.Y = -hitpos;

        foreach (var drawable in hitObjectContainer)
        {
            int index = hitObjectContainer.IndexOf(drawable);

            drawable.Width = 1f / KeyMode;
            drawable.X = index * mode.ColumnWidth;

            float time = index * 50;
            drawable.Y = -hitpos - .5f * (time * 3);
        }
    }
}
