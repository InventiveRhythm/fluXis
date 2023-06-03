using System;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Graphics;

public partial class FluXisTextFlow : TextFlowContainer
{
    public int FontSize { get; set; } = 20;
    public bool Shadow { get; set; } = true;

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
