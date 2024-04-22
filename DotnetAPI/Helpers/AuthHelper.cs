using DotnetAPI.Data;
using DotnetAPI.DTOs;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DotnetAPI.Helpers
{
    public class AuthHelper
    {
        private readonly IConfiguration _config;
        private readonly DataContextDapper _dapper;

        public AuthHelper(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
        }
        public byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);
            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
                );
        }

        public string CreateToken(int userId)
        {
            Claim[] claims = new Claim[]
            {
                new Claim("userId",userId.ToString())
            };

            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        tokenKeyString != null ? tokenKeyString : ""
                    )
                );

            SigningCredentials credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }

        public bool SetPassword(UserForLoginDTO userForSetPassword)
        {
            byte[] passwordSalt = new byte[128 / 8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }

            byte[] passwordHash = GetPasswordHash(userForSetPassword.Password, passwordSalt);

            string sqlAddAuth = "EXEC TutorialAppSchema.spRegistration_Upsert " +
                "@Email = @EmailParam" +
                ", @PasswordHash = @PasswordHashParam" +
                ", @PasswordSalt = @PasswordSaltParam";

            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            SqlParameter emailParameter = new SqlParameter("@EmailParam", System.Data.SqlDbType.VarChar);
            emailParameter.Value = userForSetPassword.Email;

            SqlParameter passwordHashParameter = new SqlParameter("@PasswordHashParam", System.Data.SqlDbType.VarBinary);
            passwordHashParameter.Value = passwordHash;

            SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSaltParam", System.Data.SqlDbType.VarBinary);
            passwordSaltParameter.Value = passwordSalt;

            sqlParameters.Add(emailParameter);
            sqlParameters.Add(passwordHashParameter);
            sqlParameters.Add(passwordSaltParameter);

            return _dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters);
        }
    }
}
