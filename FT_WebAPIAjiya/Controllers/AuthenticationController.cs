using Dapper;
using FT_WebAPIAjiya.Extensions;
using FT_WebAPIAjiya.Managers;
using FT_WebAPIAjiya.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace FT_WebAPIAjiya.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost(nameof(Create))]
        public IActionResult Create([FromBody] User input)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(configuration.GetConnectionString("Default")))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();

                    string query = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
                    var result = conn.Execute(query, new { input.Username, Password = HashPassword(input.Username, input.Password) });

                    if (result <= 0)
                    {
                        return BadRequest("Failed to create user");
                    }

                    return Ok("Success");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost(nameof(Login))]
        public IActionResult Login([FromBody] User input)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(configuration.GetConnectionString("Default")))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();
                    
                    var userExists = CheckIfUserExists(input.Username);
                    if (userExists == null)
                    {
                        return BadRequest("User does not exist");
                    }

                    var validPwd = HashPassword(input.Username, input.Password) == userExists.Password;
                    if (!validPwd)
                    {
                        return Unauthorized("Incorrect Password");
                    }

                    var token = TokenManager.GenerateToken(input);

                    return Ok(token);
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        private User? CheckIfUserExists(string username)
        {
            using (SqlConnection conn = new SqlConnection(configuration.GetConnectionString("Default")))
            {
                if (conn.State == System.Data.ConnectionState.Closed) conn.Open();

                string query = "SELECT * FROM Users WHERE Username = @Username";
                var result = conn.Query<User>(query, new { Username = username }).FirstOrDefault();

                return result;
            }
        }

        internal static string HashPassword(string username, string password)
        {
            return $"{username.ToLower()}{password}".SHA256();
        }
    }
}
