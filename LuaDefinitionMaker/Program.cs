using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Xml;
using fluXis;
using fluXis.Map.Structures.Events;
using fluXis.Scripting.Attributes;
using fluXis.Scripting.Models.Storyboarding;
using fluXis.Storyboards;
using osu.Framework.Graphics;

namespace LuaDefinitionMaker;

internal class Program
{
    private static List<LuaType> typeList { get; } = new();

    private static void Main()
    {
        var dir = Directory.GetFiles(".", "*.sln", SearchOption.TopDirectoryOnly);

        if (dir.Length == 0)
        {
            Error("Invalid working directory. Make sure it's set to the fluXis project.");
            return;
        }

        Write("Compiling project...");

        Directory.CreateDirectory("build");
        runCommand("dotnet", "build fluXis -c Release -o build /p:DocumentationFile=build/xmldoc.xml");

        var xml = loadXml("build/xmldoc.xml");
        if (xml is null) return;

        Documentation.Init(xml);

        var assembly = typeof(FluXisGame).Assembly!;
        var files = new Dictionary<string, StringBuilder>();
        var types = assembly.GetTypes();

        foreach (var type in types)
        {
            var attr = type.GetCustomAttribute<LuaDefinitionAttribute>(false);
            if (attr is null) continue;

            var name = attr.Name ?? type.Name.Replace("Lua", "");
            typeList.Add(new BasicType(type, name, attr));
        }

        typeList.Add(new EnumType<Easing>(true, ctorName: "Easing", enumName: "Easing"));
        typeList.Add(new EnumType<Anchor>(true)
        {
            Values = new[]
            {
                Anchor.TopLeft, Anchor.TopCentre, Anchor.TopRight,
                Anchor.CentreLeft, Anchor.Centre, Anchor.CentreRight,
                Anchor.BottomLeft, Anchor.BottomCentre, Anchor.BottomRight
            }
        });

        var eventTypes = LuaMap.GetMapEventTypes();
        eventTypes.Remove(typeof(ScriptEvent));
        eventTypes.Remove(typeof(NoteEvent));

        var eventSb = new StringBuilder();
        eventSb.AppendLine("---@alias EventType string");
        eventTypes.ForEach(x => eventSb.AppendLine($"---| {x.Name.Replace("Event", "")}"));
        typeList.Add(new CustomTextType("enums", eventSb.ToString()));

        // yes this is stupid but i don't want to figure out how to make it right
        typeList.Add(new CustomTextType("enums", "---@alias ParameterDefinitionType string\n---| \"string\"\n---| \"int\"\n---| \"float\""));
        typeList.Add(new EnumType<StoryboardAnimationType>(false, "AnimationType", "storyboard"));
        typeList.Add(new EnumType<SkinSprite>(false, nameof(SkinSprite), "storyboard"));
        typeList.Add(new EnumType<StoryboardLayer>(true, "Layer", "storyboard"));

        typeList.Add(new NamespaceTypes(
            types,
            new[]
            {
                "fluXis.Map.Structures.Events",
                "fluXis.Map.Structures.Events.Playfields",
                "fluXis.Map.Structures.Events.Camera",
                "fluXis.Map.Structures.Events.Scrolling"
            },
            "events"
        ));

        typeList.Add(new NamespaceTypes(
            types,
            "fluXis.Map.Structures",
            "struct"
        ));

        foreach (var type in typeList)
        {
            var filename = type.Attribute.FileName.ToLowerInvariant();

            if (!files.TryGetValue(filename, out var sb))
            {
                sb = new StringBuilder();
                sb.AppendLine("---@meta");
                sb.AppendLine("");
                files[filename] = sb;
            }

            type.Write(sb);
        }

        const string out_dir = "scripting/library";

        try
        {
            if (Directory.Exists(out_dir))
                Directory.Delete(out_dir, true);
        }
        catch { }

        Directory.CreateDirectory(out_dir);

        foreach (var (name, content) in files)
            File.WriteAllText($"{out_dir}/{name}.lua", content.ToString().Trim());
    }

    public static string GetLuaType(Type type, bool enumToNumber = true, Type? fallback = null, bool nullable = false)
    {
        string? name = getLuaTypeName(type);

        if (name is null)
        {
            var t = typeList.FirstOrDefault(x => x.BaseType == type);

            if (t is not null)
                name = (t.BaseType.IsEnum && enumToNumber) ? "number" : t.Name;
        }

        if (name is not null) 
            return nullable ? $"{name}?" : name;

        string fallbackType = getLuaTypeName(fallback, nullable) ?? fallback?.Name ?? type.Name;
        Warn($"Failed to find matching lua type for '{type.FullName}', using '{fallbackType}' as fallback.");
        return fallbackType;
    }

    private static string? getLuaTypeName(Type? type, bool nullable = false)
    {
        string? name = type?.FullName switch
        {
            "System.Single" => "number",
            "System.Double" => "number",
            "System.Int32" => "number",
            "System.Int64" => "number",
            "System.UInt32" => "number",
            "System.UInt64" => "number",
            "fluXis.Storyboards.StoryboardLayer" => "number",
            "fluXis.Map.Structures.Bases.IMapEvent" => "EventType",
            "System.Boolean" => "boolean",
            "System.String" => "string",
            "NLua.LuaTable" => "table",
            "System.Object" => "any",
            _ => null
        };

        return nullable ? $"{name}?" : name;
    }

    private static XmlDocument? loadXml(string file)
    {
        if (!File.Exists(file))
        {
            Error($"File {file} does not exist!");
            return null;
        }

        Write($"Loading XML file: {file}");

        try
        {
            var doc = new XmlDocument();
            doc.Load(file);
            return doc;
        }
        catch (Exception e)
        {
            Error($"Failed to load XML file: {e.Message}");
        }

        return null;
    }

    private static bool runCommand(string command, string args)
    {
        Write($"Running {command} {args}...");

        var psi = new ProcessStartInfo(command, args)
        {
            WorkingDirectory = Environment.CurrentDirectory,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        Process? p = Process.Start(psi);
        if (p == null) return false;

        string output = p.StandardOutput.ReadToEnd();
        output += p.StandardError.ReadToEnd();

        p.WaitForExit();

        if (p.ExitCode == 0) return true;

        Write(output);
        Error($"Command {command} {args} failed!");
        return false;
    }

    public static void Write(string? message, ConsoleColor col = ConsoleColor.Gray, bool line = true)
    {
        Console.ForegroundColor = col;

        if (line) Console.WriteLine(message);
        else Console.Write(message);
    }

    public static void Warn(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"WARNING: {message}");
    }

    public static void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"FATAL ERROR: {message}");
        Environment.Exit(1);
    }
}
