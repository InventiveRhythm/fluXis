using System;
using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline.Blueprints;
using fluXis.Scripting;
using fluXis.Storyboards;
using fluXis.Utils;
using fluXis.Utils.Attributes;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Storyboarding.Timeline.Elements;

public partial class TimelineElement : CompositeDrawable
{
    public const int HEIGHT = 32;

    [Resolved]
    private StoryboardTimeline timeline { get; set; }

    [Resolved]
    private ScriptStorage scripts { get; set; }

    public StoryboardElement Element { get; }

    private FluXisSpriteIcon icon;
    private TruncatingText text;

    public TimelineElement(StoryboardElement element)
    {
        Element = element;

        Name = $"{element.Type}";

        if (element.Type == StoryboardElementType.Sprite)
            Name += $" [{element.GetParameter("file", "")}]";
        if (element.Type == StoryboardElementType.Text)
            Name += $" [{element.GetParameter("text", "")}] ";
        if (element.Type == StoryboardElementType.Script)
            Name += $" [{element.GetParameter("path", "")}] ";
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Height = HEIGHT;

        var color = GetColor(Element.Type);
        var textColor = Theme.IsBright(color) ? Theme.TextDark : Theme.Text;

        InternalChildren = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Colour = color,
                CornerRadius = 4,
                Masking = true,
                BorderThickness = 2,
                BorderColour = color.Darken(.3f),
                Child = new Box { RelativeSizeAxes = Axes.Both }
            },
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                ColumnDimensions = new Dimension[]
                {
                    new(GridSizeMode.AutoSize, minSize: HEIGHT),
                    new(),
                    new(GridSizeMode.Absolute, 28)
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        icon = new FluXisSpriteIcon
                        {
                            Size = new Vector2(14),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Icon = Element.Type.GetIcon(),
                            Colour = textColor
                        },
                        text = new TruncatingText
                        {
                            RelativeSizeAxes = Axes.X,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Padding = new MarginPadding { Left = -2, Right = 6 },
                            WebFontSize = 12,
                            Colour = textColor
                        },
                        new DragHandle { Colour = textColor }
                    }
                }
            },
        };
    }

    public void UpdateText()
    {
        if (!string.IsNullOrWhiteSpace(Element.Label))
        {
            text.Text = Element.Label;
            return;
        }

        switch (Element.Type)
        {
            case StoryboardElementType.Box:
                text.Text = Colour4.FromRGBA(Element.Color).ToHex();
                break;

            case StoryboardElementType.Text:
                text.Text = Element.GetParameter("text", "");
                break;

            case StoryboardElementType.Sprite:
                text.Text = Element.GetParameter("file", "");
                break;

            case StoryboardElementType.Script:
            {
                var path = Element.GetParameter("path", "");
                var script = scripts.Scripts.FirstOrDefault(x => x.Path.EqualsLower(path));
                var txt = path;

                if (script is not null)
                {
                    var args = script.Parameters.Select(parameter => Element.GetParameter<object>(parameter.Key, "")).ToList();
                    txt += $" ({string.Join(", ", args)})";
                }

                text.Text = txt;
                break;
            }

            default:
                text.Text = "";
                break;
        }
    }

    public static Colour4 GetColor(StoryboardElementType type) => type switch
    {
        StoryboardElementType.Box or StoryboardElementType.OutlineBox => Theme.Red,
        StoryboardElementType.Sprite => Theme.Orange,
        StoryboardElementType.Text => Theme.Yellow,
        StoryboardElementType.Script => Theme.Lime,
        StoryboardElementType.Circle or StoryboardElementType.OutlineCircle => Theme.Green,
        StoryboardElementType.SkinSprite => Theme.Aqua,
        _ => Theme.Highlight
    };

    protected override void Update()
    {
        base.Update();

        var start = timeline.PositionAtTime(Element.StartTime);
        var end = timeline.PositionAtTime(Element.EndTime);

        Width = end - start;
        X = start;
        Y = timeline.PositionAtZ(Element.ZIndex);

        if (X <= Parent!.DrawWidth && X + Width > 0)
            UpdateText();

        icon.Margin = new MarginPadding { Left = -Math.Min(X - 12, -9), Right = 9 };
    }

    /// <summary>
    /// for the actual dragging see <see cref="TimelineElementBlueprint.BlueprintHandle"/>
    /// </summary>
    private partial class DragHandle : CompositeDrawable
    {
        private Circle handle { get; }

        public DragHandle()
        {
            RelativeSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                handle = new Circle
                {
                    Size = new Vector2(4, 16),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = .6f
                }
            };
        }

        protected override bool OnHover(HoverEvent e)
        {
            handle.FadeTo(1f, 100).ResizeWidthTo(8, 200, Easing.OutQuint);
            return false;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            handle.FadeTo(0f, 600).ResizeWidthTo(4, 400, Easing.OutQuint);
            base.OnHoverLost(e);
        }
    }
}
