using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Footer;
using fluXis.Game.Localization;
using fluXis.Game.Mods;
using fluXis.Game.Mods.Drawables;
using fluXis.Game.Screens.Select.Mods;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Game.Screens.Select.Footer;

public partial class SelectModsButton : FooterButton
{
    [Resolved]
    private ModSelector modSelector { get; set; }

    private Drawable icon;
    private ModList mods;

    public SelectModsButton(Action action)
    {
        Text = LocalizationStrings.SongSelect.FooterMods;
        Icon = FontAwesome6.Solid.LayerGroup;
        AccentColor = Colour4.FromHex("#edbb98");
        Action = action;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        modSelector.SelectedMods.BindCollectionChanged((_, _) => modsChanged(modSelector.SelectedMods));
        modSelector.RateChanged += _ => modsChanged(modSelector.SelectedMods);
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
                var multiplier = modSelector.ScoreMultiplier;
                SpriteText.Text = $"{multiplier.ToStringInvariant("0.00")}x";
                SpriteText.FadeColour(multiplier switch
                {
                    < 1 => FluXisColors.Green,
                    > 1 => FluXisColors.Red,
                    _ => Colour4.White
                }, 100);
            });
        }
        else
        {
            this.ResizeWidthTo(150, 400, Easing.OutQuint);
            icon.FadeIn(200);
            mods.FadeOut(200);
            SpriteText.Text = LocalizationStrings.SongSelect.FooterMods;
            SpriteText.FadeColour(Colour4.White, 200);
        }
    }

    protected override bool OnMouseDown(MouseDownEvent e) => true;

    protected override void OnMouseUp(MouseUpEvent e)
    {
        if (e.Button != MouseButton.Right || !IsHovered)
            return;

        modSelector.DeselectAll();
    }
}
