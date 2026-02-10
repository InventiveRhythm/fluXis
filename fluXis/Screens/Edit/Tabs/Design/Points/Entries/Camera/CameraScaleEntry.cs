using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events.Camera;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.UI.Variable;
using fluXis.Screens.Edit.UI.Variable.Preset;
using fluXis.Utils;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries.Camera;

public partial class CameraScaleEntry : PointListEntry
{
    protected override string Text => "Camera Scale";
    protected override Colour4 Color => Theme.CameraScale;

    private CameraScaleEvent camera => Object as CameraScaleEvent;

    public CameraScaleEntry(CameraScaleEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => camera.JsonCopy();

    protected override Drawable[] CreateValueContent() => new FluXisSpriteText
    {
        Text = $"{camera.Scale.ToStringInvariant("0.00")}x {(int)camera.Duration}ms",
        Colour = Color
    }.Yield().ToArray<Drawable>();

    protected override IEnumerable<Drawable> CreateSettings() => base.CreateSettings().Concat(new Drawable[]
    {
        new EditorVariableLength<CameraScaleEvent>(Map, camera, BeatLength),
        new EditorVariableNumber<float>
        {
            Text = "ScaleX",
            TooltipText = "The horizontal scale of the playfield.",
            Formatting = "0.0##",
            CurrentValue = camera.Scale,
            Step = 0.05f,
            OnValueChanged = v =>
            {
                camera.Scale = v;
                Map.Update(camera);
            }
        },
        new EditorVariableEasing<CameraScaleEvent>(Map, camera),
    });
}
