using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CityInfo.API.Controllers;

[Route("api/authentication")]
[ApiController]
public class AuthenticationController(IConfiguration configuration) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration ??
            throw new ArgumentNullException(nameof(configuration));

    // we won't use this outside of this class, so we can scope it to this namespace
    public class AuthenticationRequestBody
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }

    private class CityInfoUser(
        int userId,
        string userName,
        string firstName,
        string lastName,
        string city)
    {
        public int UserId { get; set; } = userId;
        public string UserName { get; set; } = userName;
        public string FirstName { get; set; } = firstName;
        public string LastName { get; set; } = lastName;
        public string City { get; set; } = city;
    }

    [HttpPost("authenticate")]
    public ActionResult<string> Authenticate(
        AuthenticationRequestBody authenticationRequestBody)
    {  
        // Step 1: validate the username/password
        var user = ValidateUserCredentials(
            authenticationRequestBody.UserName,
            authenticationRequestBody.Password);

        if (user == null)
        {
            return Unauthorized();
        }

        // Step 2: create a token
        var secret = _configuration["Authentication:SecretForKey"];
        if (secret == null)
        {
            return StatusCode(500);
        }

        var securityKey = new SymmetricSecurityKey(
            Convert.FromBase64String(secret));
        var signingCredentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);
         
        var claimsForToken = new List<Claim>
        {
            new ("sub", user.UserId.ToString()),
            new ("given_name", user.FirstName),
            new ("family_name", user.LastName),
            new ("city", user.City)
        };
         
        var jwtSecurityToken = new JwtSecurityToken(
            _configuration["Authentication:Issuer"],
            _configuration["Authentication:Audience"],
            claimsForToken,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1),
            signingCredentials);

        var tokenToReturn = new JwtSecurityTokenHandler()
           .WriteToken(jwtSecurityToken);

        return Ok(tokenToReturn);
    }

    private CityInfoUser ValidateUserCredentials(string? userName, string? password)
    {
        // we don't have a user DB or table.  If you have, check the passed-through
        // username/password against what's stored in the database.
        //
        // For demo purposes, we assume the credentials are valid

        // return a new CityInfoUser (values would normally come from your user DB/table)
        return new CityInfoUser(
            1,
            userName ?? "",
            "Kevin",
            "Dockx",
            "Antwerp");

    }
}
