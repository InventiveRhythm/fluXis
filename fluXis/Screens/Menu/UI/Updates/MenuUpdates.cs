using fluXis.Graphics;
using fluXis.Online.API.Models.Other;
using fluXis.Online.API.Requests;
using fluXis.Online.Fluxel;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Screens.Menu.UI.Updates;

public partial class MenuUpdates : Container<Container<MenuUpdates.UpdateImage>>
{
    [Resolved]
    private IAPIClient api { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private FluXisGame game { get; set; }

    public bool CanShow { get; set; }

    private const float update_interval = 5000;

    private bool finishedLoading;
    private double lastUpdate;
    private int currentIndex;

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(400, 200);
        Anchor = Anchor.BottomRight;
        Origin = Anchor.BottomRight;
        AlwaysPresent = true;
        CornerRadius = 20;
        Alpha = 0;
        Masking = true;
        EdgeEffect = FluXisStyles.ShadowMedium;

        loadUpdates();
    }

    private void loadUpdates()
    {
        var req = new MenuUpdatesRequest();

        req.Success += res => Schedule(() =>
        {
            if (res.Data.Count == 0)
                return;

            foreach (var update in res.Data)
            {
                var container = new Container<UpdateImage>
                {
                    RelativeSizeAxes = Axes.Both,
                    RelativePositionAxes = Axes.Both
                };

                Add(container);
                LoadComponentAsync(new UpdateImage(update), img => container.Add(img));
            }

            cycle(true);
            Show(); // in case the menu animation is already done
            finishedLoading = true;
        });

        api.PerformRequestAsync(req);
    }

    protected override void Update()
    {
        base.Update();

        if (InternalChildren.Count == 0)
            return;

        if (Time.Current - lastUpdate < update_interval)
            return;

        lastUpdate = Time.Current;
        cycle();
    }

    public void Show(float move = FluXisScreen.MOVE_DURATION, float fade = FluXisScreen.FADE_DURATION)
    {
        if (!finishedLoading || !CanShow)
            return;

        this.MoveToX(0, move, Easing.OutQuint).FadeIn(fade);
    }

    public override void Hide()
    {
        this.MoveToX(100, FluXisScreen.MOVE_DURATION, Easing.OutQuint).FadeOut(FluXisScreen.FADE_DURATION);
    }

    private void cycle(bool initial = false)
    {
        var idx = 0;

        if (!initial)
            currentIndex += 1;

        if (currentIndex >= InternalChildren.Count)
            currentIndex = 0;

        foreach (var update in InternalChildren)
        {
            var duration = initial ? 0 : 400;

            update.MoveToX(idx - currentIndex, duration, Easing.OutQuint);
            idx += 1;
        }
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (Children.Count < 1)
            return false;

        var child = Children[currentIndex];
        game?.OpenLink(child.Child.UpdateInfo.Url);
        return true;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (e.Button != MouseButton.Left)
            return false;

        this.ScaleTo(.95f, 1000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        this.ScaleTo(1, 1000, Easing.OutElastic);
    }

    public partial class UpdateImage : Sprite
    {
        public MenuUpdate UpdateInfo { get; }

        public UpdateImage(MenuUpdate update)
        {
            UpdateInfo = update;
        }

        [BackgroundDependencyLoader]
        private void load(OnlineTextureStore textures)
        {
            RelativeSizeAxes = Axes.Both;
            FillMode = FillMode.Fill;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Texture = textures.Get(UpdateInfo.Image);
        }

        protected override void LoadComplete()
        {
            this.FadeInFromZero(400);
        }
    }
}
