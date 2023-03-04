using System;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit;
using fluXis.Game.Screens.Select;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Platform;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Menu
{
    public partial class MenuScreen : Screen
    {
        [Resolved]
        private MapStore maps { get; set; }

        [Resolved]
        private BackgroundStack backgrounds { get; set; }

        [BackgroundDependencyLoader]
        private void load(Storage storage)
        {
            // load a random map
            if (maps.MapSets.Count > 0)
            {
                maps.CurrentMapSet = maps.GetRandom();

                RealmMap map = maps.CurrentMapSet.Maps.First();
                Conductor.PlayTrack(map, true, map.Metadata.PreviewTime);
            }

            backgrounds.AddBackgroundFromMap(maps.CurrentMapSet?.Maps.First());

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
                        new MenuButton("Edit", () =>
                        {
                            RealmMap map = maps.CurrentMapSet.Maps.First();
                            MapInfo mapInfo = MapUtils.LoadFromPath(storage.GetFullPath("files/" + PathUtils.HashToPath(map.Hash)));
                            this.Push(new Editor(map, mapInfo));
                        })
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
        }

        private partial class MenuButton : Button
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

        public override void OnEntering(ScreenTransitionEvent e)
        {
            this.FadeInFromZero(100);
            base.OnEntering(e);
        }

        public override void OnSuspending(ScreenTransitionEvent e)
        {
            this.FadeOut(100);
            base.OnSuspending(e);
        }

        public override void OnResuming(ScreenTransitionEvent e)
        {
            this.FadeIn(100);
            backgrounds.Zoom = 1;
            base.OnResuming(e);
        }
    }
}
