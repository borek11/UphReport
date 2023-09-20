using UphReport.Models.User;

namespace UphReport.Interfaces;

public interface IUserService
{
    Task<string> RegisterUserAsync(RegisterUser userDto);
    Task<string> LoginAsync(LoginUser loginUser);
    Task<UserResponse> GetUserAsync(string token);
    Task<bool> UpdateUserAsync(string token, UserRequest userRequest);
    Task<bool> UpdatePasswordUserAsync(string token, UserPasswordRequest userPasswordRequest);
    Task<UserName> GetNameAsync(int id);
    Task<List<UserResponseAdmin>> GetAllUsers();
    Task<bool> BlockUser(int id);
    Task<bool> UnBlockUser(int id);

}