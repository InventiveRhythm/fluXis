using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Map.Drawables;
using fluXis.Online.API.Models.Maps;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Overlay.MapSet.UI.Difficulties;

public partial class DifficultyChip : CircularContainer
{
    private APIMapSet set { get; }
    private APIMap map { get; }
    private Bindable<APIMap> bind { get; }

    private FillFlowContainer text;

    public DifficultyChip(APIMapSet set, APIMap map, Bindable<APIMap> bind)
    {
        this.set = set;
        this.map = map;
        this.bind = bind;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(48);
        AutoSizeAxes = Axes.X;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background3
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                AutoSizeDuration = 200,
                AutoSizeEasing = Easing.OutQuint,
                Direction = FillDirection.Horizontal,
                Children = new Drawable[]
                {
                    new KeyModeIcon
                    {
                        Size = new Vector2(48),
                        KeyMode = map.Mode,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft
                    },
                    text = new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Right = 16 },
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Spacing = new Vector2(-2),
                        Alpha = 0,
                        Children = new Drawable[]
                        {
                            new ForcedHeightText
                            {
                                Height = 20,
                                Text = map.Difficulty,
                                WebFontSize = 14
                            },
                            new ForcedHeightText
                            {
                                Height = 16,
                                Text = $"mapped by {map.Mapper.Username}",
                                WebFontSize = 10,
                                Alpha = set.Creator.ID == map.Mapper.ID ? 0 : .8f
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        bind.BindValueChanged(onMapChanged, true);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        bind.ValueChanged -= onMapChanged;
    }

    private void onMapChanged(ValueChangedEvent<APIMap> e)
    {
        text.FadeTo(e.NewValue.ID == map.ID ? 1 : 0, 200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (bind.Value.ID == map.ID)
            return false;

        bind.Value = map;
        return true;
    }
}
