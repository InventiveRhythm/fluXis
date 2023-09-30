using System.Collections.Generic;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Input;
using fluXis.Game.Map.Drawables;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Maps;
using fluXis.Game.Online.Fluxel;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Browse;

public partial class MapBrowser : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override float BackgroundDim => 0.75f;
    public override float BackgroundBlur => 0.5f;

    [Resolved]
    private Fluxel fluxel { get; set; }

    private FillFlowContainer flow;
    private CornerButton backButton;
    private LoadingIcon loadingIcon;
    private FluXisSpriteText text;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new FluXisScrollContainer
            {
                Width = 1330,
                RelativeSizeAxes = Axes.Y,
                Padding = new MarginPadding { Vertical = 50 },
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Child = flow = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Full,
                    Spacing = new Vector2(20)
                }
            },
            backButton = new CornerButton
            {
                Icon = FontAwesome.Solid.ChevronLeft,
                ButtonText = "Back",
                Action = this.Exit
            },
            text = new FluXisSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0,
                FontSize = 30
            },
            loadingIcon = new LoadingIcon
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(100)
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (fluxel.Status != ConnectionStatus.Online)
        {
            text.Text = "You are not connected to the server!";
            text.FadeInFromZero(20);
            loadingIcon.Hide();
            return;
        }

        loadMapsets();
    }

    private async void loadMapsets()
    {
        var req = fluxel.CreateAPIRequest("/mapsets");
        await req.PerformAsync();

        var json = req.GetResponseString();
        var mapsets = JsonConvert.DeserializeObject<APIResponse<List<APIMapSet>>>(json);

        if (mapsets.Status != 200)
        {
            Logger.Log($"Failed to load mapsets: {mapsets.Status} {mapsets.Message}", LoggingTarget.Network);
            Schedule(loadingIcon.Hide);
            return;
        }

        foreach (var mapSet in mapsets.Data)
        {
            flow.Add(new MapCard(mapSet)
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre
            });
        }

        Schedule(loadingIcon.Hide);
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.FadeInFromZero(250);
        backButton.Show();
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        this.FadeOut(250);
        backButton.Hide();
        return false;
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back:
                this.Exit();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
