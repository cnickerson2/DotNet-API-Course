using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        DataContextDapper _dapper;

        public UserController(IConfiguration config) 
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("TestConnection")]
        public DateTime TestConnection()
        {
            return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
        }

        [HttpGet("GetUsers")]
        public IEnumerable<User> GetUsers()
        {
            string sql = @"SELECT [UserId]
                                , [FirstName]
                                , [LastName]
                                , [Email]
                                , [Gender]
                                , [Active]
                          FROM  TutorialAppSchema.Users";
            IEnumerable<User> users = _dapper.LoadData<User>(sql);
            return users;
        }

        [HttpGet("GetSingleUser/{userId}")]
        public User GetSingleUser(int userId)
        {
            string sql = @"SELECT [UserId]
                                , [FirstName]
                                , [LastName]
                                , [Email]
                                , [Gender]
                                , [Active]
                          FROM  TutorialAppSchema.Users
                          WHERE UserId = " + userId.ToString();
            User user = _dapper.LoadDataSingle<User>(sql);
            return user;
        }

        [HttpPut("EditUser")]
        public IActionResult EditUser(User user)
        {
            string sql = "UPDATE TutorialAppSchema.Users " +
                    "SET [FirstName] = '"+  user.FirstName +
                    "', [LastName] = '"+    user.LastName +
                    "', [Email] = '" +      user.Email +
                    "', [Gender] = '" +     user.Gender +
                    "', [Active] = '" +     user.Active +
                    "' WHERE UserId = " +   user.UserId;
            if(_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Update User");
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(UserToAddDTO user)
        {
            string sql = @" INSERT INTO TutorialAppSchema.Users ([FirstName], [LastName], [Email], [Gender], [Active])
                            VALUES ('"+ user.FirstName + "','" + user.LastName + "','" + user.Email + "','" + user.Gender + "','" + user.Active + "')";


            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Add User");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            string sql = @" DELETE FROM TutorialAppSchema.Users 
                            WHERE UserId = " + userId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Delete User");
        }

        [HttpGet("GetUserSalaries")]
        public IEnumerable<UserSalary> GetUserSalaries()
        {
            string sql = @"SELECT [UserId]
                                , [Salary]
                          FROM  TutorialAppSchema.UserSalary";
            IEnumerable<UserSalary> userSalaries = _dapper.LoadData<UserSalary>(sql);
            return userSalaries;
        }

        [HttpGet("GetSingleUserSalary/{userId}")]
        public UserSalary GetSingleUserSalary(int userId)
        {
            string sql = @"SELECT [UserId]
                                , [Salary]
                          FROM  TutorialAppSchema.UserSalary
                          WHERE UserId = " + userId.ToString();
            UserSalary userSalary = _dapper.LoadDataSingle<UserSalary>(sql);
            return userSalary;
        }

        [HttpPut("EditUserSalary")]
        public IActionResult EditUserSalary(UserSalary userSalary)
        {
            string sql = "UPDATE TutorialAppSchema.UserSalary " +
                    "SET [Salary] = '" + userSalary.Salary +
                    "' WHERE UserId = " + userSalary.UserId;
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Update User Salary");
        }

        [HttpPost("AddUserSalary")]
        public IActionResult AddUserSalary(UserSalary userSalary)
        {
            string sql = @" INSERT INTO TutorialAppSchema.UserSalary ([UserId],[Salary])
                            VALUES ('" + userSalary.UserId + "','" + userSalary.Salary + "')";


            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Add User Salary");
        }

        [HttpDelete("DeleteUserSalary/{userId}")]
        public IActionResult DeleteUserSalary(int userId)
        {
            string sql = @" DELETE FROM TutorialAppSchema.UserSalary 
                            WHERE UserId = " + userId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Delete User Salary");
        }

        [HttpGet("GetUserJobInfos")]
        public IEnumerable<UserJobInfo> GetUserJobInfos()
        {
            string sql = @"SELECT [UserId]
                                , [JobTitle]
                                , [Department]
                          FROM  TutorialAppSchema.UserJobInfo";
            IEnumerable<UserJobInfo> userJobInfos = _dapper.LoadData<UserJobInfo>(sql);
            return userJobInfos;
        }

        [HttpGet("GetSingleUserJobInfo/{userId}")]
        public UserJobInfo GetSingleUserJobInfo(int userId)
        {
            string sql = @"SELECT [UserId]
                                , [JobTitle]
                                , [Department]
                          FROM  TutorialAppSchema.UserJobInfo
                          WHERE UserId = " + userId.ToString();
            UserJobInfo userJobInfo = _dapper.LoadDataSingle<UserJobInfo>(sql);
            return userJobInfo;
        }

        [HttpPut("EditUserJobInfo")]
        public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
        {
            string sql = "UPDATE TutorialAppSchema.UserJobInfo " +
                    "SET [JobTitle] = '" + userJobInfo.JobTitle +
                    "',   [Department] = '" + userJobInfo.Department +
                    "' WHERE UserId = " + userJobInfo.UserId;
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Update User Job Info");
        }

        [HttpPost("AddUserJobInfo")]
        public IActionResult AddUserJobInfo(UserJobInfo userJobInfo)
        {
            string sql = @" INSERT INTO TutorialAppSchema.UserJobInfo ([UserId],[JobTitle],[Department])
                            VALUES ('" + userJobInfo.UserId + "','" + userJobInfo.JobTitle + "','" + userJobInfo.Department + "')";


            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Add User Job Info");
        }

        [HttpDelete("DeleteUserJobInfo/{userId}")]
        public IActionResult DeleteUserJobInfo(int userId)
        {
            string sql = @" DELETE FROM TutorialAppSchema.UserJobInfo 
                            WHERE UserId = " + userId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Delete User Job Info");
        }
    }
}
