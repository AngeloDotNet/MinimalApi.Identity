using MinimalApi.Identity.Core.Entities.Common;

namespace MinimalApi.Identity.Core.Entities;

public class UserProfile : BaseEntity, IEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public DateOnly? LastDateChangePassword { get; set; }
    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    //public void ChangeUserId(int userId)
    //{
    //    UserId = userId switch
    //    {
    //        <= 0 => throw new ArgumentOutOfRangeException(nameof(userId), "UserId must be greater than zero."),
    //        _ => userId,
    //    };
    //}

    //public void ChangeFirstName(string firstName)
    //{
    //    if (string.IsNullOrWhiteSpace(firstName))
    //    {
    //        throw new ArgumentNullException(nameof(firstName), "FirstName cannot be null or empty.");
    //    }

    //    FirstName = firstName;
    //}

    //public void ChangeLastName(string lastName)
    //{
    //    if (string.IsNullOrWhiteSpace(lastName))
    //    {
    //        throw new ArgumentNullException(nameof(lastName), "LastName cannot be null or empty.");
    //    }

    //    LastName = lastName;
    //}

    //public void ChangeUserEnabled(bool isEnabled)
    //{
    //    IsEnabled = isEnabled;
    //}

    //public void ChangeLastDateChangePassword(DateOnly? lastDateChangePassword)
    //{
    //    var utcNow = DateOnly.FromDateTime(DateTime.UtcNow);

    //    if (lastDateChangePassword >= utcNow)
    //    {
    //        throw new ArgumentOutOfRangeException(nameof(lastDateChangePassword), "Last date change password cannot be greater than today.");
    //    }

    //    LastDateChangePassword = lastDateChangePassword;
    //}
}