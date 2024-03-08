using fluXis.Game.Online.API.Models.Clubs;
using fluXis.Shared.Components.Users;
using JetBrains.Annotations;

namespace fluXis.Game.Online.API.Models.Users;

public class APIUser : APIUserShort, IAPIUser
{
    public string AboutMe { get; set; } = string.Empty;

    [CanBeNull]
    public APIClubShort Club { get; set; } = new();

    public IAPIUserSocials Socials { get; init; } = new APIUserSocials();
    public long CreatedAt { get; init; }
    public long LastLogin { get; set; }
    public bool IsOnline { get; init; }
    public double OverallRating { get; set; }
    public double PotentialRating { get; set; }
    public int MaxCombo { get; set; }
    public int RankedScore { get; set; }
    public double OverallAccuracy { get; set; }
    public int GlobalRank { get; init; }
    public int CountryRank { get; init; }
    public bool Following { get; set; }

    public class APIUserSocials : IAPIUserSocials
    {
        public string Twitter { get; set; } = string.Empty;
        public string Twitch { get; set; } = string.Empty;
        public string YouTube { get; set; } = string.Empty;
        public string Discord { get; set; } = string.Empty;
    }
}
