using fluXis.Overlay.Volume;
using osu.Framework.Allocation;

namespace fluXis.Tests.Overlay;

public partial class TestVolumeOverlay : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        CreateDummyBeatSync();

        Add(new VolumeOverlay());
    }
}
