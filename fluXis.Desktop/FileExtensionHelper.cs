using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using Microsoft.Win32;

namespace fluXis.Desktop;

[SupportedOSPlatform("windows")]
public static class FileExtensionHelper
{
    [System.Runtime.InteropServices.DllImport("Shell32.dll")]
    private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

    private const int shcne_assocchanged = 0x8000000;
    private const int shcnf_flush = 0x1000;

    public static void EnsureAssociationsSet()
    {
        var processModule = Process.GetCurrentProcess().MainModule;
        if (processModule == null) return;

        var filePath = processModule.FileName;

        ensureAssociationsSet(new FileAssociation
        {
            Extension = ".fms",
            ProgId = "fluXis.MapSet",
            FileTypeDescription = "fluXis MapSet",
            ExecutableFilePath = filePath
        });

        ensureAssociationsSet(new FileAssociation
        {
            Extension = ".fsk",
            ProgId = "fluXis.Skin",
            FileTypeDescription = "fluXis Skin",
            ExecutableFilePath = filePath
        });
    }

    private static void ensureAssociationsSet(params FileAssociation[] associations)
    {
        bool madeChanges = associations.Aggregate(false, (current, association) => current | setAssociation(association.Extension, association.ProgId, association.FileTypeDescription, association.ExecutableFilePath));

        if (madeChanges)
            SHChangeNotify(shcne_assocchanged, shcnf_flush, IntPtr.Zero, IntPtr.Zero);
    }

    private static bool setAssociation(string extension, string progId, string fileTypeDescription, string applicationFilePath)
    {
        bool madeChanges = false;
        madeChanges |= setKeyDefaultValue(@"Software\Classes\" + extension, progId);
        madeChanges |= setKeyDefaultValue(@"Software\Classes\" + progId, fileTypeDescription);
        madeChanges |= setKeyDefaultValue($@"Software\Classes\{progId}\shell\open\command", "\"" + applicationFilePath + "\" \"%1\"");
        return madeChanges;
    }

    private static bool setKeyDefaultValue(string keyPath, string value)
    {
        var key = Registry.CurrentUser.CreateSubKey(keyPath);

        if (key == null || key.GetValue(null) as string == value) return false;

        key.SetValue(null, value);
        return true;
    }
}

public class FileAssociation
{
    public string Extension { get; init; }
    public string ProgId { get; init; }
    public string FileTypeDescription { get; init; }
    public string ExecutableFilePath { get; init; }
}
