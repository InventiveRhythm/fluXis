using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Notifications.Types.Loading;
using osu.Framework.Platform;

namespace fluXis.Game.Import;

public class MapImporter
{
    public virtual string[] FileExtensions { get; } = Array.Empty<string>();
    public virtual string GameName => "Unknown";
    public virtual bool SupportsAutoImport => false;
    public virtual string Color => "#000000";

    /// <summary>
    /// ID of the importer in the database.
    /// </summary>
    public int ID { get; internal set; }

    internal Func<string, MapResourceProvider> ResourceRequest { get; set; }
    internal Storage Storage { get; set; }
    internal FluXisRealm Realm { get; set; }
    internal MapStore MapStore { get; set; }
    internal NotificationManager Notifications { get; set; }

    public virtual void Import(string path) => throw new NotImplementedException();

    public virtual List<RealmMapSet> GetMaps() => new();

    /// <summary>
    /// Used to request the resource provider for the given folder.
    /// </summary>
    /// <param name="folder">
    /// Path to the folder to request the resource provider for.
    /// </param>
    /// <returns>
    /// The requested resource provider.
    /// </returns>
    protected MapResourceProvider GetResourceProvider(string folder) => ResourceRequest(folder);

    /// <summary>
    /// Used to pass the mapset to FluXisImport to add it to the database.
    /// </summary>
    /// <param name="path">
    /// Path to the converted .fms file.
    /// </param>
    /// <param name="notification">
    /// Loading notification to update.
    /// </param>
    protected void FinalizeConversion(string path, LoadingNotificationData notification = null)
    {
        notification ??= CreateNotification();

        new FluXisImport
        {
            MapStatus = ID,
            Notification = notification,
            Realm = Realm,
            MapStore = MapStore,
            Storage = Storage,
            Notifications = Notifications
        }.Import(path);
    }

    /// <summary>
    /// Creates a temporary folder for importing.
    /// Should be used in conjunction with <see cref="CleanUp"/>.
    /// </summary>
    /// <param name="name">
    /// Name of the folder to create.
    /// </param>
    /// <returns>
    /// The filesystem path to the created folder.
    /// </returns>
    protected string CreateTempFolder(string name)
    {
        string path = Path.Combine(Storage.GetFullPath("import"), name);

        if (Directory.Exists(path))
            Directory.Delete(path, true);

        Directory.CreateDirectory(path);
        return path;
    }

    /// <summary>
    /// Deletes the temporary folder used for importing.
    /// </summary>
    /// <param name="folder">
    /// The filesystem path to the folder to delete.
    /// </param>
    protected static void CleanUp(string folder)
    {
        if (Directory.Exists(folder))
            Directory.Delete(folder, true);
    }

    /// <summary>
    /// Creates a .fms file from the given folder.
    /// </summary>
    /// <param name="name">
    /// Name of the .fms file.
    /// </param>
    /// <param name="folder">
    /// Path to the folder to zip.
    /// </param>
    /// <returns>
    /// The filesystem path to the created .fms file.
    /// </returns>
    protected string CreatePackage(string name, string folder)
    {
        string path = Path.Combine(Storage.GetFullPath("import"), name + ".fms");

        var pack = ZipFile.Open(path, ZipArchiveMode.Create);

        foreach (var file in Directory.GetFiles(folder))
            pack.CreateEntryFromFile(file, Path.GetFileName(file));

        pack.Dispose();
        return path;
    }

    /// <summary>
    /// Creates a loading notification for the importer.
    /// </summary>
    /// <returns>
    /// The created notification.
    /// </returns>
    protected LoadingNotificationData CreateNotification()
    {
        var notification = new LoadingNotificationData
        {
            TextLoading = $"Importing {GameName} map...",
            TextSuccess = $"Imported {GameName} map!",
            TextFailure = $"Failed to import {GameName} map!"
        };

        Notifications.Add(notification);
        return notification;
    }

    /// <summary>
    /// Gets the SHA256 hash of the given file.
    /// </summary>
    /// <param name="entry">
    /// The file to hash.
    /// </param>
    /// <returns>
    /// The SHA256 hash of the file.
    /// </returns>
    protected static string GetHash(ZipArchiveEntry entry)
    {
        var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(entry.Open());
        var hashString = BitConverter.ToString(hashBytes);

        return hashString.Replace("-", "").ToLower();
    }

    /// <summary>
    /// Copies the given file to the given folder.
    /// </summary>
    /// <param name="entry">
    /// The file to copy.
    /// </param>
    /// <param name="folder">
    /// The folder to copy the file to.
    /// </param>
    protected static void CopyFile(ZipArchiveEntry entry, string folder)
    {
        string destPath = Path.Combine(folder, entry.FullName);
        entry.ExtractToFile(destPath, true);
    }

    /// <summary>
    /// Copies the given file to the given folder.
    /// </summary>
    /// <param name="path">
    /// The file to copy.
    /// </param>
    /// <param name="folder">
    /// The folder to copy the file to.
    /// </param>
    protected static void CopyFile(string path, string folder)
    {
        string destPath = Path.Combine(folder, Path.GetFileName(path));
        File.Copy(path, destPath, true);
    }

    /// <summary>
    /// Writes the given content to the given file.
    /// </summary>
    /// <param name="content">
    /// The content to write.
    /// </param>
    /// <param name="folder">
    /// The folder to write the file to.
    /// </param>
    /// <param name="name">
    /// The name of the file to write.
    /// </param>
    protected static void WriteFile(string content, string folder, string name)
    {
        string filePath = Path.Combine(folder, name);
        File.WriteAllText(filePath, content);
    }
}
