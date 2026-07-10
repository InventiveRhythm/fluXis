using System;
using System.Collections.Generic;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Input;
using fluXis.Overlay.Navigator.Pages.Club;
using fluXis.Overlay.Navigator.Pages.User;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using NavigatorMapSetPage = fluXis.Overlay.Navigator.Pages.MapSet.NavigatorMapSetPage;

namespace fluXis.Overlay.Navigator;

#nullable enable

public partial class OnlineNavigator : IconEntranceOverlay, IKeyBindingHandler<FluXisGlobalKeybind>
{
    protected override float OpenedRoundness => 8;
    protected override float OverlayPadding => 32;
    protected override ColourInfo BackgroundColor => Theme.Background1;
    protected override IconUsage Icon => Phosphor.Bold.Compass;
    protected override float MaxWidth => 1536;

    public const float HEADER_HEIGHT = 36;

    [Resolved]
    private Toolbar.Toolbar? toolbar { get; set; }

    public long PageCount => pages.Count;
    public NavigatorPage? CurrentPage => current.Value;

    private readonly Stack<NavigatorPage> pages = new();
    private readonly Bindable<NavigatorPage?> current = new();
    private bool locked;

    private Container background = null!;
    private FluXisScrollContainer scroll = null!;
    private Container<NavigatorPage> content = null!;
    private LoadingIcon loading = null!;

    protected override IEnumerable<Drawable> CreateContent()
    {
        yield return background = new Container { RelativeSizeAxes = Axes.Both };
        yield return new NavigatorBar(this, current);
        yield return new Container
        {
            RelativeSizeAxes = Axes.Both,
            Padding = new MarginPadding { Top = HEADER_HEIGHT },
            Children =
            [
                scroll = new FluXisScrollContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    ScrollbarOverlapsContent = true,
                    ScrollbarVisible = false,
                    Child = content = new Container<NavigatorPage>
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Padding = new MarginPadding(32)
                    }
                },
                loading = new LoadingIcon
                {
                    Size = new Vector2(32),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            ]
        };
    }

    protected override void Update()
    {
        base.Update();

        var pad = toolbar?.Height + toolbar?.Y ?? 0;
        if (Math.Abs(pad - Padding.Top) > 0.1f) Padding = new MarginPadding { Top = pad };
    }

    public void Push(NavigatorPage page)
    {
        if (locked)
            return;

        Show();

        locked = true;
        pages.Push(page);
        loading.Show();

        background.ForEach(x => x.FadeOut(Styling.TRANSITION_FADE).Expire());
        current.Value?.FadeOut(Styling.TRANSITION_FADE);
        current.Value = page;

        Scheduler.AddDelayed(() => LoadComponentAsync(page, p =>
        {
            if (!pages.Contains(p))
            {
                p.Dispose();
                return;
            }

            content.Clear(false);
            content.Add(p);

            var bg = p.CreateBackground();
            if (bg is not null) background.Add(bg);

            p.FadeInFromZero(Styling.TRANSITION_FADE);
            loading.Hide();
            locked = false;
        }), Styling.TRANSITION_ENTER_DELAY * 2);
    }

    public void Refresh()
    {
        if (locked || current.Value is null)
            return;

        locked = true;
        current.Value?.Refresh(() => locked = false);
    }

    public void Pop()
    {
        if (locked)
            return;

        if (pages.Count == 1)
        {
            Hide();
            return;
        }

        locked = true;

        pages.Pop();
        var prev = pages.Peek();

        background.ForEach(x => x.FadeOut(Styling.TRANSITION_FADE).Expire());
        current.Value?.FadeOut(Styling.TRANSITION_FADE);

        Scheduler.AddDelayed(() =>
        {
            content.Clear(true);
            content.Add(prev);

            var bg = prev.CreateBackground();
            if (bg is not null) background.Add(bg);

            prev.FadeInFromZero(Styling.TRANSITION_FADE);
            current.Value = prev;
            locked = false;
        }, Styling.TRANSITION_ENTER_DELAY * 2);
    }

    private void reset()
    {
        content.Clear();
        pages.ForEach(x => x.Dispose());
        pages.Clear();

        current.Value = null;
        locked = false;
    }

    protected override void PopIn()
    {
        reset();
        base.PopIn();
        scroll.ScrollTo(0, false);
    }

    protected override void PopOut()
    {
        locked = false;
        base.PopOut();
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back:
                Pop();
                break;
        }

        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }

    #region Preset

    public void PushUser(long id) => Push(new NavigatorUserPage(id));
    public void PushMapSet(long id) => Push(new NavigatorMapSetPage(id));
    public void PushClub(long id) => Push(new NavigatorClubPage(id));

    #endregion
}
