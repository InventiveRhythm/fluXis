using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using Realms;

namespace fluXis.Game.Screens.Select.List;

public partial class MapDifficultyEntry : Container
{
    private readonly MapListEntry mapListEntry;
    private readonly RealmMap map;

    private SpriteText difficultyName;
    private SpriteText difficultyMapper;
    private Container backgroundContainer;
    private DiffKeyCount keyCount;

    public MapDifficultyEntry(MapListEntry parentEntry, RealmMap map)
    {
        mapListEntry = parentEntry;
        this.map = map;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 40;
        CornerRadius = 5;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            keyCount = new DiffKeyCount(map.KeyCount),
            backgroundContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 5,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background
                }
            },
            difficultyName = new SpriteText
            {
                Text = map.Difficulty,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Margin = new MarginPadding { Left = 10 },
                Font = FluXisFont.Default(22)
            },
            difficultyMapper = new SpriteText
            {
                Text = $"mapped by {map.Metadata.Mapper}",
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Margin = new MarginPadding { Left = 10 },
                Font = FluXisFont.Default(22),
                Colour = FluXisColors.Text2
            }
        };

        EdgeEffect = new EdgeEffectParameters
        {
            Type = EdgeEffectType.Glow,
            Colour = keyCount.Colour,
            Radius = 5
        };

        mapListEntry.Screen.MapInfo.BindValueChanged(e => updateSelected(e.NewValue));
    }

    protected override void LoadComplete()
    {
        difficultyMapper.X = difficultyName.DrawWidth + 3;
        backgroundContainer.Width = (backgroundContainer.RelativeToAbsoluteFactor.X - 40) / backgroundContainer.RelativeToAbsoluteFactor.X;
        base.LoadComplete();
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (Equals(mapListEntry.Screen.MapInfo.Value, map))
            mapListEntry.Screen.Accept();
        else
            mapListEntry.Screen.MapInfo.Value = map;

        return base.OnClick(e);
    }

    private void updateSelected(ISettableManagedAccessor newMap) => FadeEdgeEffectTo(keyCount.KeyColour.Opacity(Equals(newMap, map) ? 1f : 0f), 200, Easing.OutQuint);
}
