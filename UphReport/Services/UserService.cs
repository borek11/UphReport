using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UphReport.Data;
using UphReport.Entities;
using UphReport.Exceptions;
using UphReport.Interfaces;
using UphReport.Models.User;

namespace UphReport.Services;

public class UserService : IUserService
{
    private readonly MyDbContext _myDbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly IMapper _mapper;


    public UserService(MyDbContext myDbContext, AuthenticationSettings authenticationSettings, IPasswordHasher<User> passwordHasher, IMapper mapper)
    {
        _myDbContext = myDbContext;
        _authenticationSettings = authenticationSettings;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<string> RegisterUserAsync(RegisterUser userDto)
    {
        //dodac sprawdzanie czy user z mailem istnieje
        if (userDto.Password.Length < 8)
            throw new BadRequestException("Hasła za krótkie");
        if (userDto.Password != userDto.ConfirmPassword)
            throw new BadRequestException("Hasła się różnią");

        var emailExist = await _myDbContext.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == userDto.Email.ToLower());
        if (emailExist != null)
            throw new BadRequestException("Email już istnieje");

        var newUser = _mapper.Map<User>(userDto);
        newUser.RoleId = 2;
        var hashedPassword = _passwordHasher.HashPassword(newUser, userDto.Password);
        newUser.PasswordHash = hashedPassword;
        _myDbContext.Users.Add(newUser);
        _myDbContext.SaveChanges();

        return "Ok";
    }
    public async Task<string> LoginAsync(LoginUser loginUser)
    {
        var user = await _myDbContext
            .Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == loginUser.Email);
        if (user is null)
        {
            throw new BadRequestException("Błędny email lub hasło");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginUser.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException("Błędny email lub hasło");
        }

        if(user.RoleId == 3)
        {
            throw new BadRequestException("Konto zablokowane");
        }

        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, $"{user.Role.Name}"),                
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);
        var expires = DateTime.UtcNow.AddDays(_authenticationSettings.JwtExpireDays);

        var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
            _authenticationSettings.JwtIssuer,
            claims,
            expires: expires,
            signingCredentials: credentials);

        var tokenHandler = new JwtSecurityTokenHandler();

        return tokenHandler.WriteToken(token);
    }
    public async Task<UserResponse> GetUserAsync(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);
        var userId = jwtSecurityToken.Claims.First(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

        var user = await _myDbContext.Users.FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

        if (user == null)
        {
            throw new NotFoundException($"Nie znaleziono użytkownika!");
        }
        //var userResponse = _mapper.Map<UserResponse>(user);
        var userResponse = new UserResponse()
        {
            Id = user.Id,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
        return userResponse;
    }
    public async Task<bool> UpdateUserAsync(string token, UserRequest userRequest)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);
        var userId = jwtSecurityToken.Claims.First(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
        var user = await _myDbContext.Users.FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

        if (user == null)
        {
            throw new NotFoundException($"Nie znaleziono użytkownika!");
        }
        user.DateOfBirth = userRequest.DateOfBirth;
        user.FirstName = userRequest.FirstName;
        user.LastName = userRequest.LastName;

        await _myDbContext.SaveChangesAsync();
        return true;
    }
    public async Task<bool> UpdatePasswordUserAsync(string token, UserPasswordRequest userPasswordRequest)
    {
        if (userPasswordRequest.Password != userPasswordRequest.ConfirmPassword)
        {
            throw new BadRequestException($"Podane hasła są różne");
        }
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);
        var userId = jwtSecurityToken.Claims.First(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
        var user = await _myDbContext.Users.FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

        if (user == null)
        {
            throw new NotFoundException($"Nie znaleziono użytkownika!");
        }

        var hashedPassword = _passwordHasher.VerifyHashedPassword(user,user.PasswordHash,userPasswordRequest.Password);
        if (hashedPassword == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException($"Podane hasła są różne");
        }

        return true;
    }
    public async Task<UserName> GetNameAsync(int id)
    {
        var result = await _myDbContext.Users.Where(x => x.Id == id).Select(x => new UserName
        {
            FirstName = x.FirstName,
            LastName = x.LastName,
        }).FirstOrDefaultAsync();
        if(result == null)
            throw new NotFoundException("Brak użytkownika");
        
        return result;
    }
    public async Task<List<UserResponseAdmin>> GetAllUsers()
    {
        var result = await _myDbContext.Users.Select(x => new UserResponseAdmin
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName= x.LastName,
            Email = x.Email,
            DateOfBirth = x.DateOfBirth,
            RoleId = x.RoleId
        }).ToListAsync();

        return result;
    }
    public async Task<bool> BlockUser(int id)
    {
        var user = await _myDbContext.Users.Where(_x => _x.Id == id).FirstOrDefaultAsync();
        if (user == null)
            throw new NotFoundException("Brak użytkownika");

        user.RoleId = 3;
        await _myDbContext.SaveChangesAsync();
        return true;
    }
    public async Task<bool> UnBlockUser(int id)
    {
        var user = await _myDbContext.Users.Where(_x => _x.Id == id).FirstOrDefaultAsync();
        if (user == null)
            throw new NotFoundException("Brak użytkownika");

        user.RoleId = 2;
        await _myDbContext.SaveChangesAsync();
        return true;
    }
}
