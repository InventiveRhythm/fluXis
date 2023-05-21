using System.Collections.Generic;
using fluXis.Game.Graphics.Menu;
using fluXis.Game.Map.Events;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Game.Screens.Edit.Tabs.Compose.Effect;

public partial class EditorEffectContainer : Container, IHasContextMenu
{
    [Resolved]
    private EditorValues values { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    private Container<EditorFlashEvent> flashContainer;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        Child = flashContainer = new Container<EditorFlashEvent>
        {
            AutoSizeAxes = Axes.X,
            RelativeSizeAxes = Axes.Y,
            Y = -EditorPlayfield.HITPOSITION_Y,
            Anchor = Anchor.BottomRight,
            Origin = Anchor.BottomLeft,
            Margin = new MarginPadding { Left = 10 }
        };
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
                })
            };

            return items.ToArray();
        }
    }

    public void AddFlash(FlashEvent flash)
    {
        flashContainer.Add(new EditorFlashEvent { FlashEvent = flash });
    }
}
