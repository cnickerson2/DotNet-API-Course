using Dapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;
        public AuthController(IConfiguration config) 
        { 
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDTO userForRegistration)
        {
            if(userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExists = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" + userForRegistration.Email + "'";

                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
                if (!existingUsers.Any())
                {
                    UserForLoginDTO userForSetPassword = new UserForLoginDTO()
                    {
                        Email = userForRegistration.Email,
                        Password = userForRegistration.Password
                    };

                    if (_authHelper.SetPassword(userForSetPassword))
                    {

                        string sqlAddUser = "EXEC TutorialAppSchema.spUser_Upsert " +
                                      "@FirstName = '" + userForRegistration.FirstName +
                                      "', @LastName = '" + userForRegistration.LastName +
                                      "', @Email = '" + userForRegistration.Email +
                                      "', @Gender = '" + userForRegistration.Gender +
                                      "', @Active = 1" +
                                      ", @JobTitle = '" + userForRegistration.JobTitle +
                                      "', @Department = '" + userForRegistration.Department +
                                      "', @Salary = " + userForRegistration.Salary;
                        if (_dapper.ExecuteSql(sqlAddUser))
                        {
                            return Ok();

                        }
                        throw new Exception("Failed to add user");
                    }

                    throw new Exception("Failed to register user");
                }

                throw new Exception("User with this email already exists");
            }

            throw new Exception("Password do not match");
        }

        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(UserForLoginDTO userForSetPassword)
        {
            if (_authHelper.SetPassword(userForSetPassword))
            {
                return Ok();
            }
            throw new Exception("Failed to update password");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDTO userForLogin)
        {
            string sqlForHashAndSalt = "TutorialAppSchema.spLoginConfirmation_Get " +
                                        "@Email = @EmailParam";

            DynamicParameters sqlParameters = new DynamicParameters();

            //SqlParameter emailParameter = new SqlParameter("@EmailParam", System.Data.SqlDbType.VarChar);
            //emailParameter.Value = userForLogin.Email;
            //sqlParameters.Add(emailParameter);
            sqlParameters.Add("@EmailParam", userForLogin.Email, System.Data.DbType.String);
            UserForLoginConfirmationDTO userForLoginConfirmation = _dapper.LoadDataSingleWithParameters<UserForLoginConfirmationDTO>(sqlForHashAndSalt,sqlParameters);

            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForLoginConfirmation.PasswordSalt);

            // if(passwordHash == userForLoginConfirmation.PasswordHash) // Won't work
            for(int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForLoginConfirmation.PasswordHash[index])
                {
                    return StatusCode(401, "Incorrect Password!");
                }
            }

            string userIdSql = @"SELECT [UserId] FROM TutorialAppSchema.Users WHERE Email = '" + userForLogin.Email + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string>
            {
                {"token", _authHelper.CreateToken(userId) }
            });
        }

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            string userId = User.FindFirst("userId")?.Value + "";

            string userIdSql = "SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = " + userId;

            int userIdFromDB = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string>
            {
                {"token", _authHelper.CreateToken(userIdFromDB) }
            });
        }
    }
}