using osu.Framework.Logging;
using fluXis.Scripting.Models;

namespace fluXis.Scripting;

/// <summary>
/// 2 - Made storyboard elements and animations use relative time <br/>
/// 3 - Deprecated visualLane, hitSound and holdTime for Lua Hitobjects and hitobjects types now use rhym directives. Check <see cref="LuaHitObject"/> for more detail. <br/>
///   - Replaced map:NotesInRange, EventsInRange, etc. with a single map:ObjectsInRange.
/// </summary>
public static class StoryboardApiVersion
{
    public const int LATEST = 3;

    public static bool IsValid(int version) => IsValid(version, out _);

    public static bool IsValid(int version, out string error)
    {
        error = null;

        switch (version)
        {
            case > LATEST:
                Logger.Log($"Storyboard API version '{version}' is too new and may be unsupported - Continuing as normal.",
                    level: LogLevel.Important);
                return true;

            case < 1:
                error = $"Unsupported Storyboard API version '{version}'. Supported: {1}-{LATEST}.";
                return false;

            default:
                return true;
        }
    }
}
