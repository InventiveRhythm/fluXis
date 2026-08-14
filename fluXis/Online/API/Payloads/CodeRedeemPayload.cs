using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace fluXis.Online.API.Payloads;

public class CodeRedeemPayload
{
    [Description("Redemption Code")]
    [JsonProperty("code")]
    [MaxLength(24), Required]
    public string RedemptionCode { get; set; }
}
