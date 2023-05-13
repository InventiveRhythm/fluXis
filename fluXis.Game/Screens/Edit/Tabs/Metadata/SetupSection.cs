using System;
using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Edit.Tabs.Metadata;

public partial class SetupSection : FillFlowContainer
{
    public SetupSection(string title)
    {
        Direction = FillDirection.Vertical;
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        Add(new MetadataSectionHeader(title));
    }
}

public partial class MetadataSectionHeader : SpriteText
{
    public MetadataSectionHeader(string text)
    {
        Text = text;
        Font = FluXisFont.Default(32);
    }
}

public partial class SetupTextBox : Container
{
    private readonly FluXisTextBox textBox;

    public string Text
    {
        get => textBox.Text;
        set => textBox.Text = value;
    }

    public Action OnTextChanged { set => textBox.OnTextChanged = value; }

    public SetupTextBox(string name)
    {
        RelativeSizeAxes = Axes.X;
        Height = 40;

        Children = new Drawable[]
        {
            new SpriteText
            {
                Text = name,
                Font = FluXisFont.Default(24),
                Margin = new MarginPadding { Top = 5 },
                Origin = Anchor.TopRight,
                X = 90
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 30,
                Margin = new MarginPadding { Top = 5 },
                Child = textBox = new FluXisTextBox { RelativeSizeAxes = Axes.Both },
                Padding = new MarginPadding { Left = 100 }
            }
        };
    }
}
