using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using fluXis.Game;
using osu.Framework.Android;
using Debug = System.Diagnostics.Debug;
using Environment = Android.OS.Environment;
using Path = System.IO.Path;

namespace fluXis.Android;

[Activity(ConfigurationChanges = DEFAULT_CONFIG_CHANGES, Exported = true, LaunchMode = DEFAULT_LAUNCH_MODE, MainLauncher = true)]
[IntentFilter(new[] { "android.intent.action.VIEW" }, Categories = new[] { "android.intent.category.DEFAULT" }, DataScheme = "content", DataPathPattern = ".*\\\\.fms", DataHost = "*", DataMimeType = "*/*")]
[IntentFilter(new[] { "android.intent.action.VIEW" }, Categories = new[] { "android.intent.category.DEFAULT" }, DataScheme = "content", DataMimeType = "application/x-fluXis-mapset-archive")]
public class MainActivity : AndroidGameActivity
{
    public ScreenOrientation DefaultOrientation = ScreenOrientation.Unspecified;

    protected override osu.Framework.Game CreateGame() => game = new FluXisGameAndroid();
    private FluXisGame game { get; set; }

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

    protected override void OnNewIntent(Intent intent)
    {
        Task.Run(() =>
        {
            switch (intent?.Action)
            {
                case Intent.ActionView:
                    // move file to import folder
                    string path = intent.Data?.Path;

                    if (path != null && Environment.DataDirectory != null)
                    {
                        string importPath = Path.Combine(Environment.DataDirectory.AbsolutePath, "import");
                        Directory.CreateDirectory(importPath);
                        string newPath = Path.Combine(importPath, Path.GetFileName(path));
                        File.Copy(path, newPath, true);
                    }

                    break;
            }
        });
    }
}
