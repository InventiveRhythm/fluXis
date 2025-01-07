using System;
using fluXis.Integration;
using OpenRGB.NET;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace fluXis.Desktop.Integration;

public partial class OpenRGBController : LightController
{
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
        if (Time.Current - lastUpdate < frameTime) return;

        SetColor(Colour);
        lastUpdate = Time.Current;
    }

    public override void SetColor(Colour4 color)
    {
        if (client is not { Connected: true })
            return;

        if (lastColor.Equals(color))
            return;

        lastColor = color.Opacity(1);

        foreach (var device in devices)
        {
            var colors = new Color[device.Colors.Length];

            for (var i = 0; i < colors.Length; i++)
            {
                var red = (byte)(color.R * 255f);
                var green = (byte)(color.G * 255f);
                var blue = (byte)(color.B * 255f);

                colors[i] = new Color(red, green, blue);
            }

            client.UpdateLeds(device.Index, colors);
        }
    }
}
