using System;
using fluXis.Desktop.Integration.Mpris;
using Midori.DBus;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;

namespace fluXis.Desktop.Integration;

public partial class LinuxMediaAdapter : CompositeComponent
{
    private DBusConnection session;

    [BackgroundDependencyLoader]
    private void load()
    {
        try
        {
            session = new DBusConnection(DBusAddress.Session);
            session.Connect().Wait();

            var player = new MediaPlayer2Player(session);
            session.RegisterInterface(new MediaPlayer2(), "/org/mpris/MediaPlayer2");
            session.RegisterInterface(player, "/org/mpris/MediaPlayer2");
            session.RequestName("org.mpris.MediaPlayer2.fluXis").Wait();
            AddInternal(player);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to create linux media session!");
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        session?.Close();
        session = null!;
    }
}
