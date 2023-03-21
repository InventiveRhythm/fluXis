using fluXis.Game.Overlay.Volume;

namespace fluXis.Game.Tests.Overlay;

public partial class TestVolumeOverlay : FluXisTestScene
{
    public TestVolumeOverlay()
    {
        var volumeOverlay = new VolumeOverlay();
        Add(volumeOverlay);
    }
}
