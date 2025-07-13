using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Licenses.Models;

namespace MinimalApi.Identity.Licenses.Extensions;

public static class DatabaseExtensions
{
    public static IQueryable<LicenseResponseModel> ToLicenseResponseModel(this IQueryable<License> query)
        => query.Select(lm => new LicenseResponseModel(lm.Id, lm.Name, lm.ExpirationDate));

    public static IQueryable<UserLicense> ToUserLicense(this IQueryable<UserLicense> query)
    {
        return query.Select(ul => new UserLicense
        {
            UserId = ul.UserId,
            LicenseId = ul.LicenseId,
        });
    }
}
