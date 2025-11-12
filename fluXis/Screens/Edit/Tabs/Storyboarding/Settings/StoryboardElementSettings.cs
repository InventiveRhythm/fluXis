using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Buttons.Presets;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Types;
using fluXis.Localization;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline.Blueprints;
using fluXis.Scripting;
using fluXis.Storyboards;
using fluXis.Utils;
using JetBrains.Annotations;
using osu.Framework.Allocation;
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
                var drawables = new List<Drawable>
                {
                    new FluXisSpriteText
                    {
                        Text = $"{item.Type}",
                        WebFontSize = 20
                    },
                    new PointSettingsTime(map, item)
                    {
                        TimeChanged = (oldTime, newTime) =>
                            item.EndTime -= oldTime - newTime
                    },
                    new PointSettingsTextBox
                    {
                        Text = "Start X",
                        DefaultText = item.StartX.ToStringInvariant(),
                        OnTextChanged = box =>
                        {
                            if (box.Text.TryParseFloatInvariant(out var result))
                                item.StartX = result;
                            else
                                box.NotifyError();

                            map.Update(item);
                        }
                    },
                    new PointSettingsTextBox
                    {
                        Text = "Start Y",
                        DefaultText = item.StartY.ToStringInvariant(),
                        OnTextChanged = box =>
                        {
                            if (box.Text.TryParseFloatInvariant(out var result))
                                item.StartY = result;
                            else
                                box.NotifyError();

                            map.Update(item);
                        }
                    },
                    new PointSettingsColor
                    {
                        Text = "Color",
                        Color = Colour4.FromRGBA(item.Color),
                        OnColorChanged = c =>
                        {
                            item.Color = c.ToRGBA();
                            map.Update(item);
                        }
                    },
                    new PointSettingsDropdown<StoryboardLayer>
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
                    new PointSettingsDropdown<Anchor>
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
                    new PointSettingsDropdown<Anchor>
                    {
                        Text = "Origin",
                        CurrentValue = item.Origin,
                        Items = validAnchors.ToList(),
                        OnValueChanged = o =>
                        {
                            item.Origin = o;
                            map.Update(item);
                        }
                    }
                };

                switch (item.Type)
                {
                    case StoryboardElementType.Box:
                    case StoryboardElementType.Circle:
                    case StoryboardElementType.OutlineCircle:
                        drawables.AddRange(new Drawable[]
                        {
                            new PointSettingsTextBox
                            {
                                Text = "Width",
                                DefaultText = item.Width.ToStringInvariant(),
                                OnTextChanged = box =>
                                {
                                    if (box.Text.TryParseFloatInvariant(out var result) && result >= 0)
                                        item.Width = result;
                                    else
                                        box.NotifyError();

                                    map.Update(item);
                                }
                            },
                            new PointSettingsTextBox
                            {
                                Text = "Height",
                                DefaultText = item.Height.ToStringInvariant(),
                                OnTextChanged = box =>
                                {
                                    if (box.Text.TryParseFloatInvariant(out var result) && result >= 0)
                                        item.Height = result;
                                    else
                                        box.NotifyError();

                                    map.Update(item);
                                }
                            },
                        });

                        if (item.Type == StoryboardElementType.OutlineCircle)
                        {
                            drawables.Add(new PointSettingsTextBox
                            {
                                Text = "Border Width",
                                DefaultText = item.GetParameter("border", 4f).ToStringInvariant(),
                                OnTextChanged = box =>
                                {
                                    if (box.Text.TryParseFloatInvariant(out var result) && result >= 0)
                                        item.Parameters["border"] = result;
                                    else
                                        box.NotifyError();

                                    map.Update(item);
                                }
                            });
                        }

                        break;

                    case StoryboardElementType.Sprite:
                        drawables.Add(new PointSettingsTextBox
                        {
                            Text = "Texture",
                            DefaultText = item.GetParameter("file", ""),
                            TextBoxWidth = 320,
                            OnTextChanged = t =>
                            {
                                item.Parameters["file"] = t.Text;
                                map.Update(item);
                            }
                        });
                        break;

                    case StoryboardElementType.Text:
                        drawables.AddRange(new Drawable[]
                        {
                            new PointSettingsTextBox
                            {
                                Text = "Text",
                                DefaultText = item.GetParameter("text", ""),
                                TextBoxWidth = 420,
                                OnTextChanged = t =>
                                {
                                    item.Parameters["text"] = t.Text;
                                    map.Update(item);
                                }
                            },
                            new PointSettingsTextBox
                            {
                                Text = "Font Size",
                                DefaultText = item.GetParameter("size", 20f).ToStringInvariant(),
                                OnTextChanged = box =>
                                {
                                    if (box.Text.TryParseFloatInvariant(out var result) && result >= 1)
                                        item.Parameters["size"] = result;
                                    else
                                        box.NotifyError();

                                    map.Update(item);
                                }
                            },
                        });
                        break;

                    case StoryboardElementType.Script:
                        var path = item.GetParameter("path", "");
                        PointSettingsScript box = null!;

                        box = new PointSettingsScript(path)
                        {
                            EditExternally = () =>
                            {
                                if (!scripts.TryEditExternally(item.GetParameter("path", ""), out var ex))
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
                            OnTextChanged = t =>
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
                                drawables.Add(new PointSettingsTextBox
                                {
                                    Text = parameter.Title,
                                    DefaultText = item.GetParameter(parameter.Key, ""),
                                    OnTextChanged = t =>
                                    {
                                        item.Parameters[parameter.Key] = t.Text;
                                        map.Update(item);
                                    }
                                });
                            }
                            else if (parameter.Type == typeof(int))
                            {
                                drawables.Add(new PointSettingsTextBox
                                {
                                    Text = parameter.Title,
                                    DefaultText = item.GetParameter(parameter.Key, 0).ToString(),
                                    OnTextChanged = box =>
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
                                drawables.Add(new PointSettingsTextBox
                                {
                                    Text = parameter.Title,
                                    DefaultText = item.GetParameter(parameter.Key, 0f).ToStringInvariant(),
                                    OnTextChanged = box =>
                                    {
                                        if (box.Text.TryParseFloatInvariant(out var result))
                                            item.Parameters[parameter.Key] = result;
                                        else
                                            box.NotifyError();

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
