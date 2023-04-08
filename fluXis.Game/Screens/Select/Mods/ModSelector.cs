using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics;
using fluXis.Game.Mods;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Select.Mods;

public partial class ModSelector : Container
{
    public BindableBool IsOpen = new();

    private readonly FillFlowContainer<ModEntry> mods;
    private readonly SpriteText multiplierText;

    public ModSelector()
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                Colour = Colour4.Black,
                RelativeSizeAxes = Axes.Both,
                Alpha = 0.5f,
            },
            new Container
            {
                Width = 1300,
                AutoSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                CornerRadius = 10,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        Colour = FluXisColors.Background2,
                        RelativeSizeAxes = Axes.Both,
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Padding = new MarginPadding(20),
                        Children = new Drawable[]
                        {
                            new SpriteText
                            {
                                Text = "Gameplay Modifiers",
                                Font = FluXisFont.Default(40),
                                Y = -10
                            },
                            new SpriteText
                            {
                                Text = "Make the game harder or easier for yourself.",
                                Font = FluXisFont.Default(),
                                Margin = new MarginPadding { Top = 25 },
                                Colour = FluXisColors.Text2
                            },
                            multiplierText = new SpriteText
                            {
                                Text = "Score Multiplier: 1.00x",
                                Font = FluXisFont.Default(),
                                Anchor = Anchor.TopRight,
                                Origin = Anchor.CentreRight,
                                Margin = new MarginPadding { Top = 30 },
                                Colour = FluXisColors.Text2
                            },
                            mods = new FillFlowContainer<ModEntry>
                            {
                                Direction = FillDirection.Full,
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Spacing = new Vector2(10),
                                Margin = new MarginPadding { Top = 50 }
                            }
                        }
                    }
                }
            }
        };

        IsOpen.BindValueChanged(_ => updateVisibility());

        addMod(new EasyMod());
        addMod(new HardMod());
        addMod(new NoFailMod());
        addMod(new AutoPlayMod());
        addMod(new FragileMod());
        addMod(new FlawlessMod());
    }

    public void OnModSelected(ModEntry mod)
    {
        foreach (var modIncompatibleMod in mod.Mod.IncompatibleMods)
        {
            var incompatibleMod = mods.Children.FirstOrDefault(m => m.Mod.Acronym == modIncompatibleMod);
            incompatibleMod?.Deselect();
        }

        updateTotalMultiplier();
    }

    public void OnModDeselected(ModEntry mod) => updateTotalMultiplier();

    private void updateTotalMultiplier()
    {
        multiplierText.Text = $"Score Multiplier: {1f + SelectedMods.Sum(mod => mod.ScoreMultiplier - 1f):0.00}x";
    }

    public List<IMod> SelectedMods => mods.Children.Where(mod => mod.Selected).Select(mod => mod.Mod).ToList();

    private void addMod(IMod mod)
    {
        mods.Add(new ModEntry(mod) { ParentSelector = this });
    }

    private void updateVisibility() => this.FadeTo(IsOpen.Value ? 1 : 0, 200);

    // stop mouse events from going through
    protected override bool OnHover(HoverEvent e) => true;
    protected override bool OnClick(ClickEvent e) => true;
    protected override bool OnDragStart(DragStartEvent e) => true;
    protected override bool OnScroll(ScrollEvent e) => true;
}
