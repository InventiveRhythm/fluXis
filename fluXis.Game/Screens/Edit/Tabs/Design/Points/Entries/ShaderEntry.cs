using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Map.Events;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Shared.Utils;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Points.Entries;

public partial class ShaderEntry : PointListEntry
{
    protected override string Text => "Shader";
    protected override Colour4 Color => Colour4.Purple;

    private ShaderEvent shader => Object as ShaderEvent;

    public ShaderEntry(ShaderEvent obj)
        : base(obj)
    {
    }

    protected override ITimedObject CreateClone()
    {
        return new ShaderEvent
        {
            Time = Object.Time,
            Duration = shader.Duration,
            ShaderName = shader.ShaderName,
            ShaderParams = shader.ShaderParams.Copy()
        };
    }

    protected override Drawable[] CreateValueContent()
    {
        return new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = shader.ShaderName,
                Colour = Color
            }
        };
    }
}
