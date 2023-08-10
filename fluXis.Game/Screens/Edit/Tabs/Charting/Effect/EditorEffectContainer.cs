using System.Collections.Generic;
using fluXis.Game.Graphics.Menu;
using fluXis.Game.Map.Events;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Effect;

public partial class EditorEffectContainer : Container, IHasContextMenu
{
    [Resolved]
    private EditorValues values { get; set; }

    [Resolved]
    private EditorChangeHandler changeHandler { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    private Container<EditorFlashEvent> flashContainer;
    private Container<EditorLaneSwitchEvent> lsContainer;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        Children = new Drawable[]
        {
            flashContainer = new Container<EditorFlashEvent>
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Y = -EditorHitObjectContainer.HITPOSITION,
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomLeft,
                Margin = new MarginPadding { Left = 10 }
            },
            lsContainer = new Container<EditorLaneSwitchEvent>
            {
                RelativeSizeAxes = Axes.Both,
                Y = -EditorHitObjectContainer.HITPOSITION
            }
        };

        loadEvents();

        changeHandler.OnKeyModeChanged += _ =>
        {
            ClearAll();
            loadEvents();
        };
    }

    private void loadEvents()
    {
        foreach (var flashEvent in values.MapEvents.FlashEvents)
            AddFlash(flashEvent);

        foreach (var laneSwitch in values.MapEvents.LaneSwitchEvents)
            AddLaneSwitch(laneSwitch);
    }

    public MenuItem[] ContextMenuItems
    {
        get
        {
            List<MenuItem> items = new()
            {
                new FluXisMenuItem("Add Flash", MenuItemType.Normal, () =>
                {
                    var flash = new FlashEvent
                    {
                        Time = (float)clock.CurrentTime,
                        Duration = (float)clock.BeatTime,
                        StartColor = Colour4.White,
                        EndColor = Colour4.White,
                        StartOpacity = 1,
                        EndOpacity = 0
                    };

                    values.MapEvents.FlashEvents.Add(flash);
                    AddFlash(flash);
                }),
                new FluXisMenuItem("Add LaneSwitch", MenuItemType.Normal, () =>
                {
                    var ls = new LaneSwitchEvent
                    {
                        Time = (float)clock.CurrentTime,
                        Speed = (float)clock.BeatTime,
                        Count = 1
                    };

                    values.MapEvents.LaneSwitchEvents.Add(ls);
                    AddLaneSwitch(ls);
                })
            };

            return items.ToArray();
        }
    }

    public void AddFlash(FlashEvent flash)
    {
        flashContainer.Add(new EditorFlashEvent { FlashEvent = flash });
    }

    public void AddLaneSwitch(LaneSwitchEvent ls)
    {
        lsContainer.Add(new EditorLaneSwitchEvent { Event = ls, Map = values.MapInfo });
    }

    public void ClearAll()
    {
        flashContainer.Clear();
        lsContainer.Clear();
    }
}
