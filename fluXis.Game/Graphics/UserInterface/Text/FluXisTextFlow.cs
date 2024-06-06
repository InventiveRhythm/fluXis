using System;
using fluXis.Game.Graphics.Sprites;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Graphics.UserInterface.Text;

public partial class FluXisTextFlow : TextFlowContainer
{
    public float WebFontSize { set => FontSize = FluXisSpriteText.GetWebFontSize(value); }
    public float FontSize { get; set; } = 20;
    public bool Shadow { get; init; } = true;

    public FluXisTextFlow(Action<SpriteText> defaultCreationParameters = null)
        : base(defaultCreationParameters)
    {
    }

    protected override FluXisSpriteText CreateSpriteText() => new()
    {
        FontSize = FontSize,
        Shadow = Shadow
    };
}
