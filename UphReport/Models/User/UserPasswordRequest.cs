namespace UphReport.Models.User;

public class UserPasswordRequest
{
    public string OldPassword { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }

    public UserPasswordRequest()
    {

    }
}
