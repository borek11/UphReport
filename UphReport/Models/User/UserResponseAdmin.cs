using UphReport.Entities;

namespace UphReport.Models.User;

public class UserResponseAdmin
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int RoleId { get; set; }

}
