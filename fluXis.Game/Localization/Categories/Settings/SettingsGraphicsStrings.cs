using osu.Framework.Localisation;

namespace fluXis.Game.Localization.Categories.Settings;

public class SettingsGraphicsStrings : LocalizationCategory
{
    protected override string File => "settings-graphics";

    public TranslatableString Title => Get("title", "Graphics");

    #region Layout

    public TranslatableString Layout => Get("layout-title", "Layout");

    public TranslatableString WindowMode => Get("window-mode", "Window Mode");

    #endregion

    #region Rendering

    public TranslatableString Rendering => Get("rendering-title", "Rendering");

    public TranslatableString Renderer => Get("renderer", "Renderer");
    public TranslatableString RendererDescription => Get("renderer-description", "Requires a restart to take effect.");

    public TranslatableString FrameLimiter => Get("frame-limiter", "Frame Limiter");

    public TranslatableString ThreadingMode => Get("threading-mode", "Threading Mode");

    public TranslatableString ShowFps => Get("show-fps", "Show FPS");
    public TranslatableString ShowFpsDescription => Get("show-fps-description", "Shows the current DrawsPerSecond and UpdatesPerSecond in the bottom right corner.");

    #endregion
}
