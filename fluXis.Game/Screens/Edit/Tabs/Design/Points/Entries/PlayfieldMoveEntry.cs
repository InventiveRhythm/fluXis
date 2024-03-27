using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Map.Events;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Game.Utils;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Points.Entries;

public partial class PlayfieldMoveEntry : PointListEntry
{
    protected override string Text => "Playfield Move";
    protected override Colour4 Color => Colour4.FromHex("#01FE55");

    private PlayfieldMoveEvent move => Object as PlayfieldMoveEvent;

    public PlayfieldMoveEntry(PlayfieldMoveEvent obj)
        : base(obj)
    {
    }

    protected override Drawable[] CreateValueContent()
    {
        return new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = $"{move.OffsetX.ToStringInvariant()}x {move.Duration.ToStringInvariant()}ms",
                Colour = Color
            }
        };
    }

    protected override IEnumerable<Drawable> CreateSettings()
    {
        return base.CreateSettings().Concat(new Drawable[]
        {
            new PointSettingsLength<PlayfieldMoveEvent>(Map, move, BeatLength),
            new PointSettingsTextBox
            {
                Text = "Offset X",
                DefaultText = move.OffsetX.ToStringInvariant(),
                OnTextChanged = box =>
                {
                    if (box.Text.TryParseFloatInvariant(out var result))
                        move.OffsetX = result;
                    else
                        box.NotifyError();

                    Map.Update(move);
                }
            },
            new PointSettingsTextBox
            {
                Text = "Offset Y",
                DefaultText = move.OffsetY.ToStringInvariant(),
                OnTextChanged = box =>
                {
                    if (box.Text.TryParseFloatInvariant(out var result))
                        move.OffsetY = result;
                    else
                        box.NotifyError();

                    Map.Update(move);
                }
            }
        });
    }
}
