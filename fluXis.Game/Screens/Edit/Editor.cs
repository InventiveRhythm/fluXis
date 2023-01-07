using fluXis.Game.Graphics.Background;
using fluXis.Game.Input;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Edit
{
    public class Editor : Screen, IKeyBindingHandler<FluXisKeybind>
    {
        public MapInfo Map;

        private Container tabs;
        private int currentTab;

        public Editor(MapInfo map = null)
        {
            Map = map ?? new MapInfo(new MapMetadata());
        }

        [BackgroundDependencyLoader]
        private void load(BackgroundStack backgrounds)
        {
            backgrounds.AddBackgroundFromMap(Map);

            InternalChildren = new Drawable[]
            {
                tabs = new Container
                {
                    Padding = new MarginPadding { Top = 40 },
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new SetupTab(this),
                        new ComposeTab(this),
                        new TimingTab(this),
                    }
                }
            };

            ChangeTab(0);
        }

        public void ChangeTab(int by)
        {
            currentTab += by;

            if (currentTab < 0)
                currentTab = 0;
            if (currentTab >= tabs.Count)
                currentTab = tabs.Count - 1;

            for (var i = 0; i < tabs.Children.Count; i++)
            {
                Drawable tab = tabs.Children[i];
                tab.FadeTo(i == currentTab ? 1 : 0);
            }
        }

        public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e)
        {
            switch (e.Action)
            {
                case FluXisKeybind.PreviousGroup:
                    ChangeTab(-1);
                    return true;

                case FluXisKeybind.NextGroup:
                    ChangeTab(1);
                    return true;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e)
        {
        }
    }
}
