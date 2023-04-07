using System;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Graphics;

public partial class FluXisTextFlow : TextFlowContainer
{
    public FluXisTextFlow(Action<SpriteText> defaultCreationParameters = null)
        : base(defaultCreationParameters)
    {
    }

    protected override SpriteText CreateSpriteText() => new()
    {
        Font = FluXisFont.Default(),
        Shadow = true
    };
}
