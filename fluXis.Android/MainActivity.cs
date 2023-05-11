using System;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using osu.Framework.Android;
using Debug = System.Diagnostics.Debug;

namespace fluXis.Android;

[Activity(ConfigurationChanges = DEFAULT_CONFIG_CHANGES, Exported = true, LaunchMode = DEFAULT_LAUNCH_MODE, MainLauncher = true)]
public class MainActivity : AndroidGameActivity
{
    public ScreenOrientation DefaultOrientation = ScreenOrientation.Unspecified;

    protected override osu.Framework.Game CreateGame() => new FluXisGameAndroid();

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        Debug.Assert(Window != null);

        Window.AddFlags(WindowManagerFlags.KeepScreenOn);
        Window.AddFlags(WindowManagerFlags.Fullscreen);

        Debug.Assert(WindowManager?.DefaultDisplay != null);
        Debug.Assert(Resources?.DisplayMetrics != null);

        Point displaySize = new Point();
#pragma warning disable CS0618
        WindowManager.DefaultDisplay.GetSize(displaySize);
#pragma warning restore CS0618
        float smallestWidthDp = Math.Min(displaySize.X, displaySize.Y) / Resources.DisplayMetrics.Density;
        bool isTablet = smallestWidthDp >= 600f;
        RequestedOrientation = DefaultOrientation = isTablet ? ScreenOrientation.FullUser : ScreenOrientation.SensorLandscape;
    }
}
