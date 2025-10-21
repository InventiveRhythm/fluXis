using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events.Camera;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Utils;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries.Camera;

public partial class CameraMoveEntry : PointListEntry
{
    protected override string Text => "Camera Move";
    protected override Colour4 Color => Theme.CameraMove;

    private CameraMoveEvent camera => Object as CameraMoveEvent;

    public CameraMoveEntry(CameraMoveEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => camera.JsonCopy();

    protected override Drawable[] CreateValueContent() => new FluXisSpriteText
    {
        Text = $"{(int)camera.X}x {(int)camera.Y}y {(int)camera.Duration}ms",
        Colour = Color
    }.Yield().ToArray<Drawable>();

    protected override IEnumerable<Drawable> CreateSettings() => base.CreateSettings().Concat(new Drawable[]
    {
        new PointSettingsLength<CameraMoveEvent>(Map, camera, BeatLength),
        new PointSettingsNumber<float>
        {
            Text = "Target X",
            TooltipText = "The horizontal offset of the camera.",
            DefaultValue = camera.X,
            Step = 10,
            OnValueChanged = v =>
            {
                camera.X = v;
                Map.Update(camera);
            }
        },
        new PointSettingsNumber<float>
        {
            Text = "Target Y",
            TooltipText = "The vertical offset of the camera.",
            DefaultValue = camera.Y,
            Step = 10,
            OnValueChanged = v =>
            {
                camera.Y = v;
                Map.Update(camera);
            }
        },
        new PointSettingsEasing<CameraMoveEvent>(Map, camera),
    });
}
