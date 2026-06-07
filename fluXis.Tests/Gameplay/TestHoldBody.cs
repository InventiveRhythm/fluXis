using System;
using System.Linq;
using fluXis.Graphics.UserInterface.Color;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osu.Framework.Utils;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace fluXis.Tests.Gameplay;

public partial class TestHoldBody : FluXisTestScene
{
    private Path path;
    private Container<Dot> dots;
    private float progress = 1f;

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        Add(path = new CustomPath()
        {
            AutoSizeAxes = Axes.Both,
            PathRadius = 128,
            // CustomTexture = textures.Get("Online/default-banner")
        });

        Add(dots = new Container<Dot>
        {
            RelativeSizeAxes = Axes.Both
        });

        AddStep("add dot", () =>
        {
            dots.Add(new Dot { Moved = redoPoints });
            redoPoints();
        });

        AddSliderStep("progress", 0f, 1f, 1f, v =>
        {
            progress = v;
            redoPoints();
        });

        AddSliderStep("PathRadius", 1, 128, 64, v => path.PathRadius = v);
    }

    private void redoPoints()
    {
        path.ClearVertices();

        var computed = PathApproximator.BezierToPiecewiseLinear(dots.Select(x => x.Position).ToArray());
        if (computed.Count == 0) return;

        var count = Math.Max(1, computed.Count * progress);

        for (var i = 0; i < count; i++)
        {
            path.AddVertex(computed[i]);
        }
    }

    private partial class CustomPath : SmoothPath
    {
        public Texture CustomTexture { set => Texture = value; }

        protected override Color4 ColourAt(float position)
        {
            if (position <= 0.128f)
                return Theme.Highlight;

            return Theme.Background2;
        }
    }

    private partial class Dot : Circle
    {
        [CanBeNull]
        public Action Moved { get; init; }

        public Dot()
        {
            Position = new Vector2(50);
            Size = new Vector2(15);
            Origin = Anchor.Centre;
            Colour = Theme.Highlight;
        }

        protected override bool OnDragStart(DragStartEvent e)
        {
            return e.Button == MouseButton.Left;
        }

        protected override void OnDrag(DragEvent e)
        {
            Position += e.Delta;
            Moved?.Invoke();
        }
    }
}
