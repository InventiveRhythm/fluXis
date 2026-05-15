using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using fluXis.Audio;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Screens.Edit.Tabs.Setup.Entries;
using fluXis.Utils.Attributes;
using osu.Framework.Allocation;
using osu.Framework.Extensions.TypeExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;
using Container = osu.Framework.Graphics.Containers.Container;

namespace fluXis.Graphics.UserInterface.Panel.Presets;

public partial class FormPanel<T> : Panel, ICloseable
    where T : class
{
    private const float header_height = 28;

    protected Container BottomContainer { get; }

    public FormPanel(IconUsage icon, LocalisableString title, T data, Func<FormPanel<T>, T, bool> callback, LocalisableString? performText = null)
    {
        performText ??= "Submit";

        Width = 620;
        AutoSizeAxes = Axes.Y;

        Content.RelativeSizeAxes = Axes.X;
        Content.AutoSizeAxes = Axes.Y;

        Content.Child = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Spacing = new Vector2(16),
            Direction = FillDirection.Vertical,
            Children = new Drawable[]
            {
                new GridContainer
                {
                    RelativeSizeAxes = Axes.X,
                    Height = header_height,
                    ColumnDimensions = new Dimension[]
                    {
                        new(GridSizeMode.Absolute, header_height),
                        new(GridSizeMode.Absolute, 12),
                        new(),
                        new(GridSizeMode.Absolute, 12),
                        new(GridSizeMode.Absolute, header_height)
                    },
                    Content = new[]
                    {
                        new[]
                        {
                            new FluXisSpriteIcon
                            {
                                Icon = icon,
                                Size = new Vector2(20),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre
                            },
                            Empty(),
                            new TruncatingText
                            {
                                Text = title,
                                WebFontSize = 20,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                RelativeSizeAxes = Axes.X
                            },
                            Empty(),
                            new CloseButton { Action = Close }
                        }
                    }
                },
            }.Concat(createInputs(data)).Concat([
                BottomContainer = new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Child = new FluXisButton
                    {
                        Size = new Vector2(200, 40),
                        Text = performText.Value,
                        FontSize = FluXisSpriteText.GetWebFontSize(16),
                        Color = Theme.Highlight,
                        TextColor = Theme.TextDark,
                        Action = () =>
                        {
                            if (callback.Invoke(this, data)) Close();
                        },
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight
                    }
                }
            ]).ToArray()
        };
    }

    private IEnumerable<Drawable> createInputs(T data)
    {
        var dataType = typeof(T);
        var props = dataType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var groupId = string.Empty;
        var groupItems = new Dictionary<PropertyInfo, GroupItem>();

        foreach (var prop in props)
        {
            if (prop.GetCustomAttribute<HiddenAttribute>()?.Hide ?? false)
                continue;

            var groupAttr = prop.GetCustomAttribute<GroupAttribute>();
            var group = groupAttr?.Group ?? string.Empty;

            if (group != groupId)
            {
                if (string.IsNullOrWhiteSpace(groupId))
                    groupId = group;
                else
                {
                    yield return createGroup(groupItems);

                    groupId = group;
                    groupItems.Clear();
                }
            }

            if (group == groupId && !string.IsNullOrEmpty(groupId))
            {
                groupItems.Add(prop, new GroupItem(createDrawable(prop, data), groupAttr));
                continue;
            }

            yield return createDrawable(prop, data);
        }

        if (!string.IsNullOrEmpty(groupId) && groupItems.Count > 0)
            yield return createGroup(groupItems);
    }

    private static Drawable createGroup(Dictionary<PropertyInfo, GroupItem> dict)
    {
        if (dict.Count == 0)
            return Empty();

        var dims = new List<Dimension>();
        var draws = new List<Drawable>();

        foreach (var (prop, item) in dict)
        {
            var attr = item.Attribute;
            dims.Add(new Dimension(attr.SizeMode, attr.Size, attr.MinSize, attr.MaxSize));

            item.Drawable.RelativeSizeAxes = attr.RelativeHeight ? Axes.Both : Axes.X;

            if (attr.SizeMode == GridSizeMode.Absolute && attr.Aspect > 0)
                item.Drawable.Height = attr.Size / attr.Aspect;

            draws.Add(item.Drawable);

            // spacing
            dims.Add(new Dimension(GridSizeMode.Absolute, 16));
            draws.Add(Empty());
        }

        // remove last spacing
        dims.RemoveAt(dims.Count - 1);
        draws.RemoveAt(draws.Count - 1);

        return new GridContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            ColumnDimensions = dims.ToArray(),
            RowDimensions = [new(GridSizeMode.AutoSize)],
            Content = new[] { draws.ToArray() }
        };
    }

    private static Drawable createDrawable(PropertyInfo prop, T data)
    {
        var type = prop.PropertyType;

        if (type.IsNullable())
            type = type.GetUnderlyingNullableType();

        var name = prop.GetCustomAttribute<DescriptionAttribute>()?.Description ?? prop.Name;
        var owr = prop.GetCustomAttribute<TypeOverrideAttribute>()?.CustomType;
        var val = prop.GetValue(data);

        if (owr != null)
        {
            switch (owr.Value)
            {
                case TypeOverrideAttribute.Type.Image:
                {
                    if (type != typeof(string))
                        throwInvalidCombo(owr.Value);

                    var selector = new ImageSelector
                    {
                        RelativeSizeAxes = Axes.X,
                        OnFileSelected = (select, file) =>
                        {
                            select.AddSprite(new FileSystemSprite(file.FullName));
                            var bytes = File.ReadAllBytes(file.FullName);
                            var b64 = Convert.ToBase64String(bytes);
                            prop.SetValue(data, b64);
                        }
                    };

                    if (!string.IsNullOrWhiteSpace(val as string))
                        selector.AddSprite(new CustomSprite(val as string));

                    return selector;
                }

                case TypeOverrideAttribute.Type.Color:
                {
                    if (type != typeof(string))
                        throwInvalidCombo(owr.Value);

                    return new SetupColor(name)
                    {
                        Color = Colour4.TryParseHex(val as string ?? "#ffffff", out var c) ? c : Colour4.White,
                        OnColorChanged = v => prop.SetValue(data, v.ToHex())
                    };
                }
            }

            return new FluXisSpriteText { Text = $"could not create input for type {owr} ({name})" };
        }

        if (type.IsEnum)
        {
            var getValues = typeof(Enum)
                            .GetMethods(BindingFlags.Public | BindingFlags.Static)
                            .Where(x => x.Name == nameof(Enum.GetValues))
                            .First(x => x.IsGenericMethod);

            var values = getValues.MakeGenericMethod(type).Invoke(null, []);
            var dropdown = typeof(FluXisDropdown<>).MakeGenericType(type);

            var inst = (Drawable)Activator.CreateInstance(dropdown)!;
            inst.RelativeSizeAxes = Axes.X;

            var items = dropdown.GetProperty(nameof(FluXisDropdown<object>.Items), BindingFlags.Public | BindingFlags.Instance)!;
            items.SetValue(inst, values);

            return inst;
        }

        if (type == typeof(string))
        {
            return new SetupTextBox(name)
            {
                Default = val as string,
                Placeholder = prop.GetCustomAttribute<PlaceholderAttribute>()?.Placeholder ?? string.Empty,
                MaxLength = prop.GetCustomAttribute<MaxLengthAttribute>()?.Length ?? 256,
                ReadOnly = prop.GetCustomAttribute<ReadOnlyAttribute>()?.IsReadOnly ?? false,
                Password = prop.GetCustomAttribute<PasswordPropertyTextAttribute>()?.Password ?? false,
                OnChange = v => prop.SetValue(data, v)
            };
        }

        return new FluXisSpriteText { Text = $"could not create input for type {type} ({name})" };

        void throwInvalidCombo(TypeOverrideAttribute.Type attr)
            => throw new InvalidOperationException($"Custom type '{attr}' can not be represented with '{prop.PropertyType}'.");
    }

    public void Close()
    {
        if (Loading)
            return;

        Hide();
    }

    private class GroupItem
    {
        public Drawable Drawable { get; }
        public GroupAttribute Attribute { get; }

        public GroupItem(Drawable drawable, GroupAttribute attribute)
        {
            Drawable = drawable;
            Attribute = attribute;
        }
    }

    private partial class CloseButton : ClickableContainer
    {
        [Resolved]
        private UISamples samples { get; set; }

        private readonly HoverLayer hover;
        private readonly FlashLayer flash;

        public CloseButton()
        {
            RelativeSizeAxes = Axes.Both;
            Anchor = Origin = Anchor.Centre;
            CornerRadius = 6;
            Masking = true;
            Children = new Drawable[]
            {
                hover = new HoverLayer(),
                flash = new FlashLayer(),
                new FluXisSpriteIcon
                {
                    Icon = FontAwesome6.Solid.XMark,
                    Size = new Vector2(14),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            };
        }

        protected override bool OnHover(HoverEvent e)
        {
            samples.Hover();
            hover.Show();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.Hide();
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            this.ScaleTo(.9f, 1000, Easing.OutQuint);
            return true;
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            this.ScaleTo(1, 1000, Easing.OutElastic);
        }

        protected override bool OnClick(ClickEvent e)
        {
            samples.Click();
            flash.Show();
            return base.OnClick(e);
        }
    }
}
