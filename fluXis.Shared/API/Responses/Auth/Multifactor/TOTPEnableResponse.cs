using fluXis.Shared.Utils;
using Newtonsoft.Json;

namespace fluXis.Shared.API.Responses.Auth.Multifactor;

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
