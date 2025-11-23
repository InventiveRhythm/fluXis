using System;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Buttons;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using System.Collections.Generic;
using osuTK;

namespace fluXis.Overlay.Settings.UI;

public partial class SettingsMultiButton : SettingsItem
{
    public (string Text, Action Action)[] Buttons { get; init; } = Array.Empty<(string, Action)>();
    
    private List<FluXisButton> buttons = new();

    protected override Drawable CreateContent()
    {
        var container = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(8, 0)
        };

        for (int i = 0; i < Buttons.Length; i++)
        {
            var buttonData = Buttons[i];
            var button = new FluXisButton
            {
                Text = buttonData.Text,
                Height = 32,
                Width = 150,
                FontSize = FluXisSpriteText.GetWebFontSize(14),
                Action = buttonData.Action
            };
            
            buttons.Add(button);
            container.Add(button);
        }

        return container;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        
        foreach (var button in buttons)
        {
            button.EnabledBindable.BindTo(EnabledBindable);
        }
    }

    protected override void Reset()
    {
    }
}
