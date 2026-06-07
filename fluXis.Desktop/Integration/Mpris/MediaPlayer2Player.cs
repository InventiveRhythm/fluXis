using System;
using System.Collections.Generic;
using System.IO;
using fluXis.Audio;
using fluXis.Database;
using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Screens;
using fluXis.Screens.Edit;
using fluXis.Screens.Gameplay;
using Midori.DBus;
using Midori.DBus.Attributes;
using Midori.DBus.Values;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;

namespace fluXis.Desktop.Integration.Mpris;

[DBusInterface("org.mpris.MediaPlayer2.Player")]
public partial class MediaPlayer2Player(DBusConnection session) : Component
{
    [Resolved]
    private GlobalClock clock { get; set; }

    [Resolved]
    private MapStore maps { get; set; }

    [Resolved]
    private AudioManager audio { get; set; }

    [Resolved]
    private FluXisScreenStack screens { get; set; }

    [Resolved]
    private FluXisGame game { get; set; }

    [DBusMember]
    public string PlaybackStatus { get; private set; } = "Paused";

    [DBusMember]
    public string LoopStatus => clock.Looping ? "Track" : "None";

    [DBusMember]
    public double Rate => clock.Rate;

    [DBusMember]
    public Dictionary<string, DBusVariantValue> Metadata { get; } = new();

    [DBusMember]
    public double Volume
    {
        get => audio.Volume.Value;
        set => Schedule(() => audio.Volume.Value = Math.Clamp(value, 0, 1));
    }

    [DBusMember]
    public new long Position { get; private set; }

    [DBusMember]
    public double MinimumRate => 1;

    [DBusMember]
    public double MaximumRate => 1;

    [DBusMember]
    public bool CanGoNext => true;

    [DBusMember]
    public bool CanGoPrevious => true;

    [DBusMember]
    public bool CanPlay => true;

    [DBusMember]
    public bool CanPause => true;

    [DBusMember]
    public bool CanSeek => false;

    [DBusMember]
    public bool CanControl => true;

    private RealmMap lastMap;
    private double lastLength;
    private bool lastPlayState;
    private bool lastCanPlayPause;
    private bool lastCanPrevNext;

#nullable enable

    protected override void Update()
    {
        base.Update();

        Dictionary<string, DBusVariantValue>? changes = null;

        try
        {
            var map = maps.CurrentMap;
            var playing = clock.IsRunning;
            var length = clock.CurrentTrack?.Length ?? 0;
            Position = (long)(clock.CurrentTime * 1000);

            switch (screens.CurrentScreen)
            {
                case GameplayScreen gameplay:
                    map = gameplay.RealmMap;
                    playing = !gameplay.IsPaused.Value;
                    length = gameplay.GameplayClock.Track?.Length ?? 100;
                    Position = (long)(gameplay.GameplayClock.CurrentTime * 1000);
                    break;

                case Editor edit:
                    map = edit.EditorMap.RealmMap;
                    playing = edit.EditorClock.IsRunning;
                    length = edit.EditorClock.Track.Value.Length;
                    Position = (long)(edit.EditorClock.CurrentTime * 1000);
                    break;
            }

            if (!ReferenceEquals(lastMap, map))
            {
                Metadata["xesam:title"] = new DBusVariantValue(map.Metadata.SortingTitle);
                Metadata["xesam:artist"] = new DBusVariantValue(map.Metadata.SortingArtist);

                var img = getPath(map.MapSet.Cover);
                if (!File.Exists(img)) img = getPath(map.Metadata.Background);
                Metadata["mpris:artUrl"] = new DBusVariantValue($"file://{img}");

                addChange(nameof(Metadata), Metadata);
                lastMap = map;
            }

            if (lastPlayState != playing)
            {
                PlaybackStatus = playing ? "Playing" : "Paused";
                addChange(nameof(PlaybackStatus), PlaybackStatus);
                lastPlayState = playing;
            }

            if (lastLength != length)
            {
                Metadata["mpris:length"] = new DBusVariantValue((long)(length * 1000));
                addChange(nameof(Metadata), Metadata);
                lastLength = length;
            }

            if (changes is null)
                return;

            var msg = new DBusMessage(DBusEndian.Little, DBusMessageType.Signal, DBusMessageFlags.NoReplyExpected, 1, 1)
            {
                Path = "/org/mpris/MediaPlayer2",
                Interface = "org.freedesktop.DBus.Properties",
                Member = "PropertiesChanged"
            };
            var writer = msg.GetBodyWriter();

            writer.WriteString("org.mpris.MediaPlayer2.Player");
            writer.WriteDictionary(changes);
            writer.WriteArray<string>([]);

            session.QueueMessage(msg);
        }
        catch (Exception e)
        {
        }

        void addChange(string key, object val)
        {
            changes ??= new Dictionary<string, DBusVariantValue>();
            changes[key] = new DBusVariantValue(val);
        }

        string getPath(string img)
        {
            var path = maps.CurrentMapSet.GetPathForFile(img) ?? "/";
            if (!Path.IsPathRooted(path)) path = MapFiles.GetFullPath(path);
            return path;
        }
    }

    [DBusMember]
    public void Previous()
    {
        if (screens.CurrentScreen is not FluXisScreen { AllowMusicControl: true })
            return;

        Schedule(() => game.PreviousSong());
    }

    [DBusMember]
    public void Next()
    {
        if (screens.CurrentScreen is not FluXisScreen { AllowMusicControl: true })
            return;

        Schedule(() => game.NextSong());
    }

    [DBusMember]
    public void Pause()
    {
        if (screens.CurrentScreen is not FluXisScreen { AllowMusicPausing: true })
            return;

        Schedule(() => clock.Stop());
    }

    [DBusMember]
    public void Play()
    {
        if (screens.CurrentScreen is not FluXisScreen { AllowMusicPausing: true })
            return;

        Schedule(() => clock.Start());
    }

    [DBusMember]
    public void PlayPause()
    {
        if (screens.CurrentScreen is not FluXisScreen { AllowMusicPausing: true })
            return;

        Schedule(() =>
        {
            if (clock.IsRunning)
                clock.Stop();
            else
                clock.Start();
        });
    }
}
