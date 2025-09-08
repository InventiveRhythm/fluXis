using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Requests.MapSets.Loved;
using fluXis.Online.Fluxel;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Overlay.MapSet.Buttons;

public partial class MapSetLoveButton : MapSetButton
{
    [Resolved]
    private IAPIClient api { get; set; }

    private APIMapSet set { get; }
    private bool state;

    private Container loading;
    private bool updating;

    public MapSetLoveButton(APIMapSet set)
        : base(FontAwesome6.Regular.Heart, () => { })
    {
        this.set = set;
        state = set.Loved ?? false;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        ScaleContainer.Add(loading = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Alpha = 0,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Theme.Background2,
                    Alpha = 0.5f
                },
                new LoadingIcon
                {
                    Size = new Vector2(24),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            }
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        updateIcon();
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (updating)
            return true;

        var req = new MapLoveUpdateRequest(set.ID, !state);
        req.Success += res => finish(res.Data.Loved);
        req.Failure += _ => finish(state);
        api.PerformRequestAsync(req);

        loading.FadeIn(200);

        return true;

        void finish(bool s)
        {
            state = s;
            updating = false;
            loading.FadeOut(200);
            updateIcon();
        }
    }

    private void updateIcon() => Scheduler.ScheduleIfNeeded(() =>
    {
        Icon.Icon = state ? FontAwesome6.Solid.Heart : FontAwesome6.Regular.Heart;
        this.FadeColour(state ? Theme.Pink : Colour4.White, 300);
    });
}
