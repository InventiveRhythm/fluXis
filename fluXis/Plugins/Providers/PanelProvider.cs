using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Presets;
using fluXis.Plugins.Capabilities;
using static fluXis.Plugins.Capabilities.SchedulerCapability;

namespace fluXis.Plugins.Providers;

internal class PanelProvider : IPanelProvider
{
    private readonly PanelContainer panelsContainer;

    public PanelProvider(PanelContainer panels)
    {
        panelsContainer = panels;
    }

    public void SummonPanel<T>(FormPanel<T> form) where T : class
    {
        Schedule(() =>
        {
            panelsContainer.Content = form;
        });
    }
}
