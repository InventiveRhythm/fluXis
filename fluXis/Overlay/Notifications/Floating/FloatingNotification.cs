using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;

namespace fluXis.Overlay.Notifications.Floating;

public partial class FloatingNotification : Container
{
    public float TargetY = 0;

    protected FloatingNotification()
    {
        Anchor = Origin = Anchor.BottomCentre;
    }

    protected override void Update()
    {
        base.Update();

        if (Precision.AlmostEquals(TargetY, Y))
            Y = TargetY;
        else
            Y = (float)Interpolation.Lerp(TargetY, Y, Math.Exp(-0.01 * Time.Elapsed));
    }
}
