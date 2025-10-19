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

public partial class CameraRotateEntry : PointListEntry
{
    protected override string Text => "Camera Rotate";
    protected override Colour4 Color => Theme.CameraRotate;

    private CameraRotateEvent camera => Object as CameraRotateEvent;

    public CameraRotateEntry(CameraRotateEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => camera.JsonCopy();

    protected override Drawable[] CreateValueContent() => new FluXisSpriteText
    {
        Text = $"{(int)camera.Roll}deg {(int)camera.Duration}ms",
        Colour = Color
    }.Yield().ToArray<Drawable>();

    protected override IEnumerable<Drawable> CreateSettings() => base.CreateSettings().Concat(new Drawable[]
    {
        new PointSettingsLength<CameraRotateEvent>(Map, camera, BeatLength),
        new PointSettingsNumber<float>
        {
            Text = "Rotation",
            TooltipText = "The rotation of the playfield.",
            DefaultValue = camera.Roll,
            Step = 2,
            OnValueChanged = v =>
            {
                camera.Roll = v;
                Map.Update(camera);
            }
        },
        new PointSettingsEasing<CameraRotateEvent>(Map, camera),
    });
}
