using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using fluXis.Configuration;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Buttons.Presets;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Types;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Localization;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline.Blueprints;
using fluXis.Screens.Edit.UI.Variable;
using fluXis.Screens.Edit.UI.Variable.Preset;
using fluXis.Scripting;
using fluXis.Storyboards;
using fluXis.Utils;
using fluXis.Utils.Attributes;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Logging;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Storyboarding.Settings;

public partial class StoryboardElementSettings : CompositeDrawable
{
    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private TimelineBlueprintContainer blueprints { get; set; }

    [Resolved]
    private ScriptStorage scripts { get; set; }

    [Resolved]
    private FluXisConfig config { get; set; }

    [Resolved]
    private PanelContainer panels { get; set; }

    private Anchor[] validAnchors { get; } =
    {
        Anchor.TopLeft,
        Anchor.TopCentre,
        Anchor.TopRight,
        Anchor.CentreLeft,
        Anchor.Centre,
        Anchor.CentreRight,
        Anchor.BottomLeft,
        Anchor.BottomCentre,
        Anchor.BottomRight
    };

    private FillFlowContainer flow;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background2
            },
            new FluXisScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = flow = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding(20),
                    Spacing = new Vector2(20)
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        blueprints.SelectionHandler.SelectedObjects.BindCollectionChanged(collectionChanged, true);
        map.ScriptChanged += onScriptChanged;
    }

    private void onScriptChanged(string fileName)
    {
        var collection = blueprints.SelectionHandler.SelectedObjects;

        if (collection.Count != 1)
            return;

        var item = collection.First();

        if (item.Type != StoryboardElementType.Script)
            return;

        var currentPath = item.GetParameter("path", "");
        var scriptFileName = Path.GetFileName(currentPath);

        if (fileName == scriptFileName)
            Schedule(() => collectionChanged(null, null));
    }

    private void collectionChanged(object _, NotifyCollectionChangedEventArgs __)
    {
        var collection = blueprints.SelectionHandler.SelectedObjects;
        flow.Clear();

        switch (collection.Count)
        {
            case 0:
                flow.Add(new FluXisSpriteText
                {
                    Text = "Nothing selected.",
                    WebFontSize = 16
                });
                break;

            case 1:
                var item = collection.First();
                var blendingEnabled = new BindableBool(item.Blending);

                var title = new FluXisTextBox
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 32,
                    PlaceholderText = item.Type.ToString(),
                    SidePadding = 10,
                    TextContainerHeight = .7f,
                    CommitOnFocusLost = true,
                    BackgroundInactive = Theme.Background3,
                    BackgroundActive = Theme.Background4,
                    Text = item.Label
                };

                title.OnTextChanged += () => item.Label = title.Text;

                var drawables = new List<Drawable>
                {
                    title,
                    new EditorVariableTime(map, item)
                    {
                        TimeChanged = (oldTime, newTime) =>
                        {
                            var timeDelta = newTime - oldTime;
                            item.EndTime += timeDelta;
                            item.Animations.ForEach(anim => anim.StartTime += timeDelta);
                        }
                    },
                    new EditorVariableNumber<float>
                    {
                        Text = "Start X",
                        CurrentValue = item.StartX,
                        Step = 10,
                        OnValueChanged = v =>
                        {
                            item.StartX = v;
                            map.Update(item);
                        }
                    },
                    new EditorVariableNumber<float>
                    {
                        Text = "Start Y",
                        CurrentValue = item.StartY,
                        Step = 10,
                        OnValueChanged = v =>
                        {
                            item.StartY = v;
                            map.Update(item);
                        }
                    },
                    new EditorVariableColor
                    {
                        Text = "Color",
                        CurrentValue = Colour4.FromRGBA(item.Color),
                        OnValueChanged = c =>
                        {
                            item.Color = c.ToRGBA();
                            map.Update(item);
                        }
                    },
                    new EditorVariableDropdown<StoryboardLayer>
                    {
                        Text = "Layer",
                        CurrentValue = item.Layer,
                        Items = Enum.GetValues<StoryboardLayer>().ToList(),
                        OnValueChanged = l =>
                        {
                            item.Layer = l;
                            map.Update(item);
                        }
                    },
                    new EditorVariableDropdown<Anchor>
                    {
                        Text = "Anchor",
                        CurrentValue = item.Anchor,
                        Items = validAnchors.ToList(),
                        OnValueChanged = a =>
                        {
                            item.Anchor = a;
                            map.Update(item);
                        }
                    },
                    new EditorVariableDropdown<Anchor>
                    {
                        Text = "Origin",
                        CurrentValue = item.Origin,
                        Items = validAnchors.ToList(),
                        OnValueChanged = o =>
                        {
                            item.Origin = o;
                            map.Update(item);
                        }
                    },
                    new EditorVariableToggle
                    {
                        Text = "Blend",
                        CurrentValue = item.Blending,
                        OnValueChanged = enabled =>
                        {
                            blendingEnabled.Value = enabled;
                            item.Blending = enabled;
                            map.Update(item);
                        }
                    },
                    new EditorVariableDropdown<DefaultBlendingParameters>
                    {
                        Text = "Blend Mode",
                        CurrentValue = item.BlendingMode,
                        Items = Enum.GetValues<DefaultBlendingParameters>().ToList(),
                        Enabled = blendingEnabled,
                        HideWhenDisabled = true,
                        OnValueChanged = mode =>
                        {
                            item.BlendingMode = mode;
                            map.Update(item);
                        }
                    }
                };

                if (item.Type.HasAttribute<StoryboardElementType, WidthHeightAttribute>())
                {
                    drawables.AddRange(new Drawable[]
                    {
                        new EditorVariableNumber<float>
                        {
                            Text = "Width",
                            CurrentValue = item.Width,
                            Step = 10,
                            OnValueChanged = v =>
                            {
                                item.Width = v;
                                map.Update(item);
                            }
                        },
                        new EditorVariableNumber<float>
                        {
                            Text = "Height",
                            CurrentValue = item.Height,
                            Step = 10,
                            OnValueChanged = v =>
                            {
                                item.Height = v;
                                map.Update(item);
                            }
                        },
                    });
                }

                switch (item.Type)
                {
                    case StoryboardElementType.OutlineCircle:
                    case StoryboardElementType.OutlineBox:
                        drawables.Add(new EditorVariableNumber<float>
                        {
                            Text = "Border Width",
                            CurrentValue = item.GetParameter("border", 4f),
                            Step = 1,
                            OnValueChanged = v =>
                            {
                                item.Parameters["border"] = v;
                                map.Update(item);
                            }
                        });
                        break;

                    case StoryboardElementType.Sprite:
                        drawables.Add(new EditorVariableTextBox
                        {
                            Text = "Texture",
                            CurrentValue = item.GetParameter("file", ""),
                            TextBoxWidth = 280,
                            OnValueChanged = t =>
                            {
                                item.Parameters["file"] = t.Text;
                                map.Update(item);
                            }
                        });
                        break;

                    case StoryboardElementType.Text:
                        drawables.AddRange(new Drawable[]
                        {
                            new EditorVariableTextBox
                            {
                                Text = "Text",
                                CurrentValue = item.GetParameter("text", ""),
                                TextBoxWidth = 420,
                                OnValueChanged = t =>
                                {
                                    item.Parameters["text"] = t.Text;
                                    map.Update(item);
                                }
                            },
                            new EditorVariableNumber<float>
                            {
                                Text = "Font Size",
                                CurrentValue = item.GetParameter("size", 20f),
                                Min = 0,
                                Step = 1,
                                OnValueChanged = v =>
                                {
                                    item.Parameters["size"] = v;
                                    map.Update(item);
                                }
                            },
                        });
                        break;

                    case StoryboardElementType.SkinSprite:
                        drawables.AddRange(new Drawable[]
                        {
                            new EditorVariableDropdown<SkinSprite>
                            {
                                Text = "Sprite",
                                CurrentValue = item.GetParameter("sprite", SkinSprite.HitObject),
                                Items = Enum.GetValues<SkinSprite>().ToList(),
                                OnValueChanged = l =>
                                {
                                    item.Parameters["sprite"] = (int)l;
                                    map.Update(item);
                                }
                            },
                            new EditorVariableNumber<int>
                            {
                                Text = "Lane",
                                CurrentValue = item.GetParameter("lane", 0),
                                Step = 1,
                                Min = 0,
                                Max = 8,
                                OnValueChanged = v =>
                                {
                                    item.Parameters["lane"] = v;
                                    map.Update(item);
                                }
                            },
                            new EditorVariableNumber<int>
                            {
                                Text = "Key Count",
                                CurrentValue = item.GetParameter("keycount", 0),
                                Step = 1,
                                Min = 0,
                                Max = 8,
                                OnValueChanged = v =>
                                {
                                    item.Parameters["keycount"] = v;
                                    map.Update(item);
                                }
                            }
                        });
                        break;

                    case StoryboardElementType.Script:
                        var path = item.GetParameter("path", "");
                        EditorVariableScript box = null!;

                        box = new EditorVariableScript(path)
                        {
                            EditExternally = () =>
                            {
                                if (!scripts.TryEditExternally(item.GetParameter("path", ""), config, out var ex))
                                {
                                    if (ex is FileNotFoundException)
                                    {
                                        panels.Content = new ButtonPanel
                                        {
                                            Text = "This file does not exist.",
                                            SubText = "Do you want to create it?",
                                            Icon = FontAwesome6.Solid.File,
                                            Buttons = new ButtonData[]
                                            {
                                                new PrimaryButtonData(LocalizationStrings.General.PanelGenericConfirm, () =>
                                                {
                                                    // ReSharper disable once AccessToModifiedClosure
                                                    if (!scripts.TryCreateNew(item.GetParameter("path", ""), ScriptStorage.Env.Storyboard, out var ex2))
                                                        showError("Failed to create file!", ex2);
                                                    else
                                                        box?.EditExternally?.Invoke();
                                                }),
                                                new CancelButtonData()
                                            }
                                        };

                                        return;
                                    }

                                    Logger.Error(ex, "Failed to open script externally.");
                                    showError("Failed to open!", ex);
                                }

                                void showError(string text, [CanBeNull] Exception e)
                                {
                                    panels.Content = new SingleButtonPanel(
                                        FontAwesome6.Solid.ExclamationTriangle,
                                        text,
                                        e?.Message ?? "Unknown error"
                                    );
                                }
                            },
                            OnValueChanged = t =>
                            {
                                item.Parameters["path"] = t.Text;
                                map.Update(item);
                            },
                            OnCommit = _ =>
                            {
                                collectionChanged(null, null);
                                map.Update(item);
                            }
                        };

                        drawables.Add(box);

                        var script = scripts.Scripts.FirstOrDefault(x => x.Path.Replace("\\", "/").EqualsLower(path.Replace("\\", "/")));

                        if (script is null)
                        {
                            box.ErrorText = "Script could not be loaded.";
                            break;
                        }

                        foreach (var parameter in script.Parameters)
                        {
                            if (parameter.Type == typeof(string))
                            {
                                drawables.Add(new EditorVariableTextBox
                                {
                                    Text = parameter.Title,
                                    CurrentValue = item.GetParameter(parameter.Key, parameter.GetDefaultFallback<string>()),
                                    OnValueChanged = t =>
                                    {
                                        item.Parameters[parameter.Key] = t.Text;
                                        map.Update(item);
                                    }
                                });
                            }
                            else if (parameter.Type == typeof(int))
                            {
                                drawables.Add(new EditorVariableTextBox
                                {
                                    Text = parameter.Title,
                                    CurrentValue = item.GetParameter(parameter.Key, parameter.GetDefaultFallback<int>()).ToString(),
                                    OnValueChanged = box =>
                                    {
                                        if (box.Text.TryParseIntInvariant(out var result))
                                            item.Parameters[parameter.Key] = result;
                                        else
                                            box.NotifyError();

                                        map.Update(item);
                                    }
                                });
                            }
                            else if (parameter.Type == typeof(float))
                            {
                                drawables.Add(new EditorVariableTextBox
                                {
                                    Text = parameter.Title,
                                    CurrentValue = item.GetParameter(parameter.Key, parameter.GetDefaultFallback<float>()).ToStringInvariant(),
                                    OnValueChanged = box =>
                                    {
                                        if (box.Text.TryParseFloatInvariant(out var result))
                                            item.Parameters[parameter.Key] = result;
                                        else
                                            box.NotifyError();

                                        map.Update(item);
                                    }
                                });
                            }
                            else if (parameter.Type == typeof(bool))
                            {
                                drawables.Add(new EditorVariableToggle
                                {
                                    Text = parameter.Title,
                                    CurrentValue = item.GetParameter(parameter.Key, parameter.GetDefaultFallback<bool>()),
                                    OnValueChanged = enabled =>
                                    {
                                        item.Parameters[parameter.Key] = enabled;
                                        map.Update(item);
                                    }
                                });
                            }
                        }

                        break;
                }

                flow.AddRange(drawables);
                break;

            case >= 2:
                flow.Add(new FluXisSpriteText
                {
                    Text = "More than 1 element selected.",
                    WebFontSize = 16
                });
                break;
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        map.ScriptChanged -= onScriptChanged;
        base.Dispose(isDisposing);
    }
}
