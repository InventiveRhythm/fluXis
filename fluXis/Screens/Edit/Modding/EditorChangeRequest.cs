using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Online.API.Models.Maps.Modding;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Modding;

public class EditorChangeRequest : ITimedObject, IHasDuration
{
    public APIModdingAction Action { get; }
    public APIModdingChangeRequest Request { get; }

    public Colour4 AccentColor => Colour4.TryParseHex(Request.HexColor, out var col) ? col : FluXisColors.Highlight;

    public EditorChangeRequest(APIModdingAction action, APIModdingChangeRequest request)
    {
        Action = action;
        Request = request;
    }

    public double Time
    {
        get => Request.StartTime;
        set => Request.StartTime = value;
    }

    public double Duration
    {
        get
        {
            if (Request.EndTime is null || Request.EndTime <= Request.StartTime)
                return 0;

            return Request.EndTime.Value - Request.StartTime;
        }
        set => Request.EndTime = Request.StartTime + value;
    }
}
