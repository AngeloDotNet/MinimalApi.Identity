using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Licenses.Services.Interfaces;
using MinimalApi.Identity.Licenses.Validator;

namespace MinimalApi.Identity.Licenses.DependencyInjection;

public static class LicenseExtensions
{
    public const string License = nameof(ClaimsType.License);

    public const string EndpointsDefaultApi = "/api";
    public const string EndpointsStringEmpty = "";
    public const string EndpointsLicenzeGroup = EndpointsDefaultApi + "/licenses";
    public const string EndpointsLicenzeTag = "Licenses";

    public const string EndpointsCreateLicense = "/create-license";
    public const string EndpointsAssignLicense = "/assign-license";
    public const string EndpointsRevokeLicense = "/revoke-license";
    public const string EndpointsDeleteLicense = "/delete-license";

    public const string LicenseCreated = "License created successfully";
    public const string LicenseNotFound = "License not found";
    public const string LicenseAssigned = "License assigned successfully";
    public const string LicenseCanceled = "License removed successfully";
    public const string LicensesNotFound = "Licenses not found";
    public const string LicenseDeleted = "License deleted successfully";
    public const string LicenseNotDeleted = "License not deleted, it is not possible to delete a license assigned to a user";
    public const string LicenseNotAssignable = "You cannot assign more than one license to a user. Please check which license the user owns.";
    public const string LicenseExpired = "License expired. Without a valid license you cannot use the software.";
    public const string LicenseAlreadyExist = "License already exists";

    public static IServiceCollection LicenseRegistrationService(this IServiceCollection services)
    {
        services
            .AddRegisterServices(options =>
               {
                   options.Interfaces = [typeof(ILicenseService)];
                   options.StringEndsWith = "Service";
                   options.Lifetime = ServiceLifetime.Transient;
               })
               .ConfigureFluentValidation<AssignLicenseValidator>();

        return services;
    }
}
