using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Footer;
using fluXis.Localization;
using fluXis.Mods;
using fluXis.Mods.Drawables;
using fluXis.Screens.Select.Mods;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Screens.Select.Footer;

public partial class SelectModsButton : FooterButton
{
    [Resolved]
    private ModsOverlay modsOverlay { get; set; }

    private Drawable icon;
    private ModList mods;

    public SelectModsButton(Action action)
    {
        Text = LocalizationStrings.SongSelect.FooterMods;
        Icon = FontAwesome6.Solid.Cube;
        AccentColor = FluXisColors.Footer1;
        Action = action;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        modsOverlay.SelectedMods.BindCollectionChanged((_, _) => modsChanged(modsOverlay.SelectedMods));
        modsOverlay.RateChanged += _ => modsChanged(modsOverlay.SelectedMods);
    }

    protected override Drawable CreateIcon() => new Container
    {
        AutoSizeAxes = Axes.X,
        Height = 24,
        Anchor = Anchor.TopCentre,
        Origin = Anchor.TopCentre,
        Children = new[]
        {
            icon = base.CreateIcon(),
            mods = new ModList(icon =>
            {
                icon.Width = 42;
                icon.IconWidthRate = 64;
                icon.Height = 24;
                icon.IconSize = 16;
                icon.CornerRadius = 6;
                icon.FlowSpacing = 2;
            })
            {
                AlwaysRefresh = true,
                ModSpacing = -24
            }
        }
    };

    private void modsChanged(IList<IMod> list)
    {
        if (list.Count > 0)
        {
            this.ResizeWidthTo(220, 200, Easing.OutQuint);
            mods.Mods = list.ToList();
            icon.FadeOut(100);
            mods.FadeIn(100);

            Schedule(() =>
            {
                var multiplier = modsOverlay.ScoreMultiplier;
                SpriteText.Text = $"{multiplier.ToStringInvariant("0.00")}x";
                SpriteText.FadeColour(multiplier switch
                {
                    < 1 => FluXisColors.Green,
                    > 1 => FluXisColors.Red,
                    _ => FluXisColors.Text
                }, 100);
            });
        }
        else
        {
            this.ResizeWidthTo(150, 400, Easing.OutQuint);
            icon.FadeIn(200);
            mods.FadeOut(200);
            SpriteText.Text = LocalizationStrings.SongSelect.FooterMods;
            SpriteText.FadeColour(FluXisColors.Text, 200);
        }
    }

    protected override bool OnMouseDown(MouseDownEvent e) => true;

    protected override void OnMouseUp(MouseUpEvent e)
    {
        if (e.Button != MouseButton.Right || !IsHovered)
            return;

        modsOverlay.DeselectAll();
    }
}
