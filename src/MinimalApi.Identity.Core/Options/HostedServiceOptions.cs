using System.ComponentModel.DataAnnotations;

namespace MinimalApi.Identity.API.Options;

public class HostedServiceOptions
{
    [Required]
    public int IntervalAuthPolicyUpdaterMinutes { get; set; }
}