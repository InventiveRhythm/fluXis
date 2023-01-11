using System;
using System.Linq;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit;
using fluXis.Game.Screens.Select;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Menu
{
    public class MenuScreen : Screen
    {
        [BackgroundDependencyLoader]
        private void load(MapStore maps, BackgroundStack backgrounds)
        {
            // load a random map
            if (maps.MapSets.Count == 0)
                backgrounds.AddBackgroundFromMap(null);
            else
            {
                maps.CurrentMapSet = maps.GetRandom();
                backgrounds.AddBackgroundFromMap(maps.CurrentMapSet.Maps.First());
            }

            InternalChildren = new Drawable[]
            {
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Y,
                    Width = 500,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Direction = FillDirection.Vertical,
                    Children = new Drawable[]
                    {
                        new SpriteText
                        {
                            Text = "fluXis",
                            Font = new FontUsage("Quicksand", 100, "Bold"),
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                        },
                        new MenuButton("Play", () => this.Push(new SelectScreen())),
                        new MenuButton("Edit", () => this.Push(new Editor()))
                    }
                }
            };
        }

        private class MenuButton : Button
        {
            public MenuButton(string text, Action action)
            {
                Action = action;
                RelativeSizeAxes = Axes.X;
                Height = 30;
                CornerRadius = 5;
                Masking = true;

                Children = new Drawable[]
                {
                    new Box
                    {
                        Colour = Colour4.Black,
                        RelativeSizeAxes = Axes.Both,
                        Alpha = .2f
                    },
                    new SpriteText
                    {
                        Text = text,
                        Font = new FontUsage("Quicksand", 24, "SemiBold"),
                        Padding = new MarginPadding { Horizontal = 10 },
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                };
            }
        }
    }
}
