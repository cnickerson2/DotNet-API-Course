using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System;
using Dapper;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserCompleteController : ControllerBase
    {
        DataContextDapper _dapper;

        public UserCompleteController(IConfiguration config) 
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetUsers/{userId}/{isActive}")]
        public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
        {
            string sql = @"EXEC TutorialAppSchema.spUsers_Get";
            string stringParameters = "";
            DynamicParameters sqlParameters = new DynamicParameters();

            if (userId != 0)
            {
                stringParameters += ", @UserId = @UserIdParam";
                sqlParameters.Add("@UserIdParam", userId, System.Data.DbType.Int32);
            }
            if (isActive)
            {
                stringParameters += ", @Active = @ActiveParam";
                sqlParameters.Add("@ActiveParam", isActive, System.Data.DbType.Boolean);
            }

            if (stringParameters.Length > 0)
            {
                sql += stringParameters.Substring(1);
            }

            IEnumerable<UserComplete> users = _dapper.LoadDataWithParameters<UserComplete>(sql,sqlParameters);
            return users;
        }

        [HttpPut("UpsertUser")]
        public IActionResult UpsertUser(UserComplete user)
        {
            string sql = @"EXEC TutorialAppSchema.spUser_Upsert
                         @FirstName = @FirstNameParam,
                         @LastName = @LastNameParam, 
                         @Email = @EmailParam,
                         @Gender = @GenderParam,
                         @Active = @ActiveParam,
                         @JobTitle = @JobTitleParam,
                         @Department = @DepartmentParam,
                         @Salary = @SalaryParam,
                         @UserId = @UserIdParam";
            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@FirstNameParam", user.FirstName, System.Data.DbType.String);
            sqlParameters.Add("@LastNameParam", user.LastName, System.Data.DbType.String);
            sqlParameters.Add("@EmailParam", user.Email, System.Data.DbType.String);
            sqlParameters.Add("@GenderParam", user.Gender, System.Data.DbType.String);
            sqlParameters.Add("@ActiveParam", user.Active, System.Data.DbType.Boolean);
            sqlParameters.Add("@JobTitleParam", user.JobTitle, System.Data.DbType.String);
            sqlParameters.Add("@DepartmentParam", user.Department, System.Data.DbType.String);
            sqlParameters.Add("@SalaryParam", user.Salary, System.Data.DbType.Decimal);
            sqlParameters.Add("@UserIdParam", user.UserId, System.Data.DbType.Int32);

            if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
            {
                return Ok();
            }

            throw new Exception("Failed to Update User");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            string sql = "EXEC TutorialAppSchema.spUser_Delete @UserId=@UserIdParam";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParam", userId, System.Data.DbType.Int32);

            if (_dapper.ExecuteSqlWithParameters(sql,sqlParameters))
            {
                return Ok();
            }

            throw new Exception("Failed to Delete User");
        }
    }
}
