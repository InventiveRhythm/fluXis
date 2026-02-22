using System.Collections.Generic;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API.Models.Maps.Modding;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Overlay.MapSet.UI.Modding;

public partial class ModdingActionTypeCircle : Container
{
    private static readonly Dictionary<APIModdingActionType, (IconUsage, Colour4)> icon_map = new()
    {
        { APIModdingActionType.Note, (FontAwesome6.Solid.NoteSticky, Theme.Cyan) },
        { APIModdingActionType.Approve, (FontAwesome6.Solid.Check, Theme.Green) },
        { APIModdingActionType.Deny, (FontAwesome6.Solid.XMark, Theme.Red) },
        { APIModdingActionType.Update, (FontAwesome6.Solid.ArrowsRotate, Theme.Yellow) },
        { APIModdingActionType.Submitted, (FontAwesome6.Solid.AngleDoubleRight, Theme.Pink) }
    };

    private readonly APIModdingActionType type;

    public ModdingActionTypeCircle(APIModdingActionType type)
    {
        this.type = type;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        (IconUsage icon, Colour4 colour) = icon_map.ContainsKey(type)
            ? icon_map[type]
            : (FontAwesome6.Solid.Gear, Theme.Red); // fallback for unhandled types

        Size = new Vector2(32, 32);
        Children = new Drawable[]
        {
            new Circle
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background2,
            },
            new FluXisSpriteIcon
            {
                Icon = icon,
                Colour = colour,
                RelativeSizeAxes = Axes.Both,
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Size = new Vector2(0.4f, 0.4f)
            }
        };
    }
}
