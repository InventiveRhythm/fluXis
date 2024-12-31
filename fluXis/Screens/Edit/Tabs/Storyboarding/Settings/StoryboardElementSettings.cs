using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline.Blueprints;
using fluXis.Storyboards;
using fluXis.Utils;
using Newtonsoft.Json.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Storyboarding.Settings;

public partial class StoryboardElementSettings : CompositeDrawable
{
    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private TimelineBlueprintContainer blueprints { get; set; }

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
                Colour = FluXisColors.Background2
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
                    new PointSettingsTime(map, item),
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
                        }
                    },
                    new PointSettingsColor
                    {
                        Text = "Color",
                        Color = Colour4.FromRGBA(item.Color),
                        OnColorChanged = c => item.Color = c.ToRGBA()
                    },
                    new PointSettingsDropdown<StoryboardLayer>
                    {
                        Text = "Layer",
                        CurrentValue = item.Layer,
                        Items = Enum.GetValues<StoryboardLayer>().ToList(),
                        OnValueChanged = l => item.Layer = l
                    },
                    new PointSettingsDropdown<Anchor>
                    {
                        Text = "Anchor",
                        CurrentValue = item.Anchor,
                        Items = validAnchors.ToList(),
                        OnValueChanged = a => item.Anchor = a
                    },
                    new PointSettingsDropdown<Anchor>
                    {
                        Text = "Origin",
                        CurrentValue = item.Origin,
                        Items = validAnchors.ToList(),
                        OnValueChanged = o => item.Origin = o
                    }
                };

                switch (item.Type)
                {
                    case StoryboardElementType.Box:
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
                                }
                            },
                        });
                        break;

                    case StoryboardElementType.Sprite:
                        drawables.Add(new PointSettingsTextBox
                        {
                            Text = "Texture",
                            DefaultText = item.Parameters["file"]?.ToString() ?? "",
                            TextBoxWidth = 320,
                            OnTextChanged = t => item.Parameters["file"] = JToken.FromObject(t.Text)
                        });
                        break;

                    case StoryboardElementType.Text:
                        drawables.AddRange(new Drawable[]
                        {
                            new PointSettingsTextBox
                            {
                                Text = "Text",
                                DefaultText = item.Parameters["text"]?.ToString() ?? "",
                                TextBoxWidth = 420,
                                OnTextChanged = t => item.Parameters["text"] = JToken.FromObject(t.Text)
                            },
                            new PointSettingsTextBox
                            {
                                Text = "Font Size",
                                DefaultText = (item.Parameters["size"]?.ToObject<float>() ?? 20).ToStringInvariant(),
                                OnTextChanged = box =>
                                {
                                    if (box.Text.TryParseFloatInvariant(out var result) && result >= 1)
                                        item.Parameters["size"] = JToken.FromObject(result);
                                    else
                                        box.NotifyError();
                                }
                            },
                        });
                        break;
                }

                flow.AddRange(drawables);
                break;

            case 2:
                flow.Add(new FluXisSpriteText
                {
                    Text = "More than 1 element selected.",
                    WebFontSize = 16
                });
                break;
        }
    }
}
