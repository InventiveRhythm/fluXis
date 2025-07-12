using System;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Mods;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Select.Mods;

public partial class ModCategory : CompositeDrawable
{
    public int ModCount => entries.Length;

    private Container header { get; }
    private ModEntry[] entries { get; }

    public ModCategory(ModsOverlay selector, LocalisableString label, Colour4 accent, IMod[] mods)
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        AlwaysPresent = true;

        InternalChild = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(8),
            ChildrenEnumerable = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 32,
                    Child = header = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        CornerRadius = 4,
                        Masking = true,
                        Shear = new Vector2(.2f, 0),
                        Alpha = 0,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = accent
                            },
                            new Box
                            {
                                Width = 100,
                                RelativeSizeAxes = Axes.Y,
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                Colour = FluXisColors.Background2,
                                Alpha = .5f
                            },
                            new FluXisSpriteText
                            {
                                Text = label,
                                Colour = FluXisColors.TextDark,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Shear = new Vector2(-.2f, 0),
                                WebFontSize = 16,
                                X = 12
                            }
                        }
                    }
                }
            }.Concat(entries = mods.Select(m => new ModEntry(m, selector)).ToArray())
        };
    }

    public void Show(double delay)
    {
        header.FadeOut();
        entries.ForEach(x => x.FadeOut());

        // doing a normal BeginDelayedSequence() does not work
        // i've sat here for 30 minutes trying to get it to work
        Scheduler.AddDelayed(() =>
        {
            header.MoveToX(50).MoveToX(0, 400, Easing.OutQuint).FadeInFromZero(200);

            for (var i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                using (BeginDelayedSequence(ModsOverlay.STAGGER_DURATION * (i + 1)))
                    entry.Show();
            }
        }, delay);
    }

    public void Hide(double delay)
    {
        Scheduler.AddDelayed(() =>
        {
            header.MoveToX(-50, 400, Easing.OutQuint).FadeOut(200);

            for (var i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                using (BeginDelayedSequence(ModsOverlay.STAGGER_DURATION * (i + 1)))
                    entry.Hide();
            }
        }, delay);
    }

    protected override bool OnClick(ClickEvent e)
    {
        return true;
    }
}
