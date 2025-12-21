using MinimalApi.Identity.ClaimsManager.Models;

namespace MinimalApi.Identity.ClaimsManager.Services;

public interface IClaimsService
{
    Task<List<ClaimResponseModel>> GetAllClaimsAsync();
    Task<string> CreateClaimAsync(CreateClaimModel model);
    Task<string> AssignClaimAsync(AssignClaimModel model);
    Task<string> RevokeClaimAsync(RevokeClaimModel model);
    Task<string> DeleteClaimAsync(DeleteClaimModel model);
}