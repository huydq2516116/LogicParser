using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LogicParser.Api.Data;
using LogicParser.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LogicParser.Api.Services;

public class UserService(LogicParserContext context)
{
    public readonly LogicParserContext _context = context;
    private const string DummyHash =
    "$2a$11$u1u1u1u1u1u1u1u1u1u1uO8Qw8Qw8Qw8Qw8Qw8Qw8Qw8Qw8Qw8Qw8Qw"; // bcrypt fake
    public async Task<string?> SignUp(string username, string password)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (existingUser != null)
            return null;

        var user = new User
        {
            Username = username,
            Password = PasswordHasher.Hash(password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return "Sign up successful";
    }

    public async Task<string?> SignIn(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        // nếu không có user → dùng dummy hash
        var hashToVerify = user?.Password ?? DummyHash;
        bool isValid = PasswordHasher.Verify(password, hashToVerify);
        if (user == null || !isValid)
            return null;

        return GenerateToken(user);
    }

    public string GenerateToken(User user)
{
    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username)
    };

    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes("your-secret-key-123")
    );

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.AddHours(2),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}
}
