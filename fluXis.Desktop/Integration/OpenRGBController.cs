using System;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Integration;
using fluXis.Map.Structures.Events;
using fluXis.Screens;
using fluXis.Screens.Gameplay;
using JetBrains.Annotations;
using OpenRGB.NET;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Logging;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Desktop.Integration;

public partial class OpenRGBController : LightController
{
    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private FluXisScreenStack stack { get; set; }

    private readonly OpenRgbClient client;
    private Colour4 lastColor;
    private Device[] devices;
    private double lastUpdate;

    private const int fps = 30;
    private static double frameTime => 1000d / fps;

    public OpenRGBController()
    {
        try
        {
            client = new OpenRgbClient();
            client.Connect();
            devices = client.GetAllControllerData();

            client.DeviceListUpdated += (_, _) => devices = client.GetAllControllerData();

            Logger.Log("OpenRGBController connected", LoggingTarget.Runtime, LogLevel.Important);
            Colour = Colour4.Black;
        }
        catch (Exception)
        {
            Logger.Log("Error while connecting to OpenRGBController", LoggingTarget.Runtime, LogLevel.Error);
        }
    }

    protected override void Update()
    {
        if (client is not { Connected: true })
            return;

        if (Time.Current - lastUpdate < frameTime) return;

        if (stack?.CurrentScreen is GameplayScreen gameplay)
        {
            var manager = gameplay.RulesetContainer.PlayfieldManager.FirstPlayer.MainPlayfield.ColorManager;
            var greyscale = gameplay.ShaderStack.GetShader(ShaderType.Greyscale)?.Strength ?? 0;
            var hue = gameplay.ShaderStack.GetShader(ShaderType.HueShift)?.Strength ?? 0;

            setColor(
                applyGreyscale(applyHue(manager.Primary, hue), greyscale),
                applyGreyscale(applyHue(manager.Secondary, hue), greyscale)
            );
        }
        else
            setColor(Theme.Primary, Theme.Secondary);

        lastUpdate = Time.Current;
    }

    private static Colour4 applyGreyscale(Colour4 col, float strength)
    {
        var grey = col.R * .299f + col.G * .587f + col.B * .114f;
        return new Colour4(
            (float)Interpolation.Lerp(col.R, grey, strength),
            (float)Interpolation.Lerp(col.G, grey, strength),
            (float)Interpolation.Lerp(col.B, grey, strength),
            1f
        );
    }

    private static Colour4 applyHue(Colour4 col, float deg)
    {
        var hsl = col.ToHSV();
        return Colour4.FromHSV(Math.Abs(hsl.X + deg) % 1f, hsl.Y, hsl.Z, hsl.W);
    }

    private void setColor(Colour4 first, Colour4 second)
    {
        if (client is not { Connected: true })
            return;

        var gradient = ColourInfo.GradientHorizontal(first, second);

        foreach (var device in devices)
        {
            var colors = new Color[device.Colors.Length];

            for (var i = 0; i < colors.Length; i++)
            {
                var interp = gradient.Interpolate(new Vector2(i / (float)(colors.Length - 1))).SRGB;

                var red = (byte)(interp.R * 255f);
                var green = (byte)(interp.G * 255f);
                var blue = (byte)(interp.B * 255f);

                colors[i] = new Color(red, green, blue);
            }

            client.UpdateLeds(device.Index, colors);
        }
    }
}
