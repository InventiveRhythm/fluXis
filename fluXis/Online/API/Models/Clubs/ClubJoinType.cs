using System.ComponentModel;

namespace fluXis.Online.API.Models.Clubs;

public enum ClubJoinType
{
    [Description("Anyone can join")]
    Open = 0,

    [Description("Invite Only")]
    InviteOnly = 1
}
