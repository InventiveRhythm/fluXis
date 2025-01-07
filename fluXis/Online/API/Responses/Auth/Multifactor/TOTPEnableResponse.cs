using System;
using System.Collections.Generic;
using fluXis.Utils;
using Newtonsoft.Json;

namespace fluXis.Online.API.Responses.Auth.Multifactor;

#nullable enable

public class TOTPEnableResponse
{
    [JsonProperty("backup-codes")]
    public List<string> BackupCodes { get; init; } = null!;

    public TOTPEnableResponse(List<string> codes)
    {
        BackupCodes = codes;
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public TOTPEnableResponse()
    {
    }
}
