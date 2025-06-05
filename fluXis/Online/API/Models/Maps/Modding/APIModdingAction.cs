using System;
using System.Collections.Generic;
using fluXis.Online.API.Models.Users;
using fluXis.Utils;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Maps.Modding;

public class APIModdingAction
{
    [JsonProperty("id")]
    public string ID { get; init; }

    [JsonProperty("user")]
    public APIUser User { get; init; }

    [JsonProperty("type")]
    public APIModdingActionType Type { get; set; }

    [JsonProperty("state")]
    public APIModdingActionState State { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }

    [JsonProperty("time")]
    public long Timestamp { get; init; }

    [CanBeNull]
    [JsonProperty("changes")]
    public List<APIModdingChangeRequest> ChangeRequests { get; init; }

    public APIModdingAction(string id, APIUser user, APIModdingActionType type, APIModdingActionState state, string content, long timestamp)
    {
        ID = id;
        User = user;
        Type = type;
        State = state;
        Content = content;
        Timestamp = timestamp;
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR)]
    public APIModdingAction()
    {
    }
}

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public enum APIModdingActionType
{
    /// <summary>
    /// a basic comment
    /// </summary>
    Note = 0,

    /// <summary>
    /// a reply to a comment
    /// </summary>
    Reply = 1,

    /// <summary>
    /// purifier approve comment
    /// </summary>
    Approve = 2,

    /// <summary>
    /// purifier deny comment
    /// </summary>
    Deny = 3,

    /// <summary>
    /// map submitted to queue
    /// </summary>
    Submitted = 4,

    /// <summary>
    /// map update
    /// </summary>
    Update = 5,

    /// <summary>
    /// requests changes to a map
    /// </summary>
    RequestChanges = 6
}

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public enum APIModdingActionState
{
    Pending,
    Outdated,
    Resolved
}
