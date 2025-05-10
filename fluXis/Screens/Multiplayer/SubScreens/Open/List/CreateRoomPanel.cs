using System;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Buttons.Presets;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Online.API.Models.Multi;
using fluXis.Screens.Edit.Tabs.Setup.Entries;
using fluXis.Utils.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Multiplayer.SubScreens.Open.List;

public partial class CreateRoomPanel : Panel
{
    // name, privacy, password
    public CreateRoomPanel(string defaultName, Action<string, MultiplayerPrivacy, string, Action> create)
    {
        Width = 490;
        AutoSizeAxes = Axes.Y;

        Content.RelativeSizeAxes = Axes.X;
        Content.AutoSizeAxes = Axes.Y;

        var name = defaultName;
        var password = "";

        Content.Child = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Spacing = new Vector2(12),
            Direction = FillDirection.Vertical,
            Children = new Drawable[]
            {
                new FluXisTextFlow
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    TextAnchor = Anchor.TopCentre,
                    Text = "Create new room",
                    FontSize = FluXisSpriteText.GetWebFontSize(20),
                    Shadow = false
                },
                new SetupTextBox("Name")
                {
                    Default = name,
                    Placeholder = "Room Name",
                    BackgroundColor = FluXisColors.Background2,
                    OnChange = v => name = v
                },
                new SetupTextBox("Password")
                {
                    Default = password,
                    Placeholder = "Leave empty for a public lobby",
                    BackgroundColor = FluXisColors.Background2,
                    OnChange = v => password = v
                },
                new Container
                {
                    Width = 450,
                    AutoSizeAxes = Axes.Y,
                    Children = new Drawable[]
                    {
                        new FluXisButton
                        {
                            Width = 219,
                            Height = 48,
                            Data = new CancelButtonData(),
                            FontSize = FluXisSpriteText.GetWebFontSize(14),
                            Action = Hide
                        },
                        new FluXisButton
                        {
                            Width = 219,
                            Height = 48,
                            Text = "Create!",
                            FontSize = FluXisSpriteText.GetWebFontSize(14),
                            Anchor = Anchor.TopRight,
                            Origin = Anchor.TopRight,
                            Data = new PrimaryButtonData("Create!", () =>
                            {
                                StartLoading();
                                create?.Invoke(
                                    name,
                                    string.IsNullOrWhiteSpace(password) ? MultiplayerPrivacy.Public : MultiplayerPrivacy.Private,
                                    password,
                                    () => Scheduler.ScheduleIfNeeded(() => StopLoading())
                                );
                            })
                        },
                    }
                },
            }
        };
    }
}
