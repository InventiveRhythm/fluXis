using fluXis.Overlay.Volume;

namespace fluXis.Tests.Overlay;

public partial class TestVolumeOverlay : FluXisTestScene
{
    public TestVolumeOverlay()
    {
        var volumeOverlay = new VolumeOverlay();
        Add(volumeOverlay);
    }
}
