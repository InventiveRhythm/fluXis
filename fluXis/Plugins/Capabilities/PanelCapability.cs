using fluXis.Graphics.UserInterface.Panel.Presets;

namespace fluXis.Plugins.Capabilities;

public interface IPanelProvider
{
    void SummonPanel<T>(FormPanel<T> form) where T : class;
}

public static class PanelCapability
{
    public static void SummonPanel<T>(FormPanel<T> form) where T : class
        => CapabilityRegistry.Get<IPanelProvider>().SummonPanel(form);
}
