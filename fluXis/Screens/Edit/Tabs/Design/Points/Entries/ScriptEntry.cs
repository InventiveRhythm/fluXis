using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Scripting;
using fluXis.Utils;
using Newtonsoft.Json.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries;

public partial class ScriptEntry : PointListEntry
{
    protected override string Text => "Script";
    protected override Colour4 Color => FluXisColors.Script;

    private ScriptEvent script => (ScriptEvent)Object;

    [Resolved]
    private ScriptStorage scripts { get; set; }

    private Lazy<ScriptStorage.Script> compiled { get; }

    public ScriptEntry(ITimedObject obj)
        : base(obj)
    {
        compiled = new Lazy<ScriptStorage.Script>(() => scripts.Scripts.FirstOrDefault(x => x.Path.EqualsLower(script.ScriptPath)));
    }

    public override ITimedObject CreateClone() => script.JsonCopy();

    protected override Drawable[] CreateValueContent()
    {
        var text = $"{script.ScriptPath}";

        var scr = compiled.Value;

        if (scr is not null)
        {
            var args = scr.Parameters.Select(parameter => script.TryGetParameter<object>(parameter.Key, out var v) ? v : null).ToList();
            text += $"({string.Join(", ", args)})";
        }

        return new FluXisSpriteText
        {
            Text = text,
            Colour = Color
        }.Yield().ToArray<Drawable>();
    }

    protected override IEnumerable<Drawable> CreateSettings()
    {
        var settings = base.CreateSettings().Concat(new Drawable[]
        {
            new PointSettingsTextBox
            {
                Text = "Path",
                DefaultText = script.ScriptPath,
                OnTextChanged = t => script.ScriptPath = t.Text,
                OnCommit = _ =>
                {
                    RequestClose?.Invoke();
                    Map.Update(script);
                    OpenSettings();
                }
            }
        }).ToList();

        var scr = compiled.Value;
        if (scr is null) return settings;

        script.Parameters ??= new Dictionary<string, JToken>();

        foreach (var parameter in scr.Parameters)
        {
            if (parameter.Type == typeof(string))
            {
                settings.Add(new PointSettingsTextBox
                {
                    Text = parameter.Title,
                    DefaultText = script.TryGetParameter<string>(parameter.Key, out var v) ? v : "",
                    OnTextChanged = t =>
                    {
                        script.Parameters[parameter.Key] = t.Text;
                        Map.Update(script);
                    }
                });
            }
        }

        return settings;
    }

    private void get() { }
}
