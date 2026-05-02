using System;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Panel.Presets;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Online.API.Payloads.Maps;
using fluXis.Online.API.Requests.Maps;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Notifications;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;

namespace fluXis.Overlay.MapSet.UI;

public partial class RateVoteFormPanel : FormPanel<MapRateVotePayload>
{
    [Resolved]
    private IAPIClient api { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    private readonly FluXisTextFlow ratingText;
    private readonly long mapId;
    private readonly Action<float> onSuccess;

    public RateVoteFormPanel(long mapId, Action<float> onSuccess = null)
        : base(FontAwesome6.Solid.Check, new LocalisableString($"Rate Vote (id {mapId})"), new MapRateVotePayload(), (form, data) => ((RateVoteFormPanel)form).onVote(form, data), new LocalisableString("Vote"))
    {
        this.mapId = mapId;
        this.onSuccess = onSuccess;

        BottomContainer.Add(new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,
            Children = new Drawable[]
            {
                new FluXisTextFlow
                {
                    AutoSizeAxes = Axes.Both,
                    Text = "Your Rating: ",
                    WebFontSize = 16,
                },
                ratingText = new FluXisTextFlow
                {
                    AutoSizeAxes = Axes.Both,
                    Text = "0",
                    Colour = Theme.GetDifficultyColor(0f),
                    WebFontSize = 16,
                }
            }
        });
    }

    protected override void OnDataUpdate(MapRateVotePayload data)
    {
        float rating = data.ComputeRating();
        ratingText.Text = rating.ToStringInvariant();
        ratingText.Colour = Theme.GetDifficultyColor(rating);
    }

    private bool onVote(FormPanel<MapRateVotePayload> form, MapRateVotePayload data)
    {
        // just in case
        if (!data.IsRatingValid())
            return false;

        form.StartLoading();

        var req = new MapRateVoteRequest(mapId, data);
        req.Success += _ => Schedule(() =>
        {
            form.StopLoading();
            form.Close();
            onSuccess?.Invoke(data.ComputeRating());
        });
        req.Failure += ex => Schedule(() =>
        {
            form.StopLoading();
            notifications.SendError("Failed to vote!", ex.Message);
        });
        api.PerformRequestAsync(req);

        return false;
    }
}
