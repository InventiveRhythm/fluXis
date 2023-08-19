using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Overlay.Settings.UI;

public partial class SettingsButton : SettingsItem
{
    public Action Action { get; init; } = () => { };
    public string ButtonText { get; init; } = string.Empty;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new[]
        {
            new ClickableContainer
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Height = 30,
                Width = 100,
                Action = Action,
                CornerRadius = 15,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Accent
                    },
                    new FluXisSpriteText
                    {
                        Text = ButtonText,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Colour = FluXisColors.TextDark
                    }
                }
            }
        });
    }

    public override void Reset()
    {
    }
}
