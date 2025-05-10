using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public void AddParam(string name, string title, string type)
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

                param.Add(new ParameterDefinition(name, title, t));
            }
            catch (Exception ex)
            {
                ScriptRunner.Logger.Add($"Failed to add parameter '{name}' in '{Path}': {ex.Message}", LogLevel.Error);
            }
        }
    }

    public class ParameterDefinition
    {
        public string Key { get; }
        public string Title { get; }
        public Type Type { get; }

        public ParameterDefinition(string key, string title, Type type)
        {
            Key = key;
            Title = title;
            Type = type;
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
