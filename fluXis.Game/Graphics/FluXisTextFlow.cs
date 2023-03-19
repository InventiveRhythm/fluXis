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
        Font = new FontUsage("Quicksand", 20, "Bold"),
        Shadow = true
    };
}
