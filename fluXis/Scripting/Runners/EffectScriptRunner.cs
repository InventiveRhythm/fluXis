using System;
using System.Collections.Generic;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Scripting.Runners;

public class EffectScriptRunner : ScriptRunner
{
    private ScriptStorage.Script script { get; }

    public Action<FlashEvent> AddFlash;

    public EffectScriptRunner(ScriptStorage.Script script)
    {
        this.script = script;

        Run(script.Content);

        AddFunction("CreateFlash", (float time, float duration, string colStart, float opStart, string colEnd, float opEnd, string ease, bool background) =>
        {
            var flash = new FlashEvent
            {
                Time = time,
                Duration = duration,
                StartOpacity = Math.Clamp(opStart, 0, 1),
                EndOpacity = Math.Clamp(opEnd, 0, 1),
                InBackground = background
            };

            if (Colour4.TryParseHex(colStart, out var cs))
                flash.StartColor = cs;
            if (Colour4.TryParseHex(colEnd, out var ce))
                flash.EndColor = ce;

            if (Enum.TryParse<Easing>(ease, out var e))
                flash.Easing = e;

            AddFlash?.Invoke(flash);
        });
    }

    public void Handle(ScriptEvent ev)
    {
        var func = GetFunction("ProcessEvent") ?? throw new Exception("No ProcessEvent() function found.");

        var args = new List<object> { ev.Time };

        foreach (var parameter in script.Parameters)
            args.Add(ev.TryGetParameter<object>(parameter.Key, out var v) ? v : null);

        func.Call(args.ToArray());
    }
}
