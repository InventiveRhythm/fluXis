using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using fluXis.Configuration;
using fluXis.Utils;
using JetBrains.Annotations;
using osu.Framework.Logging;

namespace fluXis.Scripting;

public class ScriptStorage
{
    private readonly List<Script> scripts = new();
    public IReadOnlyList<Script> Scripts => scripts;

    private Dictionary<string, ScriptRunner> runners { get; } = new();

    private string path { get; }

    public ScriptStorage(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"Directory {path} does not exist.");

        this.path = path;
        Reload();
    }

    public void Reload()
    {
        scripts.Clear();
        runners.Clear();

        var files = Directory.GetFiles(path, "*.lua", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var relative = Path.GetRelativePath(path, file);

            try
            {
                var content = File.ReadAllText(file);
                var lines = File.ReadAllLines(file);

                var env = getEnv(lines.FirstOrDefault());
                var script = new Script(env, relative, content);

                var runner = new StorageScriptRunner { DefineParameter = script.AddParam };
                runner.Run(content);

                scripts.Add(script);
            }
            catch (Exception ex)
            {
                ScriptRunner.Logger.Add($"Failed to load script '{relative}': {ex.Message}", LogLevel.Error);
            }
        }
    }

    [CanBeNull]
    public T GetRunner<T>(string file, Func<Script, T> create) where T : ScriptRunner
    {
        if (runners.TryGetValue(path, out var existing))
            return existing as T;

        var script = scripts.FirstOrDefault(x => x.Path.EqualsLower(file));
        if (script is null) return null;

        var runner = create(script);
        runners[file] = runner;
        return runner;
    }

    public bool TryEditExternally(string file, FluXisConfig config, [CanBeNull] [NotNullWhen(false)] out Exception ex)
    {
        ex = null;

        var full = Path.Combine(path, file);

        if (!File.Exists(full))
        {
            ex = new FileNotFoundException();
            return false;
        }

        try
        {
            var template = config.Get<string>(FluXisSetting.ExternalEditorLaunch)
                                 .Replace("%d", $"\"{path}\"")
                                 .Replace("%f", $"\"{full}\"");

            var idx = template.IndexOf(' ');
            var filename = template[..idx];
            var args = template[idx..];

            Process.Start(new ProcessStartInfo
            {
                FileName = filename,
                Arguments = args,
                CreateNoWindow = true,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });

            return true;
        }
        catch (Exception exc)
        {
            ex = exc;
            return false;
        }
    }

    public bool TryCreateNew(string file, Env env, [CanBeNull] [NotNullWhen(false)] out Exception ex)
    {
        ex = null;
        var full = Path.Combine(path, file);

        if (File.Exists(full))
        {
            ex = new Exception("File already exists.");
            return false;
        }

        try
        {
            var sb = new StringBuilder();
            sb.AppendLine($"---@env {(env == Env.Effect ? "effect" : "storyboard")}");
            sb.AppendLine();
            sb.AppendLine("---@param parent StoryboardElement");
            sb.AppendLine("function process(parent)");
            sb.AppendLine("    -- your code here");
            sb.AppendLine("end");

            File.WriteAllText(full, sb.ToString());
            return true;
        }
        catch (Exception exc)
        {
            ex = exc;
            return false;
        }
    }

    private Env getEnv(string line)
    {
        if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("---@env"))
            return Env.None;

        var split = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        if (split.Length < 2) return Env.None;

        var str = split[1].ToLower();

        switch (str)
        {
            case "effect":
                return Env.Effect;

            case "sb":
            case "storyboard":
                return Env.Storyboard;

            default:
                ScriptRunner.Logger.Add($"Unknown environment {str}.");
                return Env.None;
        }
    }

    public class Script
    {
        public Env Environment { get; }
        public string Path { get; }
        public string Content { get; }

        private readonly List<ParameterDefinition> param = new();
        public IReadOnlyList<ParameterDefinition> Parameters => param;

        public Script(Env env, string path, string content)
        {
            Environment = env;
            Path = path;
            Content = content;
        }

        #nullable enable
        public void AddParam(string name, string title, string type, object? fallback)
        {
            try
            {
                if (param.Any(x => x.Key.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                    throw new Exception($"Parameter with name '{name}' already exists.");

                var t = type switch
                {
                    "string" => typeof(string),
                    "int" => typeof(int),
                    "float" => typeof(float),
                    _ => throw new ArgumentOutOfRangeException(nameof(type))
                };

                object defaultFallback = parseDefault(type, fallback?.ToString() ?? "");
                param.Add(new ParameterDefinition(name, title, t, defaultFallback));
            }
            catch (Exception ex)
            {
                ScriptRunner.Logger.Add($"Failed to add parameter '{name}' in '{Path}': {ex.Message}", LogLevel.Error);
            }
        }

        private static object parseDefault(string type, string? defaultFallback = default)
        {
            var isEmpty = string.IsNullOrWhiteSpace(defaultFallback);

            return type switch
            {
                "string" => defaultFallback ?? string.Empty,
                "int" => isEmpty ? 0 : 
                    int.TryParse(defaultFallback, out var intValue) ? intValue : 0,
                "float" => isEmpty ? 0f : 
                    float.TryParse(defaultFallback, out var floatValue) ? floatValue : 0f,
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }
        #nullable disable
    }

    public class ParameterDefinition
    {
        public string Key { get; }
        public string Title { get; }
        public Type Type { get; }
        
        private readonly object defaultFallback;
        
        public T GetDefaultFallback<T>() => (T)defaultFallback ?? default;

        public ParameterDefinition(string key, string title, Type type, object defaultFallback = default)
        {
            Key = key;
            Title = title;
            Type = type;
            this.defaultFallback = defaultFallback;
        }
    }

    public enum Env
    {
        None,
        Effect,
        Storyboard
    }

    private class StorageScriptRunner : ScriptRunner
    {
    }
}
